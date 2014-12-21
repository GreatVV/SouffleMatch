using Assets.Game.Data;
using Assets.Game.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Tower
{
    public class Floor : MonoBehaviour, IJsonSerializable
    {
        [SerializeField]
        private FloorDesc floorDescription;

        public SpriteRenderer SpriteRenderer;

        public BoxCollider2D BoxCollider2D;

        [SerializeField]
        private Text text;

        public FloorDesc FloorDescription
        {
            get
            {
                return floorDescription;
            }
        }

        public JSONObject Serialize()
        {
            throw new System.NotImplementedException();
        }

        public void Deserialize(JSONObject json)
        {
            throw new System.NotImplementedException();
        }

        public void Init(FloorDesc floorDesc)
        {
            floorDescription = floorDesc;
            if (floorDescription.Visual.sprite)
            {
                SpriteRenderer.sprite = floorDescription.Visual.sprite;
            }
            text.text = floorDesc.ToString();
        }

        
    }
}
