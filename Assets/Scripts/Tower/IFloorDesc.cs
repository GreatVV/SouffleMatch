using System;

namespace Tower
{
    public interface IFloorDesc
    {
    }

    public class SimpleFloorDesc : IFloorDesc
    {
        
    }

    [Serializable]
    public class WidthFloorDesc : IFloorDesc
    {
        
    }

    [Serializable]
    public class HeightFloorDesc : IFloorDesc
    {
        
    }

    [Serializable]
    public class TurnsFloorDesc : IFloorDesc
    {
        public ushort AddTurns;
    }

    [Serializable]
    public class PointsPerTileFloorDesc : IFloorDesc
    {
        public ushort PointsPerTile;
    }

    [Serializable]
    public class DoubleCoinsFloorDesc : IFloorDesc
    {
        
    }

    [Serializable]
    public class DecreaseWinPointsFloorDesc : IFloorDesc
    {
        
    }
}