using System;
using UnityEngine;

public class GuiLevelList : Window
{
    public LevelManager levelManager;

    public MapId mapIdPrefab;

    public UITable grid;

    protected override void OnAwake()
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
            mapId.name = string.Format("{00}", Convert.ToInt32(serializedLevel.Name));
            mapId.UpdateName();
        }

        grid.Reposition();
    }
}
