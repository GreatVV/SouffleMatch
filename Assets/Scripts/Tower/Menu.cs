using Assets.Game;
using Assets.Game.Levels;
using UnityEngine;

namespace Assets.Tower
{
    public class Menu : MonoBehaviour
    {
        public LevelManager LevelManager;
        public Tower tower;

        public void OnPlayClick()
        {
            var towerDesc = tower.GetTowerDescription();
            var level = LevelManager.GetLevel(towerDesc);

        }

    }
}


