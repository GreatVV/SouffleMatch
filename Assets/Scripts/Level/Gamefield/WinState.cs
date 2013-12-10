using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class WinState : GamefieldState
{
    
    public GameObject TileReplaceEffect;

    #region Event Handlers

    public override void OnEnter()
    {
        if (Gamefield.GameMode.Turns > 0)
        {
            CreateBonusPowerUps();
        }
        else
        {
            var levelInfo = Player.Instance.GetLevelInfo(Gamefield.Level.Serialized.Name);
            var currentPoints = Gamefield.PointSystem.CurrentPoints;

            if (currentPoints > levelInfo.BestScore)
            {
                levelInfo.BestScore = currentPoints;
                //     FacebookIntegration.SendLevelResult(levelInfo.Name, currentPoints);
            }

            levelInfo.IsCompleted = true;
            levelInfo.NumberOfAttempts++;

            UI.Instance.GuiGameplay.OnWin();
        }
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

    public void CreateBonusPowerUps()
    {
        UI.Instance.BomBomPopup.BomBomHided += OnBomBomHided;
        UI.Instance.ShowBomBomTime();
    }

    public void OnBomBomHided()
    {
        var newPowerUps = new List<Chuzzle>();
        var usualChuzzles = Gamefield.Level.Chuzzles.Where(ch => !GamefieldUtility.IsPowerUp(ch)).ToList();

        for (var i = 0; i < Gamefield.GameMode.Turns; i++)
        {
            var newPowerUp = usualChuzzles[UnityEngine.Random.Range(0, usualChuzzles.Count())];
            newPowerUps.Add(newPowerUp);
            usualChuzzles.Remove(newPowerUp);
            if (!usualChuzzles.Any())
                break;
        }
        Gamefield.GameMode.Turns = 0;
        StartCoroutine(CreateNewPowerUps(newPowerUps.ToList()));
    }

    IEnumerator CreateNewPowerUps(List<Chuzzle> newPowerUps)
    {
        yield return new WaitForSeconds(1f);
        foreach (Chuzzle ch in newPowerUps)
        {
            ch.Explosion = TileReplaceEffect;
            ch.Destroy(false);
            yield return new WaitForSeconds(0.5f);
            TilesFactory.Instance.CreateBomb(ch.Current);
        }
        Gamefield.SwitchStateTo(Gamefield.WinRemoveCombinationState);
    }
}