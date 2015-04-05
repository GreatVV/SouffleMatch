using System;
using Game.GameMode;
using Tower;
using UnityEngine;

namespace Game.Data
{
    [Serializable]
    public class LevelFactory : ScriptableObject
    {
        public int MinFieldWidth = 6;
        public int MinFieldHeight = 6;
        public int MinTurns = 20;
        public int MinPointsPerTile;
        public int AverageTileCoefficient = 5;
        public LevelDescription CurrentLevel;

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

        public LevelDescription Create(TowerDescription description)
        {
            var width = description.Width + MinFieldWidth;
            var height = description.Height + MinFieldHeight;
            var turns = description.Turns + MinTurns;
            var isCoinsDoubled = description.IsCoinsDoubled;
            var pointsPerTile = MinPointsPerTile + description.PointsPerTile;
            var targetScore = CalculateTargetScore(
                                                   width,
                                                   height,
                                                   turns,
                                                   description.WinCoefficent,
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