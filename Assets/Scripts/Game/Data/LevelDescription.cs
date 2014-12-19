#region

using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Game.GameMode;
using Assets.Game.Gameplay.Conditions;
using Assets.Game.Utility;

#endregion

namespace Assets.Game.Data
{
    [Serializable]
    public class LevelDescription
    {
        public ConditionDescription Condition = new ConditionDescription();
        public FieldDescription Field = new FieldDescription();
        public string Name;

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

        public JSONObject Serialize()
        {
            var json = new JSONObject();
            json.AddField("name", Name);
            json.AddField("width", Field.Width);
            json.AddField("height", Field.Height);
            json.AddField("numberOfColors", Field.NumberOfColors);
            json.AddField("seed", Field.Seed);
            json.AddField("gameMode",Condition.GameMode.Serialize());

            json.AddField("Star1Score", Condition.Star1Score);
            json.AddField("Star2Score", Condition.Star2Score);
            json.AddField("Star3Score", Condition.Star3Score);

            var map = new JSONObject(JSONObject.Type.ARRAY);
            foreach (var cellDescription in Field.SpecialCells)
            {
                JSONObject tile = cellDescription.Serialize();
                map.Add(tile);
            }
            json.AddField("map",map);
            return json;
        }

        public static LevelDescription FromJson(JSONObject jsonObject)
        {
           // Debug.Log("Print: \n" + jsonObject);
            var serializedLevel = new LevelDescription();
            serializedLevel.Name = jsonObject.GetField("name").str;

            serializedLevel.Field.Width = (int) jsonObject.GetField("width").n;
            serializedLevel.Field.Height = (int) jsonObject.GetField("height").n;
            serializedLevel.Field.NumberOfColors = jsonObject.HasField("numberOfColors")
                ? (int) jsonObject.GetField("numberOfColors").n
                : 6;
            serializedLevel.Field.Seed = jsonObject.HasField("seed") ? (int) jsonObject.GetField("seed").n : -1;


            serializedLevel.Condition.GameMode = GameModeDescription.CreateFromJson(jsonObject.GetField("gameMode"));

            if (jsonObject.HasField("map"))
            {
                List<JSONObject> array = jsonObject.GetField("map").list;
                try
                {
                    foreach (JSONObject tile in array)
                    {
                        serializedLevel.Field.SpecialCells.Add(CellDescription.Deserialize(tile));
                    }
                }
                catch (Exception)
                {
                }
                
            }

            if (jsonObject.HasField("stages"))
            {
                serializedLevel.Field.Stages = CreateStagesFromJsonObject(jsonObject.GetField("stages"));
            }

            serializedLevel.Condition.Star1Score = jsonObject.HasField("Star1Score")
                ? (int) jsonObject.GetField("Star1Score").n
                : 1000;
            serializedLevel.Condition.Star2Score = jsonObject.HasField("Star2Score")
                ? (int) jsonObject.GetField("Star2Score").n 
                : 2000;
            serializedLevel.Condition.Star3Score = jsonObject.HasField("Star3Score")
                ? (int) jsonObject.GetField("Star3Score").n
                : 3000;

            return serializedLevel;
        }


        public override string ToString()
        {
            return string.Format("Name: {0} Width:{1} Height: {2}", Name, Field.Width, Field.Height);
        }
    }
}