using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Game.GameMode
{
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

        private void OnCombinationDestroyed(List<Chuzzle> destroyedChuzzles)
        {   
            if (destroyedChuzzles.Contains(TargetChuzzle))
            {
                Amount -= destroyedChuzzles.Count()-1;
                SetTargetAmount(Amount);                                                               
            }
        }

        public void UpdateCounter()
        {
            TargetChuzzle = Gamefield.Level.Chuzzles.GetTile(x => x is CounterChuzzle) as CounterChuzzle;
            TargetChuzzle.Died += OnTargetChuzzleDeath;
            SetTargetAmount(Amount);
        }

        private void OnTargetChuzzleDeath(Chuzzle obj)
        {
            IsWin = true;
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
}