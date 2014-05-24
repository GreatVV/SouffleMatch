#region

using System.Collections.Generic;
using UnityEngine;

#endregion

namespace GamefieldStates
{
    [RequireComponent(typeof(global::Gamefield))]
    public abstract class GamefieldState : MonoBehaviour
    {
        public List<Chuzzle> AnimatedChuzzles = new List<Chuzzle>();  
        protected virtual void Awake()
        {
            Gamefield = GetComponent<global::Gamefield>();   
        }                
  
        public global::Gamefield Gamefield { get; private set; }

        #region Event Handlers

        public abstract void OnEnter();
        public abstract void OnExit();

        #endregion

        public abstract void UpdateState();
        public abstract void LateUpdateState();
    }
}