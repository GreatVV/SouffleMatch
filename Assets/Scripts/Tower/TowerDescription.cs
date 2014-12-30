using System;
using System.Collections.Generic;
using System.Linq;
using Tower.Floors;

namespace Tower
{
    [Serializable]
    public class TowerDescription
    {
        public float WinPoinstCoeffiecientWithOutFloor = 2;
        public float WinPoinstCoeffiecientWithFloor = 1;
        

        public ushort Width;
        public ushort Height;
        public ushort Turns;
        public ushort PointsPerTile;

        private List<IFloorDesc> _floors = new List<IFloorDesc>();
        public bool IsCoinsDoubled;
        public float WinPointsСoefficient;

        public void AddFloor(IFloorDesc floor)
        {
            _floors.Add(floor);
        }

        public void Calculate()
        {
            Width = (ushort)_floors.Count(x => x is WidthFloorDesc);
            Height = (ushort)_floors.Count(x => x is HeightFloorDesc);
            Turns = (ushort) _floors.Where(x=>x is TurnsFloorDesc).Cast<TurnsFloorDesc>().Sum(y =>y.AddTurns );
            PointsPerTile = (ushort) _floors.Where(x=>x is PointsPerTileFloorDesc).Cast<PointsPerTileFloorDesc>().Sum(y =>y.PointsPerTile );
            IsCoinsDoubled = _floors.Any(x => x is DoubleCoinsFloorDesc);
            WinPointsСoefficient = _floors.Any(x => x is DecreaseWinPointsFloorDesc)
                                       ? WinPoinstCoeffiecientWithFloor
                                       : WinPoinstCoeffiecientWithOutFloor;
        }
    }
}