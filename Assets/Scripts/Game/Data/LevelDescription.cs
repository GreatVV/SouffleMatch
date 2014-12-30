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

        private static List<Stage> CreateStagesFromJsonObject(JSONObject stagesJsonObject)
        {
            if (stagesJsonObject == null || stagesJsonObject.list == null ||
                stagesJsonObject.list.First().type == JSONObject.Type.NULL)
            {
                return null;
            }

            var stages = new List<Stage>();
            foreach (JSONObject jsonObject in stagesJsonObject.list)
            {
                /*   if (jsonObject.type == JSONObject.Type.NULL)
            {
                return null;
            }*/
                var stage = new Stage
                {
                    Id = (int) jsonObject.GetField("Id").n,
                    MinY = (int) jsonObject.GetField("MinY").n,
                    MaxY = (int) jsonObject.GetField("MaxY").n,
                    NextStage = (int) jsonObject.GetField("NextStage").n,
                    WinOnComplete = jsonObject.GetField("WinOnComplete").b,
                    Condition = new Condition
                    {
                        IsScore = jsonObject.GetField("Condition").GetField("IsScore").b,
                        Target = (int) jsonObject.GetField("Condition").GetField("Target").n
                    }
                };
                stages.Add(stage);
            }
            return stages;
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