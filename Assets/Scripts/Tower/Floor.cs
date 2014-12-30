using System;
using System.Reflection;
using Tower.Floors;
using UnityEngine;
using UnityEngine.UI;

namespace Tower
{
    public class Floor : MonoBehaviour
    {
        [SerializeField]
        private string floorDescName;

        private IFloorDesc floorDescription;

        [SerializeField]
        private Text text;

        public IFloorDesc FloorDescription
        {
            get
            {
                if (floorDescription == null && !string.IsNullOrEmpty(floorDescName))
                {
                    Type type = Assembly.GetAssembly(typeof(IFloorDesc)).GetType(floorDescName);
                    floorDescription = Activator.CreateInstance(type) as IFloorDesc;
                }
                return floorDescription;
            }
        }

        public void Init(IFloorDesc floorDesc)
        {
            floorDescription = floorDesc;
            floorDescName = floorDescription.GetType().FullName;
            if (text)
            {
                text.text = floorDesc.GetType().Name;
            }
        }
    }
}