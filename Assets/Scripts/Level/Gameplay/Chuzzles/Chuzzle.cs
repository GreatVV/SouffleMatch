#region

using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

#endregion

[RequireComponent(typeof (TeleportableEntity))]
public abstract class Chuzzle : MonoBehaviour
{
    public float Alpha;

    public GameObject Arrow;
    public ChuzzleColor Color;

    public Cell Current;
    public GameObject Explosion;
    public bool IsCheckedForSearch;
    public Cell MoveTo;
    public Cell Real;

    public Vector3 Velocity;

    public bool _shine;
    public bool IsAnimationStarted;

    public bool NeedCreateNew = true;
    public TeleportableEntity Teleportable;

    public bool IsDead;

    #region Events

    public event Action<Chuzzle> Died;
    public event Action<Chuzzle> AnimationFinished;
    public static event Action<Chuzzle> AnimationStarted;
    
    #endregion

    #region Event Handlers

    private void OnDeathAnimationEnd()
    {   
        if (IsAnimationStarted)
        {
            InvokeAnimationFinished();
        }
        Object.Destroy(gameObject);
    }

    private void OnAnimateMoveEnd(object obj)
    {
        InvokeAnimationFinished();
    }

    #endregion

    #region Event Invokators

    protected virtual void InvokeAnimationFinished()
    {
        if (IsAnimationStarted == false)
        {
            Debug.LogError("Try to finish finished animation on "+name);
        }
        IsAnimationStarted = false;
        var handler = AnimationFinished;
        if (handler != null) handler(this);
    }

    protected virtual void InvokeDied()
    {   
        var handler = Died;
        if (handler != null) handler(this);
    }

    protected void InvokeAnimationStarted()
    {
        if (IsAnimationStarted)
        {
            Debug.LogError("Already started on " + name + " "+GetInstanceID());
        }
        IsAnimationStarted = true;
        var handler = AnimationStarted;
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
                Object.Destroy(Arrow);
            }
        }
    }

    public static Vector3 Scale
    {
        get { return Vector3.one; }
    }

    public override string ToString()
    {
        return string.Format("{0} ({1},{2}) - {3}", Color, Current.x, Current.y, GetType());
    }

    public bool Frozen { get; set; }

    protected virtual void Die(bool withAnimation)
    {
        if (IsDead)
        {
            return;
        }

        IsDead = true;
       // Debug.Log("Die: " + name + " " + GetInstanceID());
        InvokeDied();
        //TODO Do Explosion
        if (Math.Abs(transform.localScale.x) > 0.01f && withAnimation)
        {
            if (IsAnimationStarted)
            {
                return;
            }
            InvokeAnimationStarted();
            iTween.ScaleTo(gameObject,
                iTween.Hash(
                    "x", 0,
                    "y", 0,
                    "z", 0,
                    "time", 0.2f));
            var ps = Instantiate(Explosion) as GameObject;
            //  Debug.Log("Ps: "+ps);
            ps.transform.position = transform.position;
            if (gameObject.activeSelf)
            {
                StartCoroutine("CheckIfAlive");
            }
        }
        else
        {
            OnDeathAnimationEnd();
        }
    }

    public void AnimateMoveTo(Vector3 targetPosition, float time = 0.3f)
    {
        //Debug.Log("Move: "+name+" "+GetInstanceID());
        
        if (Vector3.Distance(targetPosition, transform.position) > 0.01f)
        {
            if (IsAnimationStarted)
            {
                return;
            }
            InvokeAnimationStarted();
            iTween.MoveTo(gameObject,
                iTween.Hash(
                    "x", targetPosition.x,
                    "y", targetPosition.y,
                    "z", targetPosition.z,
                    "time", time,
                    "easetype", iTween.EaseType.easeInOutQuad,
                    "oncomplete", new Action<object>(OnAnimateMoveEnd),
                    "oncompleteparams", gameObject
                    ));
        }
        else
        {
            transform.position = targetPosition;
        }
    }    

    public virtual void Destroy(bool needCreateNew, bool withAnimation = true)
    {
        NeedCreateNew = needCreateNew;
        Die(withAnimation);
    }

    private IEnumerator CheckIfAlive()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            if (!Explosion.particleSystem.IsAlive(true))
            {
                OnDeathAnimationEnd();
            }
        }
    }

    void Awake()
    {
        Teleportable = GetComponent<TeleportableEntity>();
        OnAwake();
    }

    protected abstract void OnAwake();

    public static void DropEventHandlers()
    {
        AnimationStarted = null;
    }
}