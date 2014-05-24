using UnityEngine;

namespace Game
{
    [RequireComponent(typeof (SpriteRenderer))]
    public class RandomSprite : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;

        #region In Inspector

        public bool HorizontalFlip = true;
        public Sprite[] Sprites;
        public bool VerticalFlip = true;

        #endregion

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();

            if (Sprites == null || Sprites.Length == 0)
            {
                Debug.LogWarning("Sprites to chose from is not set: " + gameObject);
                return;
            }
            Sprite randomSprite = Sprites[Random.Range(0, Sprites.Length)];
            if (HorizontalFlip)
            {
                bool needFlip = Random.Range(0, 100) > 50;
                if (needFlip)
                {
                    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y,
                        transform.localScale.z);
                }
            }

            if (VerticalFlip)
            {
                bool needFlip = Random.Range(0, 100) > 50;
                if (needFlip)
                {
                    transform.localScale = new Vector3(transform.localScale.x, -transform.localScale.y,
                        transform.localScale.z);
                }
            }
            _spriteRenderer.sprite = randomSprite;
        }
    }
}