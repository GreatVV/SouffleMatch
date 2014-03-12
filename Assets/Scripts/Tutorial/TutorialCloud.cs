using UnityEngine;

namespace TutorialSpace
{
    internal class TutorialCloud : MonoBehaviour
    {
        [SerializeField]
        private UILabel label;

        [SerializeField] private Camera currentCamera;


        public void SetText(string text)
        {
            label.text = text;
        }

        public void SetPosition(Vector3 positionInScreenCoordinate)
        {
            transform.position = currentCamera.ScreenToWorldPoint(positionInScreenCoordinate);
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