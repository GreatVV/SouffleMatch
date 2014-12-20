using System;

namespace Assets.Tower
{
    [Serializable]
    public class FloorDesc
    {
        public VisualFloorDesc Visual;
        public GameFloorDesc GameLogic;
    }

    public class WidthFloorDesc : FloorDesc
    {
        
    }

    public class HeightFloorDesc : FloorDesc
    {
        
    }

    public class TurnsFloorDesc : FloorDesc
    {
        public ushort AddTurns;
    }

    public class PointsPerTileFloorDesc : FloorDesc
    {
        public ushort PointsPerTile;
    }

    public class DoubleCoinsFloorDesc : FloorDesc
    {
        
    }

    public class DecreaseWinPointsFloorDesc : FloorDesc
    {
        
    }
}