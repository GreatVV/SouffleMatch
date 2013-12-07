#region

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

#endregion

[Serializable]
public class WinRemoveCombinationState : GamefieldState
{
    #region Event Handlers

    public override void OnEnter()
    {
        AnimatedChuzzles.Clear();
        Chuzzle.AnimationStarted += OnAnimationStarted;

        var powerUpChuzzles =
            from ch in Gamefield.Level.Chuzzles
            where GamefieldUtility.IsPowerUp(ch)
            select ch;
        Debug.Log("WinremoveCombinationState!");
        foreach (Chuzzle ch in powerUpChuzzles)
        {
            ch.Destroy(true, true);
        }

        var combinations = GamefieldUtility.FindCombinations(Gamefield.Level.ActiveChuzzles);
        if (combinations.Any())
        {
            RemoveCombinations(combinations);
        }
        else
        {
            Gamefield.SwitchStateTo(Gamefield.WinCreateNewChuzzlesState);
        }
    }

    public override void OnExit()
    {
        if (AnimatedChuzzles.Any())
        {
            Debug.LogError("FUCK YOU: " + AnimatedChuzzles.Count);
        }
        Chuzzle.AnimationStarted -= OnAnimationStarted;
    }

    public void OnAnimationFinished(Chuzzle chuzzle)
    {
        chuzzle.AnimationFinished -= OnAnimationFinished;
        AnimatedChuzzles.Remove(chuzzle);
        if (!AnimatedChuzzles.Any())
        {
            Gamefield.SwitchStateTo(Gamefield.WinCreateNewChuzzlesState);
        }
    }

    #endregion

    public override void UpdateState()
    {
    }

    public override void LateUpdateState()
    {
    }

    private void RemoveCombinations(IEnumerable<List<Chuzzle>> combinations)
    {
        //remove combinations
        foreach (var combination in combinations)
        {
            Gamefield.InvokeCombinationDestroyed(combination);

            //count points
            Gamefield.PointSystem.CountForCombinations(combination);

            foreach (var chuzzle in combination)
            {
                chuzzle.Destroy(true);
            }
        }
    }

    private void OnAnimationStarted(Chuzzle chuzzle)
    {
        if (!AnimatedChuzzles.Contains(chuzzle))
        {
            AnimatedChuzzles.Add(chuzzle);
            chuzzle.AnimationFinished += OnAnimationFinished;
        }
    }
}
