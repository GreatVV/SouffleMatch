#region

using System;
using System.Collections.Generic;
using System.Linq;
using Game.GameMode;
using UnityEngine;

#endregion

namespace Game.Data
{
    [Serializable]
    public class LevelDescription
    {
        public ConditionDescription Condition = new ConditionDescription();
        public FieldDescription Field = new FieldDescription();
        public string Name;

        #region Event Handlers

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

        public static LevelDescription FromJson(JSONObject jsonObject)
        {
            Debug.Log("Print: \n" + jsonObject);
            var serializedLevel = new LevelDescription();
            serializedLevel.Name = jsonObject.GetField("name").str;

            serializedLevel.Field.Width = (int) jsonObject.GetField("width").n;
            serializedLevel.Field.Height = (int) jsonObject.GetField("height").n;
            serializedLevel.Field.NumberOfColors = jsonObject.HasField("numberOfColors")
                ? (int) jsonObject.GetField("numberOfColors").n
                : 6;
            serializedLevel.Field.Seed = jsonObject.HasField("seed") ? (int) jsonObject.GetField("seed").n : -1;


            serializedLevel.Condition.GameMode = GameModeDescription.CreateFromJson(jsonObject.GetField("gameMode"));

            List<JSONObject> array = jsonObject.GetField("map").list;
            foreach (JSONObject tile in array)
            {
                int x = array.IndexOf(tile)%serializedLevel.Field.Width;
                int y = serializedLevel.Field.Height - (array.IndexOf(tile)/serializedLevel.Field.Width) - 1;

                var tileType = (int) tile.n;
                switch (tileType)
                {
                    case (0): //empty
                        break;
                    case (2): // place
                        serializedLevel.Field.SpecialCells.Add(new Cell(x, y) {CreationType = CreationType.Place});
                        break;
                    case (3): //counter
                        serializedLevel.Field.SpecialCells.Add(new Cell(x, y) {CreationType = CreationType.Counter});
                        break;
                    case (4): //lock
                        serializedLevel.Field.SpecialCells.Add(new Cell(x, y) {CreationType = CreationType.Lock});
                        break;
                    case (5): //two time
                        serializedLevel.Field.SpecialCells.Add(new Cell(x, y) {CreationType = CreationType.TwoTimes});
                        break;
                    case (6): //invader
                        serializedLevel.Field.SpecialCells.Add(new Cell(x, y) {CreationType = CreationType.Invader});
                        break;
                    default: // block
                        serializedLevel.Field.SpecialCells.Add(new Cell(x, y, CellTypes.Block));
                        break;
                }
            }

            serializedLevel.Field.Stages = CreateStagesFromJsonObject(jsonObject.GetField("stages"));

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

    [Serializable]
    public class ConditionDescription
    {
        public GameModeDescription GameMode;
        public int Star1Score;
        public int Star2Score;
        public int Star3Score;
    }

    [Serializable]
    public class FieldDescription
    {
        public int Height;
        public int NumberOfColors = -1;
        public int Seed;
        public List<Cell> SpecialCells = new List<Cell>();
        public List<Stage> Stages = new List<Stage>();
        public int Width;
    }
}