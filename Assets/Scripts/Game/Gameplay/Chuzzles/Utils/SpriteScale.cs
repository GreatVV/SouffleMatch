using Plugins;
using UnityEngine;

namespace Game.Gameplay.Chuzzles.Utils
{
    [ExecuteInEditMode]
    public class SpriteScale : MonoBehaviour
    {

        public Vector2 Scale;

        public void Start()
        {
            Zoom();
        }

        public void Update()
        {

        }

        private void Zoom()
        {
            var spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            //transform.localScale = new Vector3(Scale.x / spriteRenderer.sprite.bounds.size.x * Mathf.Sign(transform.localScale.x), Scale.y / spriteRenderer.sprite.bounds.size.y * Mathf.Sign(transform.localScale.y), 1);
            var target = new Vector3(Scale.x / spriteRenderer.sprite.bounds.size.x * Mathf.Sign(transform.localScale.x), Scale.y / spriteRenderer.sprite.bounds.size.y * Mathf.Sign(transform.localScale.y), 1);
            iTween.ScaleTo(gameObject, target, 0.5f);

            if (GetComponent<BoxCollider2D>())
            {
                GetComponent<BoxCollider2D>().size = new Vector2(Mathf.Abs(Scale.x / target.x), Mathf.Abs(Scale.y / target.y));
            }
           
        }
    }
}
