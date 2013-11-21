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
    public List<Chuzzle> DeathAnimationChuzzles = new List<Chuzzle>();          

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

    public void OnCompleteDeath(object chuzzleObject)
    {
        var chuzzle = chuzzleObject as Chuzzle;

        DeathAnimationChuzzles.Remove(chuzzle);
        Destroy(chuzzle.gameObject);

        if (DeathAnimationChuzzles.Count == 0)
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
            var powerUp = combination.FirstOrDefault(x => x.PowerType != PowerType.Usual);
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
            if (chuzzle.Counter > 0)
            {
                continue;
            }
            //remove chuzzle from game logic
            Gamefield.RemoveChuzzle(chuzzle);

            chuzzle.Die();
         

            if (chuzzle.gameObject.transform.localScale != Vector3.zero)
            {
                if (!DeathAnimationChuzzles.Contains(chuzzle))
                {
                    DeathAnimationChuzzles.Add(chuzzle);

                    iTween.ScaleTo(chuzzle.gameObject,
                        iTween.Hash(
                            "x", 0,
                            "y", 0,
                            "z", 0,
                            "time", 0.5f,
                            "oncomplete", new Action<object>(OnCompleteDeath),
                            "oncompleteparams", chuzzle));
                }
            }
        }

        if (!DeathAnimationChuzzles.Any())
        {
            Gamefield.SwitchStateTo(Gamefield.CreateNewChuzzlesState);
        }
    }
}