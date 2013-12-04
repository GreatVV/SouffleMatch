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
        AnimatedChuzzles.Clear();
        Chuzzle.AnimationStarted += OnAnimationStarted;

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
        Chuzzle.AnimationStarted -= OnAnimationStarted;
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

    private void RemoveCombinations(IEnumerable<List<Chuzzle>> combinations)
    {
        //remove combinations
        foreach (var combination in combinations)
        {   
            RemoveTiles(combination);
        }
    }

    private void RemoveTiles(IEnumerable<Chuzzle> combination)
    {
        var enumerable = combination as Chuzzle[] ?? combination.ToArray();
        Gamefield.InvokeCombinationDestroyed(enumerable);

        //count points
        Gamefield.PointSystem.CountForCombinations(enumerable);


        foreach (var chuzzle in enumerable)
        {
            RemoveChuzzle(chuzzle);
        }
    }

    private void RemoveChuzzle(Chuzzle chuzzle)
    {
        chuzzle.Destroy();
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