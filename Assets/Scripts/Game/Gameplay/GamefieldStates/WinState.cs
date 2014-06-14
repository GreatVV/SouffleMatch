﻿using System;
using UnityEngine;

namespace GamefieldStates
{
    [Serializable]
    public class WinState : GamefieldState
    {
        public GameObject TileReplaceEffect;

        #region Event Handlers

        public override void OnEnter()
        {
            TilesCollection = Gamefield.Level.Chuzzles;
            PanelManager.instance.WinPopup.Init(Gamefield.PointSystem.CurrentPoints);
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