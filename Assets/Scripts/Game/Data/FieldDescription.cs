using System;
using System.Collections.Generic;
using Assets.Game.Gameplay.Conditions;

namespace Assets.Game.Data
{
    [Serializable]
    public class FieldDescription
    {
        public int Height;
        public int NumberOfColors = -1;
        public int Seed;
        public List<CellDescription> SpecialCells = new List<CellDescription>();
        public List<Stage> Stages = new List<Stage>();
        public int Width;
    }
}