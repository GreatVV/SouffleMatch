using System;

namespace Tower.Floors
{
    [Serializable]
    public class PointsPerTileFloorDesc : IFloorDesc
    {
        public ushort PointsPerTile = 1;
    }

    [Serializable]
    public class PointsPerTileFloorDescBuilder : FloorDescBuilder<PointsPerTileFloorDesc>
    {
        
    }
}