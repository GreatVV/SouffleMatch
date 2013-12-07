using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

[Serializable]
public class TargetChuzzleGameMode : GameMode
{
    public int TargetAmount;
    public int Amount;
    public CounterChuzzle TargetChuzzle;

    public TargetChuzzleGameMode(GameModeDescription description) : base(description)
    {
        Amount = TargetAmount = description.Amount;
    }

    protected override void OnInit()
    {
        Gamefield.CombinationDestroyed -= OnCombinationDestroyed;
        Gamefield.CombinationDestroyed += OnCombinationDestroyed;
    }


    public override void OnDestroy()
    {
        Gamefield.CombinationDestroyed -= OnCombinationDestroyed;
    }

    private void OnCombinationDestroyed(IEnumerable<Chuzzle> destroyedChuzzles)
    {
        //TODO find chuzzle (it's special type)
        if (TargetChuzzle == null)
        {
            TargetChuzzle = Gamefield.Level.ActiveChuzzles.FirstOrDefault(x => x is CounterChuzzle) as CounterChuzzle;
            if (TargetChuzzle == null)
            {
                Debug.LogError("No target chuzzle");
                return;
            }
            SetTargetAmount(Amount);
        }
       
        if (destroyedChuzzles.Contains(TargetChuzzle))
        {
            Amount -= destroyedChuzzles.Count()-1;
            SetTargetAmount(Amount);                                                               
        }


        if (Amount <= 0)
        {
            IsWin = true;
        }
    }

    private void SetTargetAmount(int amount)
    {
        if (amount < 0)
        {
            amount = 0;
        }
        TargetChuzzle.Counter = amount;
        TargetChuzzle.TextMesh.text = TargetChuzzle.Counter.ToString(CultureInfo.InvariantCulture);
    }

    public override void HumanTurn()
    {
        SpendTurn();     
    }

    public override void OnReset()
    {
        Amount = TargetAmount;
        //TODO find target chuzzle
        TargetChuzzle = null;
    }

    public override string ToString()
    {
        return string.Format("You should destroy tile {0} times for {1} turns", TargetAmount, Turns);
    }
}