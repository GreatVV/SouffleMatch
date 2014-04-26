using System;
using System.Linq;
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
            mapId.name = string.Format("{0:00}", Convert.ToInt32(serializedLevel.Name));
            mapId.UpdateName();
        }
    }

    void OnEnable()
    {
        grid.Reposition();

        var size = ((BoxCollider) mapIdPrefab.collider).size;
        var rows = grid.children.Count / grid.columns;

        Debug.Log("Box collider: " + size + "grid.columns: " + grid.columns + " scale: " + grid.transform.localScale);
        transform.localPosition = new Vector3(-size.x * grid.columns * grid.transform.localScale.x * 0.5f, 0.5f * size.y * rows * grid.transform.localScale.y, grid.transform.localPosition.z);
        Debug.Log("Transform: " + grid.transform.localPosition);
    }
}
