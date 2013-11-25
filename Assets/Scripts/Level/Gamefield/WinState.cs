using System;
using UnityEngine;

[Serializable]
public class WinState : GamefieldState
{          
    #region Event Handlers

    public override void OnEnter()
    {
        var levelInfo = Player.Instance.GetLevelInfo(Gamefield.Level.Serialized.Name);
        var currentPoints = Gamefield.PointSystem.CurrentPoints;
        
        if (currentPoints > levelInfo.BestScore)
        {
            levelInfo.BestScore = currentPoints;
            FacebookIntegration.SendLevelResult(levelInfo.Name, currentPoints);
        }

        levelInfo.IsCompleted = true;
        levelInfo.NumberOfAttempts++; 
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