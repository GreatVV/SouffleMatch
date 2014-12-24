using System;
using UnityEngine;

namespace UI
{
    public class BackgroundChanger : MonoBehaviour
    {
        [SerializeField] private Sprite DaySprite;
        [SerializeField] private Sprite NightSprite;
        [SerializeField] private SpriteRenderer spriteRenderer;

        // Use this for initialization

        #region Unity Methods

        private void Start()
        {
            if (DateTime.Now.Hour > 6 && DateTime.Now.Hour < 18)
            {
                spriteRenderer.sprite = DaySprite;
            }
            else
            {
                spriteRenderer.sprite = NightSprite;
            }
        }

        #endregion
    }
}