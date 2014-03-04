using System;
using UnityEngine;
using System.Collections;

public class GuiLevelList : MonoBehaviour
{

    public LevelManager levelManager;

    public MapId mapIdPrefab;

    public UITable grid;

    public void Awake()
    {
        levelManager.LevelsAreReady += OnLevelsAreReady;
    }

    void OnDestroy()
    {
        levelManager.LevelsAreReady -= OnLevelsAreReady;
    }

    private void OnLevelsAreReady()
    {
        foreach (var serializedLevel in levelManager.LoadedLevels)
        {
            var mapId = ((GameObject) Instantiate(mapIdPrefab.gameObject)).GetComponent<MapId>();
            mapId.transform.parent = grid.transform;
            mapId.transform.localScale = Vector3.one;
            mapId.name = serializedLevel.Name;
            mapId.UpdateName();
        }

        grid.Reposition();
    }
}
