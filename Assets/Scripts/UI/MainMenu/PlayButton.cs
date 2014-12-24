using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.MainMenu
{
    public class PlayButton : MonoBehaviour, IPointerClickHandler 
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            Application.LoadLevel(ScenesName.Game);
        }
    }
}