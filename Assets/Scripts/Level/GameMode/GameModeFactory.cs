using System;
using UnityEngine;

[Serializable]
public class GameModeDescription
{
    public string Mode; // TargetScore | TargetPlace | TargetChuzzle

    public int Turns;
    public int TargetScore;
    public int Amount;


    public static GameModeDescription CreateFromJson(JSONObject jsonObject)
    {
        if (jsonObject == null)
        {
            Debug.LogWarning("There is no gameMode");
            return new GameModeDescription()
            {
                Mode = "TargetScore",
                TargetScore = 3000,
                Turns = 40
            };
            /*return new GameModeDescription()
            {
                Mode = "TargetChuzzle",
                Turns = 30,
                Amount = 20
            };*/
            /*return new GameModeDescription()
            {
                Mode = "TargetPlace",
                Turns = 30
            };*/
        }


        var desc = new GameModeDescription
        {
            Mode = jsonObject.GetField("Mode").str,
            Turns = (int) jsonObject.GetField("Turns").n,
            TargetScore = jsonObject.HasField("TargetScore") ? (int) jsonObject.GetField("TargetScore").n : 0,
            Amount = jsonObject.HasField("Amount") ? (int) jsonObject.GetField("Amount").n : 0
        };
        return desc;
    }
}

public class GameModeFactory
{
    public static GameMode CreateGameMode(GameModeDescription description)
    {
        switch (description.Mode)
        {
            case ("TargetScore"):
                return new TargetScoreGameMode(description);
            case ("TargetPlace"):
                return new TargetPlaceGameMode(description);
            case ("TargetChuzzle"):
                return new TargetChuzzleGameMode(description);
            default:
                throw new ArgumentOutOfRangeException("Not correct gammode" + description.Mode);
        }
    }

}

