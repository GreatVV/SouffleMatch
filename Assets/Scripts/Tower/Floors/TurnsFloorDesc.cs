using System;

namespace Tower.Floors
{
    [Serializable]
    public class TurnsFloorDesc : IFloorDesc
    {
        public ushort AddTurns = 5;
    }

    [Serializable]
    public class TurnsFloorDescBuilder : FloorDescBuilder<TurnsFloorDesc>
    {
        
    }
}