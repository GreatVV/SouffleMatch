using System;
using Assets.Game.GameMode;
using Assets.Tower;

namespace Assets.Game.Data
{
    [Serializable]
    public class LevelFactory
    {
        public ushort MinFieldWidth = 6;
        public ushort MinFieldHeight = 6;
        public ushort MinTurns = 20;
        public ushort MinPointsPerTile;
        public int AverageTileCoefficient = 5;

        public LevelDescription Create(int width, int height)
        {
            if (width < MinFieldWidth)
            {
                width = MinFieldWidth;
            }

            if (height < MinFieldHeight)
            {
                height = MinFieldHeight;
            }

            var levelDescription = new LevelDescription
                                   {
                                       Field = new FieldDescription()
                                               {
                                                   Width = width,
                                                   Height = height,
                                               }
                                   };

            return levelDescription;
        }

        public LevelDescription Create(TowerDescription towerDescription)
        {
            var width = towerDescription.Width + MinFieldWidth;
            var height = towerDescription.Height + MinFieldHeight;
            var turns = towerDescription.Turns + MinTurns;
            var isCoinsDoubled = towerDescription.IsCoinsDoubled;
            var pointsPerTile = MinPointsPerTile + towerDescription.PointsPerTile;
            var targetScore = CalculateTargetScore(
                                                     width,
                                                     height,
                                                     turns,
                                                     towerDescription.WinPointsÑoefficient,
                                                     AverageTileCoefficient);

            var desc =  Create(width, height);
            desc.IsCoinsDoubled = isCoinsDoubled;
            desc.PointsPerTile = pointsPerTile;
            desc.Condition.GameMode = new GameModeDescription
                                      {
                                          Turns = turns,
                                          TargetScore = targetScore
                                      };
            return desc;
        }

        public static int CalculateTargetScore(int width, int height, int turns, float winPointsCoefficient, int averageTileCoefficient)
        {
            return (int) (width * height * turns * winPointsCoefficient * averageTileCoefficient);
        }
    }
}