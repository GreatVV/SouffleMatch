#region

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#endregion

[RequireComponent(typeof (TeleportableEntity))]
public abstract class Chuzzle : MonoBehaviour
{
    public float Alpha;

    public GameObject Arrow;

    public bool IsCheckedForSearch;

    public Cell Current;
    public Cell MoveTo;
    public Cell Real;

    public ChuzzleColor Color;

    public Vector3 Velocity;
    
    public bool _shine;

    public GameObject Explosion;

    #region Events

    public event Action<Chuzzle> Died;
    public event Action<Chuzzle> AnimationFinished;

    #endregion

    #region Event Handlers

    private void OnDisable()
    {
        //drop all event handlers
        Died = null;
    }

    private void OnDeathAnimationEnd(object chuzzle)
    {
        InvokeDied();
        InvokeAnimationFinished();
        Destroy(gameObject);
    }

    #endregion

    #region Event Invokators

    protected virtual void InvokeAnimationFinished()
    {
        var handler = AnimationFinished;
        if (handler != null) handler(this);
    }

    protected virtual void InvokeDied()
    {
        var handler = Died;
        if (handler != null) handler(this);
    }

    #endregion

    public bool Shine
    {
        get { return _shine; }
        set
        {
            _shine = value;
            Alpha = 1;
            if (!_shine && Arrow != null)
            {
                Destroy(Arrow);
            }
        }
    }

    public Vector3 Scale
    {
        get { return transform.localScale; }
    }

    public override string ToString()
    {
        return string.Format("{0} ({1},{2}) - {3}", Color, Current.x, Current.y, GetType());
    }

    public bool Frozen { get; set; }
    public bool IsDiying;
    public static event Action<Chuzzle> AnimationStarted;

    protected void InvokeAnimationStarted()
    {
        var handler = AnimationStarted;
        if (handler != null) handler(this);
    }

    protected virtual void Die()
    {
        InvokeAnimationStarted();
        IsDiying = true;
        //TODO Do Explosion
        if (Math.Abs(transform.localScale.x) > 0.01f)
        {
            iTween.ScaleTo(gameObject,
                iTween.Hash(
                    "x", 0,
                    "y", 0,
                    "z", 0,
                    "time", 0.4f));
            var ps = Instantiate(Explosion) as GameObject;
          //  Debug.Log("Ps: "+ps);
            ps.transform.position = transform.position;
            StartCoroutine("CheckIfAlive");
        }
        else
        {
            OnDeathAnimationEnd(this);
        }
    }

    public virtual void Destroy()
    {
        Die();
    }

    IEnumerator CheckIfAlive()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            if (!Explosion.particleSystem.IsAlive(true))       
            {
                OnDeathAnimationEnd(this);
            }
        }
    }
}