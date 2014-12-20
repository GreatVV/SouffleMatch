using Assets.Game.Data;
using Assets.Game.Utility;
using UnityEngine;

namespace Assets.Tower
{
    public class Floor : MonoBehaviour, IJsonSerializable
    {
        [SerializeField]
        private FloorDesc floorDescription;

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
            var spriteGO = new GameObject("Sprite", typeof(SpriteRenderer));
            spriteGO.transform.SetParent(transform, false);
            var sprite = spriteGO.GetComponent<SpriteRenderer>();
            sprite.sprite = floorDescription.Visual.sprite;
        }
    }
}
