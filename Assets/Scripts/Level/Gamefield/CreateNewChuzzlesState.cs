using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class CreateNewChuzzlesState : GamefieldState
{
    public List<Chuzzle> NewTilesAnimationChuzzles = new List<Chuzzle>();    

    #region Event Handlers

    public override void OnEnter()
    {
        CreateNew();
    }

    public override void OnExit()
    {
        Gamefield.NewTilesInColumns = new int[Gamefield.Level.Width];
    }

    public void OnCompleteNewChuzzleTween(object chuzzleObject)
    {
        var chuzzle = chuzzleObject as Chuzzle;
        chuzzle.Real = chuzzle.Current = chuzzle.MoveTo;

        if (NewTilesAnimationChuzzles.Contains(chuzzle))
        {      
            NewTilesAnimationChuzzles.Remove(chuzzle);
        }

        if (!NewTilesAnimationChuzzles.Any())
        {
            Gamefield.Level.UpdateActive();

            var combinations = GamefieldUtility.FindCombinations(Gamefield.Level.ActiveChuzzles);
            if (combinations.Count > 0)
            {
                Gamefield.SwitchStateTo(Gamefield.CheckSpecialState);
            }
            else
            {
                //check gameover or win
                if (!Gamefield.GameMode.Check())
                {
                     Gamefield.SwitchStateTo(Gamefield.FieldState);
                }
            }
        }
    }

    #endregion

    public override void UpdateState()
    {
    }

    public override void LateUpdateState()
    {
    }

    public bool CreateNew()
    {
        var hasNew = Gamefield.NewTilesInColumns.Any(x => x > 0);
        if (!hasNew)
        {
            Gamefield.SwitchStateTo(Gamefield.CheckSpecialState);
            return false;
        }

        //check if need create new tiles
        for (var x = 0; x < Gamefield.NewTilesInColumns.Length; x++)
        {
            var newInColumn = Gamefield.NewTilesInColumns[x];
            if (newInColumn > 0)
            {
                for (var j = 0; j < newInColumn; j++)
                {
                    //create new tiles
                    var chuzzle = Gamefield.Level.CreateRandomChuzzle(Gamefield.Level.GetCellAt(x, Gamefield.Level.Height + j), true);
                    chuzzle.Current.IsTemporary = true;
                }
            }
        }

        //move tiles to fill positions
        for (var x = 0; x < Gamefield.NewTilesInColumns.Length; x++)
        {
            var newInColumn = Gamefield.NewTilesInColumns[x];
            if (newInColumn > 0)
            {
                for (var y = 0; y < Gamefield.Level.Height; y++)
                {
                    var cell = Gamefield.Level.GetCellAt(x, y, false);
                    if (Gamefield.Level.At(x, y) == null && cell.Type != CellTypes.Block)
                    {
                        while (cell != null)
                        {
                            var chuzzle = Gamefield.Level.At(cell.x, cell.y);
                            if (chuzzle != null)
                            {
                                chuzzle.MoveTo = chuzzle.MoveTo.GetBottomWithType();
                                //Level.GetCellAt(chuzzle.MoveTo.x, chuzzle.MoveTo.y - 1);                                
                            }
                            cell = cell.Top;
                        }
                    }
                }
            }
        }

        foreach (var c in Gamefield.Level.Chuzzles)
        {
            if (c.MoveTo.y != c.Current.y)
            {
                NewTilesAnimationChuzzles.Add(c);
                var targetPosition = new Vector3(c.Current.x*c.Scale.x, c.MoveTo.y*c.Scale.y, 0);
                iTween.MoveTo(c.gameObject,
                    iTween.Hash("x", targetPosition.x, "y", targetPosition.y, "z", targetPosition.z, "time", 0.3f,
                        "oncomplete", new Action<object>(OnCompleteNewChuzzleTween), "oncompletetarget", Gamefield.gameObject,
                        "oncompleteparams", c));
            }
        }

        if (!NewTilesAnimationChuzzles.Any())
        {
            Gamefield.SwitchStateTo(Gamefield.CheckSpecialState);
        }
        return true;
    }
}