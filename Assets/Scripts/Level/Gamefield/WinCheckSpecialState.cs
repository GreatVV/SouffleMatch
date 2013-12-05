#region

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

#endregion

[Serializable]
public class WinCheckSpecialState : GamefieldState
{


    #region Event Handlers

    public override void OnEnter()
    {
        AnimatedChuzzles.Clear();
        Chuzzle.AnimationStarted += OnAnimationStarted;

        var combinations = GamefieldUtility.FindCombinations(Gamefield.Level.ActiveChuzzles);
        if (!CheckForSpecial(combinations))
        {
            Gamefield.SwitchStateTo(Gamefield.RemoveState);
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

    private void OnAnimationStarted(Chuzzle chuzzle)
    {
        if (!AnimatedChuzzles.Contains(chuzzle))
        {
            AnimatedChuzzles.Add(chuzzle);
            chuzzle.AnimationFinished += OnAnimationFinished;
        }
    }

    private void OnAnimationFinished(Chuzzle chuzzle)
    {
        chuzzle.AnimationFinished -= OnAnimationFinished;
        AnimatedChuzzles.Remove(chuzzle);

        Gamefield.RemoveChuzzle(chuzzle);
        Destroy(chuzzle.gameObject);

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

    public bool CheckForSpecial(List<List<Chuzzle>> combinations)
    {
        var isNewSpecial = false;

        foreach (var comb in combinations)
        {
            //if any tile is powerup - then don't check for new bonuses
            //or any tile has counter
            if (comb.Any(x => !(GamefieldUtility.IsUsual(x))))
            {
                continue;
            }

            if (comb.Count == 4)
            {
                isNewSpecial = CreateLine(comb);
            }
            else
            {
                if (comb.Count >= 5)
                {
                    isNewSpecial = CreateBomb(comb);
                }
            }
        }

        return isNewSpecial;
    }

    private bool CreateBomb(List<Chuzzle> comb)
    {
        return CreateSpecialWithType(comb, TilesFactory.Instance.BombChuzzlePrefabs);
    }

    public bool CreateSpecialWithType(List<Chuzzle> comb, GameObject[] prefabs)
    {
        var ordered = comb;

        var targetTile = comb[Random.Range(0, ordered.Count)];
        var cellForNew = targetTile.Current;
        foreach (var chuzzle in ordered)
        {
            if (chuzzle != targetTile)
            {
                chuzzle.MoveTo = cellForNew;
            }
        }

        var powerUp = prefabs.First(x => x.GetComponent<Chuzzle>().Color == targetTile.Color);
        var powerUpChuzzle = TilesFactory.Instance.CreateChuzzle(targetTile.Current, powerUp);
        powerUpChuzzle.Color = targetTile.Color;

        var child = powerUpChuzzle.transform.GetChild(0).gameObject;
        Destroy(child.GetComponent<BoxCollider>());

        Gamefield.InvokeTileDestroyed(targetTile);
        Destroy(targetTile.gameObject);
        Gamefield.Level.ActiveChuzzles.Remove(targetTile);
        Gamefield.Level.Chuzzles.Remove(targetTile);
        ordered.Remove(targetTile);

        foreach (var c in ordered)
        {
            var targetPosition = new Vector3(c.MoveTo.x * Chuzzle.Scale.x, c.MoveTo.y * Chuzzle.Scale.y, 0);
            c.AnimateMoveTo(targetPosition);
        }

        return true;
    }

    private bool CreateLine(List<Chuzzle> comb)
    {
        return CreateSpecialWithType(comb,
            Random.Range(0, 100) > 50 ? TilesFactory.Instance.HorizontalLineChuzzlePrefabs : TilesFactory.Instance.VerticalLineChuzzlePrefabs);
    }
}
