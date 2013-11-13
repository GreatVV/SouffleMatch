#region

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#endregion

[Serializable]
public class SerializedLevel
{
    public GameModeDescription GameMode;
    public int Height;
    public string Name;

    public int NumberOfColors = -1;
    public int Seed;

    public List<Cell> SpecialCells = new List<Cell>();
    public List<Stage> Stages = new List<Stage>();

    public int Star1Score;
    public int Star2Score;
    public int Star3Score;
    public int Width;

    #region Event Handlers

    private static List<Stage> CreateStagesFromJsonObject(JSONObject stagesJsonObject)
    {
        if (stagesJsonObject == null || stagesJsonObject.list == null ||
            stagesJsonObject.list.First().type == JSONObject.Type.NULL)
        {
            return null;
        }

        var stages = new List<Stage>();
        foreach (var jsonObject in stagesJsonObject.list)
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

    public static SerializedLevel FromJson(JSONObject jsonObject)
    {
        Debug.Log("Print: \n" + jsonObject);
        var serializedLevel = new SerializedLevel();
        serializedLevel.Name = jsonObject.GetField("name").str;

        serializedLevel.Width = (int) jsonObject.GetField("width").n;
        serializedLevel.Height = (int) jsonObject.GetField("height").n;
        serializedLevel.NumberOfColors = jsonObject.HasField("numberOfColors")
            ? (int) jsonObject.GetField("numberOfColors").n
            : 6;
        serializedLevel.Seed = jsonObject.HasField("seed") ? (int) jsonObject.GetField("seed").n : 1;

        serializedLevel.GameMode = GameModeDescription.CreateFromJson(jsonObject.GetField("gameMode"));

        var array = jsonObject.GetField("map").list;
        foreach (var tile in array)
        {
            var x = array.IndexOf(tile)%serializedLevel.Width;
            var y = serializedLevel.Height - (array.IndexOf(tile)/serializedLevel.Width) - 1;

            var tileType = (int) tile.n;
            switch (tileType)
            {
                case (0): //empty
                    break;
                case (2): // place
                    serializedLevel.SpecialCells.Add(new Cell(x, y) {HasPlace = true});
                    break;
                case (3): //counter
                    serializedLevel.SpecialCells.Add(new Cell(x, y) {HasCounter = true});
                    break;
                default: // block
                    serializedLevel.SpecialCells.Add(new Cell(x, y, CellTypes.Block));
                    break;
            }
        }

        serializedLevel.Stages = CreateStagesFromJsonObject(jsonObject.GetField("stages"));

        serializedLevel.Star1Score = jsonObject.HasField("Star1Score")
            ? (int) jsonObject.GetField("Star1Score").n
            : 1000;
        serializedLevel.Star2Score = jsonObject.HasField("Star2Score")
            ? (int) jsonObject.GetField("Star2Score").n
            : 2000;
        serializedLevel.Star3Score = jsonObject.HasField("Star3Score")
            ? (int) jsonObject.GetField("Star3Score").n
            : 3000;

        return serializedLevel;
    }


    public override string ToString()
    {
        return string.Format("Name: {0} Width:{1} Height: {2}", Name, Width, Height);
    }

    public int NumberOfStarForScore(int score)
    {
        return score <= Star2Score ? 1 : (score <= Star3Score ? 2 : 3);
    }

}