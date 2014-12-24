using System;
using Game.GameMode;

namespace Game.Data
{
    [Serializable]
    public class ConditionDescription
    {
        public GameModeDescription GameMode = new GameModeDescription();
        public int Star1Score;
        public int Star2Score;
        public int Star3Score;
    }
}