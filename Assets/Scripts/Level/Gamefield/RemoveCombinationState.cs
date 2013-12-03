#region

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

#endregion

[Serializable]
public class RemoveCombinationState : GamefieldState
{
    public List<Chuzzle> AnimatedChuzzles = new List<Chuzzle>();          

    #region Event Handlers

    public override void OnEnter()
    {
        var combinations = GamefieldUtility.FindCombinations(Gamefield.Level.ActiveChuzzles);
        if (combinations.Any())
        {   
            RemoveCombinations(combinations);
        }
        else
        {
            Gamefield.SwitchStateTo(Gamefield.FieldState);
        }
    }

    public override void OnExit()
    {
    }

    public void OnAnimationFinished(Chuzzle chuzzle)
    {
        chuzzle.AnimationFinished -= OnAnimationFinished;
        AnimatedChuzzles.Remove(chuzzle);
        if (AnimatedChuzzles.Count == 0)
        {
            Gamefield.SwitchStateTo(Gamefield.CreateNewChuzzlesState);
        }
    }

    #endregion

    public override void UpdateState()
    {
    }

    public override void LateUpdateState()
    {
    }

    public void RemoveCombinations(List<List<Chuzzle>> combinations)
    {
        //remove combinations
        foreach (var combination in combinations)
        {
            var powerUp = combination.FirstOrDefault(GamefieldUtility.IsPowerUp);
            if (powerUp != null)
            {
                Gamefield.ApplyPowerUp(combination, powerUp);
            }
            RemoveTiles(combination, true);
        }
    }

    

    public void RemoveTiles(List<Chuzzle> combination, bool needCountPoints)
    {
        Gamefield.InvokeCombinationDestroyed(combination);
        if (needCountPoints)
        {
            //count points
            Gamefield.PointSystem.CountForCombinations(combination);
        }

        foreach (var chuzzle in combination)
        {
            RemoveChuzzle(chuzzle);
        }
    }

    public void RemoveChuzzle(Chuzzle chuzzle)
    {
        if (chuzzle is CounterChuzzle)
        {
            return;
        }

        if (!AnimatedChuzzles.Contains(chuzzle))
        {
            AnimatedChuzzles.Add(chuzzle);
            chuzzle.AnimationFinished += OnAnimationFinished;
            chuzzle.Destroy();
        }
    }
}