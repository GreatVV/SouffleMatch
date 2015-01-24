using UnityEngine;
using UnityEngine.UI;

namespace Tower
{
    public enum FloorType
    {
        First,
        Width,
        Height,
        DecreaseWinPoints,
        DoubleCoins,
        AddTurns,
        PointerPerTile
    }

    public class Floor : MonoBehaviour
    {
        [SerializeField]
        public FloorType FloorType;

        [SerializeField]
        private Text text;

        public void Init(FloorType floorType)
        {
            FloorType = floorType;
            if (text)
            {
                text.text = floorType.ToString();
            }
        }
    }
}