#region

using System;
using System.Collections.Generic;
using System.Linq;
using Game.GameMode;
using Game.Gameplay.Conditions;
using Game.Utility;

#endregion

namespace Game.Data
{
    [Serializable]
    public class LevelDescription
    {
        public ConditionDescription Condition = new ConditionDescription();
        public FieldDescription Field = new FieldDescription();
        public string Name;
        public int PointsPerTile = 10;
        public bool IsCoinsDoubled;

        #region Event Handlers

        public LevelDescription()
        {
            Name = "New level";
        }

        #endregion

        #region Events Subscribers

        public int NumberOfStarForScore(int score)
        {
            return score <= Condition.Star2Score ? 1 : (score <= Condition.Star3Score ? 2 : 3);
        }

        #endregion
        
        public override string ToString()
        {
            return string.Format("Name: {0} Width:{1} Height: {2}", Name, Field.Width, Field.Height);
        }
    }
}