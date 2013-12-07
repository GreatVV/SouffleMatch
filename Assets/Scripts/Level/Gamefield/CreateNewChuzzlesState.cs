﻿#region

using System;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

#endregion

[Serializable]
public class CreateNewChuzzlesState : GamefieldState
{
    public GameObject WinBonusTitle;
    public GameObject TileReplaceEffect;

    #region Event Handlers

    public override void OnEnter()
    {
        AnimatedChuzzles.Clear();
        Chuzzle.AnimationStarted += OnAnimationStarted;
        CreateNew();
    }

    private void OnAnimationStarted(Chuzzle chuzzle)
    {
        if (!AnimatedChuzzles.Contains(chuzzle))
        {
            AnimatedChuzzles.Add(chuzzle);
            chuzzle.AnimationFinished += OnAnimationFinished;
        }
    }

    public override void OnExit()
    {
        if (AnimatedChuzzles.Any())
        {
            Debug.LogError("FUCK YOU: " + AnimatedChuzzles.Count);
        }
        Gamefield.NewTilesInColumns = new int[Gamefield.Level.Width];
        Chuzzle.AnimationStarted -= OnAnimationStarted;
    }

    public void OnAnimationFinished(Chuzzle chuzzle)
    {
        chuzzle.Real = chuzzle.Current = chuzzle.MoveTo;

        chuzzle.AnimationFinished -= OnAnimationFinished;   
        AnimatedChuzzles.Remove(chuzzle);
        
        if (!AnimatedChuzzles.Any())
        {
            Gamefield.Level.UpdateActive();

            var combinations = GamefieldUtility.FindCombinations(Gamefield.Level.ActiveChuzzles);
            if (combinations.Count > 0) 
            {
                Gamefield.SwitchStateTo(Gamefield.CheckSpecialState);
            }
            else
            {
                if (!Gamefield.GameMode.IsWin && !Gamefield.GameMode.IsGameOver)
                {
                    Gamefield.SwitchStateTo(Gamefield.FieldState);
                }
                else 
                {
                    if (Gamefield.GameMode.Turns > 0 && Gamefield.GameMode.IsWin)
                    {
                        CreateBonusPowerUps();
                    }
                    else
                    {
                        Gamefield.GameMode.Check();
                    }
                }
            }
        }
    }

    public void OnWinTitleDestroyed()
    {
        List<Chuzzle> NewPowerUps = new List<Chuzzle>();
        List<Chuzzle> usualChuzzles = new List<Chuzzle>();
        var usualChuzzlesCollection =
                from ch in Gamefield.Level.Chuzzles
                where !GamefieldUtility.IsPowerUp(ch)
                select ch;
        usualChuzzles = usualChuzzlesCollection.ToList();
        for (var i = 0; i < Gamefield.GameMode.Turns; i++)
        {
            var newPowerUp = usualChuzzles[UnityEngine.Random.Range(0, usualChuzzles.Count())];
            NewPowerUps.Add(newPowerUp);
            usualChuzzles.Remove(newPowerUp);
            if (!usualChuzzles.Any())
                break;
        }
        StartCoroutine(NewCoroutine(NewPowerUps.ToList()));
    }

    IEnumerator NewCoroutine(List<Chuzzle> NewPowerUps)
    {
        yield return new WaitForSeconds(1f);
        foreach(Chuzzle ch in NewPowerUps)
        {
            ch.Destroy(false, false);
            var ps = Instantiate(TileReplaceEffect) as GameObject;
            ps.transform.position = ch.transform.position;
            yield return new WaitForSeconds(0.5f);
            TilesFactory.Instance.CreateBomb(ch.Current);
        }
        Gamefield.SwitchStateTo(Gamefield.WinRemoveCombinationState);
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
                    var chuzzle = TilesFactory.Instance.CreateChuzzle(Gamefield.Level.GetCellAt(x, Gamefield.Level.Height + j));
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
                var targetPosition = new Vector3(c.Current.x*Chuzzle.Scale.x, c.MoveTo.y*Chuzzle.Scale.y, 0);
                c.AnimateMoveTo(targetPosition);
            }
        }
        return true;
    }

    public void CreateBonusPowerUps()
    {
        var SuffleTime = Instantiate(WinBonusTitle) as GameObject;
        SuffleTime.GetComponent<CreateBonusTitle>().WinTitleDestroyed += OnWinTitleDestroyed;
    }
}