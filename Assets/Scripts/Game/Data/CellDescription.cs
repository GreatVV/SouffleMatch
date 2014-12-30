using System;
using Game.Gameplay.Cells;
using Game.Utility;

namespace Game.Data
{
    [Serializable]
    public class CellDescription
    {
        public CreationType CreationType;
        public int X;
        public int Y;
        public CellTypes Type;
        public bool IsPlace;

        public CellDescription()
        {
            
        }

        public CellDescription(int x, int y)
        {
            X = x;
            Y = y;
        }

        public CellDescription(int x, int y, CellTypes type) : this (x,y)
        {
            Type = type;
        }

        public CellDescription(int x, int y, CellTypes type, CreationType creationType) : this(x,y,type)
        {
            CreationType = creationType;
        }
    }
}