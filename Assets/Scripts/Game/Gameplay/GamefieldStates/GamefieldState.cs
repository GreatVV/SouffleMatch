#region

using System.Collections.Generic;
using UnityEngine;

#endregion

namespace GamefieldStates
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Gamefield))]
    public abstract class GamefieldState : MonoBehaviour
    {
        public TilesCollection TilesCollection
        {
            get { return Gamefield.Level.Chuzzles; }
        }
        protected virtual void Awake()
        {
            Gamefield = GetComponent<Gamefield>();   
        }                
  
        public Gamefield Gamefield { get; private set; }

        #region Event Handlers
        public abstract void OnEnter();
        public abstract void OnExit();

        #endregion

        public abstract void UpdateState();
        public abstract void LateUpdateState();
    }
}