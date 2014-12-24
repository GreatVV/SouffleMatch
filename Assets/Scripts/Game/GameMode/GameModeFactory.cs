using System;

namespace Game.GameMode
{
    public class GameModeFactory
    {
        public static GameMode CreateGameMode(GameModeDescription description)
        {
            switch (description.Mode)
            {
                case (GameModes.TargetScore):
                    return new TargetScoreGameMode(description);
                case (GameModes.TargetPlace):
                    return new TargetPlaceGameMode(description);
                case (GameModes.TargetChuzzle):
                    return new TargetChuzzleGameMode(description);
                default:
                    throw new ArgumentOutOfRangeException("Not correct gammode" + description.Mode);
            }
        }
    }
}