using System;
using UnityEngine;

namespace Tower
{
    [Serializable]
    public class FloorDesc
    {
        public VisualFloorDesc Visual;
        public GameFloorDesc GameLogic;
    }
}