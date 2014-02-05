#region

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#endregion

public class MainMenu : Window
{
    public GameObject Grid;
    public GameObject LevelLabelPrefab;
    public string LevelUrl = "https://www.dropbox.com/s/h2ejykp67vdb784/loadzerbalance.txt?dl=1";

    public TextAsset Levels;
    public List<SerializedLevel> LoadedLevels;
    public UILabel Loading;
    public JSONObject levels;

    public bool IsLevelsLoaded;
    public static MainMenu Instance { get; private set; }

    public static event Action<bool> LevelsLoaded;

    protected virtual void InvokeLevelsLoaded(bool isSuccessful)
    {
        IsLevelsLoaded = true;
        var handler = LevelsLoaded;
        if (handler != null) handler(isSuccessful);
    }

    #region Event Handlers

    private void OnEnable()
    {
        IsLevelsLoaded = false;
        return;
#if !UNITY_ANDROID
        Loading.text = "Loading";
        //NGUITools.ClearChildren(Grid);
        StartCoroutine(DownloadLevel(LevelUrl, levels));
#endif
    }

    public void Start()
    {
      //  #if UNITY_ANDROID
            LoadDefaultLevels();
      //  #endif
        Instance = this;
    }

    public void LoadDefaultLevels()
    {
        var jsonObject = new JSONObject(Levels.text);
        var levelArray = jsonObject.GetField("levelArray").list;
        foreach (var level in levelArray)
        {
            LoadedLevels.Add(SerializedLevel.FromJson(level));
        }
        PopulateToGrid();
        InvokeLevelsLoaded(true);
    }

    #endregion

    public IEnumerator DownloadLevel(string url, JSONObject jsonObject)
    {
        var www = new WWW(url);
        yield return www;
        if (www.isDone && www.text != "")
        {
            jsonObject = new JSONObject(www.text);
            try
            {
                LoadedLevels.Clear();
                var levelArray = jsonObject.GetField("levelArray").list;
                foreach (var level in levelArray)
                {
                    LoadedLevels.Add(SerializedLevel.FromJson(level));
                }
                Debug.Log("Number of loaded levels: " + LoadedLevels.Count);
                PopulateToGrid();
                InvokeLevelsLoaded(true);
            }
            catch
            {
                LoadDefaultLevels();
            }
        }

        //NGUIDebug.Log("Levels Loaded:" + jsonObject);
    }

    public void PopulateToGrid()
    {
        /*NGUITools.ClearChildren(Grid);
        foreach (var level in LoadedLevels)
        {
            var child = NGUITools.AddChild(Grid, LevelLabelPrefab);
            child.name = level.Name;
            child.GetComponent<UILabel>().text = level.Name;
            child.GetComponent<UIButtonMessage>().target = gameObject;
            child.transform.localScale = new Vector3(40, 40, 0);
            child.GetComponent<UIDragPanelContents>().draggablePanel =
                Grid.transform.parent.gameObject.GetComponent<UIDraggablePanel>();
        }
        Grid.GetComponent<UIGrid>().Reposition();*/
        Loading.text = "";
    }
}