using System;

namespace Assets.Tower
{
    [Serializable]
    public class FloorDesc
    {
        public VisualFloorDesc Visual;
        public GameFloorDesc GameLogic;
    }
}