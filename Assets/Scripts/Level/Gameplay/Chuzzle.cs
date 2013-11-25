#region

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#endregion

[RequireComponent(typeof (TeleportableEntity))]
public class Chuzzle : MonoBehaviour
{
    public float Alpha;

    public GameObject Arrow;
    public int Counter;
    public Cell Current;

    public bool IsCheckedForSearch;
    public Cell MoveTo;

    public PowerType PowerType;
    public Cell Real;

    public SpriteRenderer Sprite;
    public int TimesDestroyed;
    public ChuzzleType Type;

    public Vector3 Velocity;
    private int _counter;
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
        return "" + Type + " (" + Current.x + "," + Current.y + ")";
    }

    private void Update()
    {
        /*
        if (Shine && !Frozen)
        {
            Alpha = Alpha + Time.deltaTime*3;
            Sprite.color = new Color(Sprite.color.r, Sprite.color.g, Sprite.color.b, Mathf.Sin(Alpha)/2f + 0.5f);
        }                           */
    }

    public bool Frozen { get; set; }

    private void Die()
    {
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

    public void Destroy(List<Chuzzle> combination)
    {
        TimesDestroyed++;
        if (PowerType == PowerType.TwoTimes)
        {
            if (TimesDestroyed == 2)
            {
                Die();
            }
            else
            {
                InvokeAnimationFinished();
            }
            return;
        }


        if (Counter > 0)
        {
            InvokeAnimationFinished();
        }
        else
        {
            Die();
        }
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