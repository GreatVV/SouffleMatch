using System;
using UnityEngine;

[Serializable]
public class InitState : GamefieldState
{           
    #region Event Handlers

    public override void OnEnter()
    {
        Gamefield.PointSystem.Reset();
        Gamefield.Level.Reset();

        Gamefield.CombinationDestroyed -= InvaderChuzzle.OnCombinationDestroyed;
        Gamefield.CombinationDestroyed += InvaderChuzzle.OnCombinationDestroyed;

        Gamefield.Level.InitFromFile(Player.Instance.LastPlayedLevel);
        Gamefield.StageManager.Init(Player.Instance.LastPlayedLevel.Stages);  

        Gamefield.NewTilesInColumns = new int[Gamefield.Level.Width];

        Gamefield.AddEventHandlers();

        Gamefield.InvokeGameStarted();
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