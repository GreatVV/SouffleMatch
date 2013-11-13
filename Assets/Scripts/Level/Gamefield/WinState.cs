﻿using System;
using UnityEngine;

[Serializable]
public class WinState : GamefieldState
{
    public WinState(Gamefield gamefield = null)
        : base(gamefield)
    {
    }

    #region Event Handlers

    public override void OnEnter()
    {
        var levelInfo = Player.Instance.GetLevelInfo(Gamefield.Level.Serialized.Name);
        var currentPoints = Gamefield.PointSystem.CurrentPoints;
        var level = Gamefield.Level.Serialized;
        if (currentPoints > levelInfo.BestScore)
        {
            levelInfo.BestScore = currentPoints;
        }

        levelInfo.IsCompleted = true;
        levelInfo.NumberOfAttempts++;

        
        var numberOfStars = currentPoints <= level.Star2Score ? 1 : (currentPoints <= level.Star3Score ? 2 : 3);
    }

    public override void OnExit()
    {
    }

    #endregion

    public override void Update()
    {
    }

    public override void LateUpdate()
    {
    }
}