#region

using System;
using UnityEngine;

#endregion

public class GameModeToString : MonoBehaviour
{
    public static Phrase PlaceString = new Phrase("Вам нужно очистить грязь за {0} ходов", "GameMode_Place");
    public static Phrase ScoreString = new Phrase("Вам нужно набрать {0} очков за {1} ходов", "GameMode_Score"); 
    public static Phrase ChuzzleString = new Phrase("Вам нужно уничтожить конфетку {0} раз за {1} ходов",
        "GameMode_Chuzzle");

    public static string GetString(GameMode gameMode)
    {
        if (gameMode is TargetChuzzleGameMode)
        {
            return LocalizationStrings.GetString(ChuzzleString, (gameMode as TargetChuzzleGameMode).Amount,
                gameMode.Turns);
        }
        if (gameMode is TargetPlaceGameMode)
        {
            return LocalizationStrings.GetString(PlaceString,gameMode.Turns);
        }
        if (gameMode is TargetScoreGameMode)
        {
            return LocalizationStrings.GetString(ScoreString, (gameMode as TargetScoreGameMode).TargetScore,
                gameMode.Turns);
        }
        throw new NotImplementedException("Unknown game mode");
    }
}