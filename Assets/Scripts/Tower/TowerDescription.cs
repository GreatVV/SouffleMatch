using System;
using System.Collections.Generic;
using System.Linq;
using Utils;

namespace Tower
{
    [Serializable]
    public class TowerDescription
    {
        private List<FloorType> _floors = new List<FloorType>();
        public ushort Height;
        public bool IsCoinsDoubled;
        public ushort PointsPerTile;
        public ushort Turns;
        public ushort Width;
        public float WinPoinstCoeffiecientWithFloor = 1;
        public float WinPoinstCoeffiecientWithOutFloor = 2;
        public float WinPointsСoefficient;

        public void AddFloor(FloorType floor)
        {
            _floors.Add(floor);
        }

        public void Calculate()
        {
            Width = (ushort) _floors.Count(x => x == FloorType.Width);
            Height = (ushort) _floors.Count(x => x == FloorType.Height);
            Turns =
                (ushort)
                ((ushort) _floors.Count(x => x == FloorType.AddTurns) * Instance.FloorFactory.TurnsPerFloor);
            PointsPerTile =
                (ushort)
                ((ushort) _floors.Count(x => x == FloorType.PointerPerTile) * Instance.FloorFactory.PointsPerTile);
            IsCoinsDoubled = _floors.Any(x => x == FloorType.DoubleCoins);
            WinPointsСoefficient = _floors.Any(x => x == FloorType.DecreaseWinPoints)
                                       ? WinPoinstCoeffiecientWithFloor
                                       : WinPoinstCoeffiecientWithOutFloor;
        }
    }
}