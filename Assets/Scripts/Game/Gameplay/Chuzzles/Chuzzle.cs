#region

using System;
using System.Collections;
using Game;
using UnityEngine;

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

    protected void InvokeAnimationStarted()
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

        Teleportable = GetComponent<TeleportableEntity>();
        OnAwake();
    }

    private void OnEnable()
    {
        transform.localScale = Vector3.one;
        IsDead = false;
    }

    #endregion

    public override string ToString()
    {
        return string.Format("{0} ({1},{2}) - {3}", Color, Current.x, Current.y, GetType());
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

    public void AnimateMoveTo(Vector3 targetPosition, float time = 0.3f)
    {
        if (IsDead)
        {
            return;
        }
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