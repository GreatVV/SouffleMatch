using System.Linq;
using UnityEngine;

namespace Tower.Floors
{
    public interface IFloorDesc
    {
    }

    public abstract class FloorDescBuilder<T> where T:IFloorDesc, new()
    {
        [SerializeField]
        private int _maxAmount = 3;

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
            return tower.Floors.Count(x => x.FloorDescription is T) < MaxAmount;
        }

        public T Create()
        {
            return new T();
        }
    }
}