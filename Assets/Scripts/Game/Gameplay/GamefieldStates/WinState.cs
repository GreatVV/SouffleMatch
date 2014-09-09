using System;
using Game;
using UnityEngine;

namespace GamefieldStates
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
            
            PanelManager.instance.WinPopup.Init(Gamefield.ManaManagerSystem.CurrentPoints);
            PanelManager.Show(PanelManager.instance.WinPopup);
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