using System;
using System.Linq;
using UnityEngine;

namespace Tower
{
    [Serializable]
    public class FloorDescBuilder
    {
        [SerializeField]
        private FloorType _floorType;

        [SerializeField]
        private int _maxAmount = 3;

        public FloorDescBuilder(FloorType floorType)
        {
            _floorType = floorType;
        }

        public virtual int MaxAmount
        {
            get
            {
                return _maxAmount;
            }
            set
            {
                _maxAmount = value;
            }
        }

        public bool CanCreateInTower(Tower tower)
        {
            return tower.Floors.Count(x => x.FloorType == _floorType) < MaxAmount;
        }
    }
}