using Assets.Game.Data;
using Assets.Game.Utility;
using UnityEngine;

namespace Assets.Tower
{
    public class Floor : MonoBehaviour, IJsonSerializable
    {
        [SerializeField]
        private FloorDesc floorDescription;

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
            spriteGO.transform.parent = transform;
            spriteGO.transform.localPosition = Vector3.zero;
            spriteGO.transform.localScale = Vector3.one;
            var sprite = spriteGO.GetComponent<SpriteRenderer>();
            sprite.sprite = floorDesc.Visual.sprite;
        }
    }
}
