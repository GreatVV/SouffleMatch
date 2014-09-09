#region

using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

#endregion

namespace GamefieldStates
{
    [Serializable]
    public abstract class GameState
    {
        public TilesCollection TilesCollection
        {
            get { return Gamefield.Level.Chuzzles; }
        }

        protected GameState(Gamefield gamefield)
        {
            Gamefield = gamefield;
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