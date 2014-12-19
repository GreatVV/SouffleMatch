using System;
using UnityEngine;

namespace Assets.Game.Gameplay.GamefieldStates
{
    [Serializable]
    public class WinState : GameState
    {
        public GameObject TileReplaceEffect;

        #region Event Handlers

        public WinState(Gamefield gamefield) : base(gamefield)
        {
        }

        public override void OnEnter()
        {
        }

        public override void OnExit()
        {
        }

        #endregion

        public override void UpdateState()
        {
        }

        public override void LateUpdateState()
        {
        }
    }
}