#region

using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using UnityEngine.EventSystems;

#endregion

[RequireComponent(typeof (TeleportableEntity))]
public abstract class Chuzzle : MonoBehaviour
{
    private static ExplosionPool ExplosionPool;
    public float Alpha;

    public ChuzzleColor Color;

    public Cell Current;
    public GameObject Explosion;
    public bool IsAnimationStarted;
    public bool IsCheckedForSearch;
    public bool IsDead;
    public bool IsReplacingOnDeath;
    public Cell MoveTo;
    public bool NeedCreateNew = true;
    public Cell Real;
    public TeleportableEntity Teleportable;

    public Vector3 Velocity;

    private ParticleSystem _shineParticle;
    public GameObject ShineParticlePrefab;

    public bool _tipping;

    public bool Tipping
    {
        get { return _tipping; }
        set
        {
            _tipping = value;
            
            if (!_shineParticle)
            {
                _shineParticle = ((GameObject) Instantiate(ShineParticlePrefab)).particleSystem;
                _shineParticle.transform.parent = transform;
                _shineParticle.transform.localPosition = Vector3.zero;
            }
            _shineParticle.gameObject.SetActive(value);
        }
    }

    public static Vector3 Scale
    {
        get { return Vector3.one; }
    }

    public bool Frozen { get; set; }

    #region Events

    public event Action<Chuzzle> Died;
    public event Action<Chuzzle> AnimationFinished;
    public event Action<Chuzzle> AnimationStarted;

    #endregion

    #region Event Handlers

    private void OnDeathAnimationEnd()
    {
        ChuzzlePool.Instance.Release(Color, GetType(), gameObject);
        InvokeAnimationFinished();
        InvokeDied();
        // Object.Destroy(gameObject);
    }

    #endregion

    #region Event Invokators

    public virtual void InvokeAnimationFinished()
    {
        if (IsAnimationStarted == false)
        {
            Debug.LogError("Try to finish finished animation on " + name);
        }
        IsAnimationStarted = false;
        var handler = AnimationFinished;
        if (handler != null) handler(this);
    }

    protected virtual void InvokeDied()
    {
        Action<Chuzzle> handler = Died;
        if (handler != null) handler(this);
    }

    public void InvokeAnimationStarted()
    {
        if (IsAnimationStarted)
        {
            Debug.LogError("Already started on " + name + " " + GetInstanceID());
        }
        IsAnimationStarted = true;
        Action<Chuzzle> handler = AnimationStarted;
        if (handler != null) handler(this);
    }

    #endregion

    #region Events Subscribers

    protected abstract void OnAwake();

    #endregion

    #region Unity Methods

    private void Awake()
    {
        if (!ExplosionPool)
        {
            ExplosionPool = FindObjectOfType<ExplosionPool>();
        }

        if (!EventTrigger)
        {
            EventTrigger = gameObject.AddComponent<EventTrigger>();
        }
        var onPointerDownCallback = new EventTrigger.TriggerEvent();
        onPointerDownCallback.AddListener(OnPointerDown);

         var onPointerUpCallback = new EventTrigger.TriggerEvent();
        onPointerUpCallback.AddListener(OnPointerUp);

        var onMoveCallback = new EventTrigger.TriggerEvent();
        onMoveCallback.AddListener(OnMove);

        EventTrigger.delegates = new List<EventTrigger.Entry>()
        {
            new EventTrigger.Entry()
            {
                eventID = EventTriggerType.PointerDown,
                callback = onPointerDownCallback
            },
            new EventTrigger.Entry()
            {
                eventID = EventTriggerType.PointerUp,
                callback = onPointerUpCallback
            },
            new EventTrigger.Entry()
            {
                eventID = EventTriggerType.Drag,
                callback = onMoveCallback
            }
        };


        Teleportable = GetComponent<TeleportableEntity>();
        OnAwake();
    }

    public void OnMove(BaseEventData eventData)
    {
        Gamefield.Instance.OnDrag(eventData);
    }

    private void OnPointerUp(BaseEventData arg0)
    {
        FirePointerUp();
    }

    private void OnPointerDown(BaseEventData arg0)
    {
        FirePointerDown();
    }

    public event Action<Chuzzle> PointerDown;

    protected virtual void FirePointerDown()
    {
        var handler = PointerDown;
        if (handler != null) handler(this);
    }

    public event Action<Chuzzle> PointerUp;

    protected virtual void FirePointerUp()
    {
        var handler = PointerUp;
        if (handler != null) handler(this);
    }

    public EventTrigger EventTrigger;

    private void OnEnable()
    {
        transform.localScale = Vector3.one;
        IsDead = false;
    }

    #endregion

    public override string ToString()
    {
        return string.Format("{0} ({1},{2}) - {3}", Color, Current.X, Current.Y, GetType());
    }

    protected virtual void Die(bool withAnimation)
    {
        if (IsDead)
        {
            return;
        }
        InvokeAnimationStarted();
        IsDead = true;
        // Debug.Log("Die: " + name + " " + GetInstanceID());
        //TODO Do Explosion
        if (Math.Abs(transform.localScale.x) > 0.01f && withAnimation)
        {
            ExplosionPool.Explode(this);

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

    

    public virtual void Destroy(bool needCreateNew, bool withAnimation = true, bool isReplacingOnDeath = false)
    {
        IsReplacingOnDeath = isReplacingOnDeath;
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
}