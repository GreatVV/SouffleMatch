
using System;

namespace Game.GameMode
{
    [Serializable]
    public class TargetScoreGameMode : GameMode
    {
        public int TargetScore;

        public override int TargetPoints { get { return TargetScore; }}

        public TargetScoreGameMode(GameModeDescription description) : base(description)
        {
            TargetScore = description.TargetScore;
        }                                 

        public override void OnDestroy()
        {
            ManaManagerSystem.PointChanged -= OnManaManagerChanged;
        }

        public void OnManaManagerChanged(int points, int i)
        {
            if (points >= TargetScore)
            {
                IsWin = true;
            }
        }

        protected override void OnInit()
        {
            ManaManagerSystem = Gamefield.ManaManagerSystem;
            ManaManagerSystem.PointChanged += OnManaManagerChanged;
        }

        public override void OnReset()
        {
            Turns = StartTurns;
        }

        public override void HumanTurn()
        {
            SpendTurn();
        }

        public override string ToString()
        {
            return string.Format("You should get {0} points", TargetScore);
        }
    }
}