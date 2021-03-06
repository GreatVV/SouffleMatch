﻿#region

using System;
using System.Collections;
using System.Collections.Generic;
using Game.Gameplay.Cells;
using Game.Gameplay.Chuzzles.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;
using Object = UnityEngine.Object;

#endregion

namespace Game.Gameplay.Chuzzles
{
    [RequireComponent(typeof (TeleportableEntity))]
    public abstract class Chuzzle : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
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
                    _shineParticle = ((GameObject) Instantiate(ShineParticlePrefab)).GetComponent<ParticleSystem>();
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
            Instance.ChuzzlePool.Release(Color, GetType(), gameObject);
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


            Teleportable = GetComponent<TeleportableEntity>();
            if (!VisualRoot)
            {
                VisualRoot = GetComponentInChildren<SpriteRenderer>().gameObject;
            }

            OnAwake();
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
        public GameObject VisualRoot;

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

        public void OnDrag(PointerEventData eventData)
        {
            Instance.Gamefield.OnDrag(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            FirePointerUp();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            FirePointerDown();
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
                if (!Explosion.GetComponent<ParticleSystem>().IsAlive(true))
                {
                    OnDeathAnimationEnd();
                }
            }
        }
    }
}