using Game.Data;
using Game.Player;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace UI.MainMenu
{
    public class PlayButton : MonoBehaviour, IPointerClickHandler 
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            var towerDesc = FindObjectOfType<Tower.Tower>().GetTowerDescription();
            var levelDescription = Instance.LevelFactory.Create(towerDesc);
            SessionRestorer.CurrentLevel = levelDescription;

            Application.LoadLevel(ScenesName.Game);
        }
    }
}