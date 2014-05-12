using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GuiLevelList : Window
{
    public LevelManager levelManager;

    public MapId mapIdPrefab;

    public UITable grid;

    public Dictionary<string, MapId> mapItems = new Dictionary<string, MapId>();

    public TweenPosition pauseButtonTween;

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
            mapItems[serializedLevel.Name] = mapId;
        }
    }

    void OnEnable()
    {
        grid.Reposition();

        var size = NGUIMath.CalculateRelativeWidgetBounds(grid.transform);
            //((BoxCollider) mapIdPrefab.collider).size;
        //var rows = grid.children.Count / grid.columns;

        //Debug.Log("Box collider: " + size + "grid.columns: " + grid.columns + " scale: " + grid.transform.localScale);
        transform.localPosition = new Vector3(-size.extents.x, size.extents.y, grid.transform.localPosition.z);
        //Debug.Log("Transform: " + grid.transform.localPosition);


        if (levelManager.IsLoaded)
        {
            var level = levelManager[SessionRestorer.Instance.lastPlayedLevel];
            mapItems[level.Name].ShowCurrent();
        }
    }

    protected override void OnHideWindow()
    {
        pauseButtonTween.PlayForward();
    }

    protected override void OnShowWindow()
    {
        base.OnShowWindow();
        pauseButtonTween.PlayReverse();
    }
}
