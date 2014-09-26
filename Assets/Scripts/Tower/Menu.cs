using Game;
using UnityEngine;
using System.Collections;
using Tower;

public class Menu : MonoBehaviour
{
    public LevelManager LevelManager;
    public Tower.Tower tower;

    public void OnPlayClick()
    {
        var towerDesc = tower.GetTowerDescription();
        var level = LevelManager.GetLevel(towerDesc);

    }

}


