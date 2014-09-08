using System;
using System.Collections.Generic;
using Game;
using Game.Data;
using UnityEngine;
using UnityEngine.UI;

public class GuiLevelList : Window
{
    public LevelManager levelManager;

    public MapId mapIdPrefab;

    public int tilesInRow = 5;

    public Animator animator;

    public Dictionary<string, MapId> mapItems = new Dictionary<string, MapId>();

    protected override void OnAwake()
    {
        levelManager.LevelsAreReady += OnLevelsAreReady;
    }

    private void OnDestroy()
    {
        levelManager.LevelsAreReady -= OnLevelsAreReady;
    }

    private void OnLevelsAreReady()
    {
        if (!mapIdPrefab)
        {
            return;
        }

        var screenCenter = new Vector3(Screen.width/2f, Screen.height/2f, 0);
        List<LevelPack> packs = levelManager.LevelPackManager.Packs;
        for (int packIndex = 0; packIndex < packs.Count; packIndex++)
        {
            LevelPack levelPack = packs[packIndex];
            for (int levelIndex = 0; levelIndex < levelPack.LoadedLevels.Count; levelIndex++)
            {
                LevelDescription serializedLevel = levelManager.LevelPackManager.Packs[0].LoadedLevels[levelIndex];
                var x = levelIndex % tilesInRow;
                var y = levelIndex/tilesInRow;
                var mapId = ((GameObject) Instantiate(mapIdPrefab.gameObject)).GetComponent<MapId>();
                mapId.transform.parent = transform;
                //   mapId.transform.parent = grid.transform;
                mapId.transform.localScale = Vector3.one;
                Vector2 size;
              //  Debug.Log("screen: "+Screen.width + ": "+Screen.height);
                size = (Screen.width < Screen.height ? new Vector2(Screen.width/tilesInRow, Screen.width/tilesInRow) : new Vector2(Screen.height / tilesInRow, Screen.height / tilesInRow))*0.8f;

                var columns = levelPack.LoadedLevels.Count/tilesInRow;
                var totalHeight = columns*size.y;
                var totalWidth = tilesInRow*size.y;
                var totalSize = new Vector2(-totalWidth, totalHeight);
                var offset = screenCenter + (Vector3)totalSize / 2f + new Vector3(size.x / 2f, -size.y/2f);

                var rectTransform = mapId.GetComponent<RectTransform>();
                rectTransform.sizeDelta = size;
                rectTransform.position = new Vector3(x*size.x, -y*size.y, 0) + offset ;
                mapId.name = string.Format("{0}", Convert.ToInt32(serializedLevel.Name));
                mapId.Pack = packIndex;
                mapId.Index = levelIndex;

                mapId.UpdateName();
                mapItems[serializedLevel.Name] = mapId;

                mapId.GetComponent<Button>().onClick.AddListener(OnClick);
            }
        }
    }

    private void OnClick()
    {
        gameObject.SetActive(false);
        animator.SetTrigger("Back");
    }

    private void OnEnable()
    {
        //  grid.Reposition();

        // var size = NGUIMath.CalculateRelativeWidgetBounds(grid.transform);
        //((BoxCollider) mapIdPrefab.collider).size;
        //var rows = grid.children.Count / grid.columns;

        //Debug.Log("Box collider: " + size + "grid.columns: " + grid.columns + " scale: " + grid.transform.localScale);
        //   transform.localPosition = new Vector3(-size.extents.x, size.extents.y, grid.transform.localPosition.z);
        //Debug.Log("Transform: " + grid.transform.localPosition);


        if (levelManager.IsLoaded)
        {
            LevelDescription level = levelManager.LevelPackManager.Packs[0][SessionRestorer.Instance.lastPlayedLevel];
            OnLevelsAreReady();
            mapItems[level.Name].ShowCurrent();
        }
    }

    protected override void OnHideWindow()
    {
    }

    protected override void OnShowWindow()
    {
        base.OnShowWindow();
    }
}