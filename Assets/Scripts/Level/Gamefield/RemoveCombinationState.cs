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
    public List<List<Chuzzle>> Combinations;

    #region Event Handlers

    public override void OnEnter()
    {
        AnimatedChuzzles.Clear();
        Chuzzle.AnimationStarted += OnAnimationStarted;

        Combinations = GamefieldUtility.FindCombinations(Gamefield.Level.ActiveChuzzles);
        if (Combinations.Any())
        {   
            RemoveCombinations();
        }
        else
        {
            Gamefield.SwitchStateTo(Gamefield.FieldState);
        }
    }

    public override void OnExit()
    {
        if (AnimatedChuzzles.Any())
        {
            Debug.LogError("FUCK YOU: "+AnimatedChuzzles.Count);
        }
        Chuzzle.AnimationStarted -= OnAnimationStarted;
    }

    public void OnAnimationFinished(Chuzzle chuzzle)
    {
        chuzzle.AnimationFinished -= OnAnimationFinished;
        AnimatedChuzzles.Remove(chuzzle);
        if (!AnimatedChuzzles.Any())
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

    private void RemoveCombinations()
    {
        //remove combinations
        foreach (var combination in Combinations)
        {
            Gamefield.InvokeCombinationDestroyed(combination);

            //count points
            Gamefield.PointSystem.CountForCombinations(combination);


            foreach (var chuzzle in combination)
            {
                if (!chuzzle.IsDiying)
                {
                    chuzzle.Destroy(true);
                }
            }

            if (!AnimatedChuzzles.Any())
            {
                Gamefield.SwitchStateTo(Gamefield.CreateNewChuzzlesState);
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