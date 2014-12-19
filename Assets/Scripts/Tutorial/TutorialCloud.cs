﻿using UnityEngine;
using UnityEngine.UI;

namespace Assets.Tutorial
{
    internal class TutorialCloud : MonoBehaviour
    {
        [SerializeField]
        private Text label;

        [SerializeField] private Camera currentCamera;

        [SerializeField] private Image cloudSprite;


        public void SetText(string text)
        {
            label.text = text;
        }

        public void SetPosition(Vector3 positionInScreenCoordinate)
        {
            var width = cloudSprite.rectTransform.rect.width;
            var height = cloudSprite.rectTransform.rect.height;

            var targetPosition = currentCamera.ScreenToWorldPoint(positionInScreenCoordinate);

            if (targetPosition.x + width/2f > Screen.width)
            {
                targetPosition.x = Screen.width - width;
            }

            if (targetPosition.y + height/2f > Screen.height)
            {
                targetPosition.y = Screen.height - height;
            }

            if (targetPosition.x - width/2f < 0)
            {
                targetPosition.x = 0;
            }

            if (targetPosition.y - height/2f < 0)
            {
                targetPosition.y = 0;
            }

            transform.position = targetPosition;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }
    }
}