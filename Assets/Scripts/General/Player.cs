using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;
    public List<LevelInfo> Levels;
    public LifeSystem Lifes;

    public SerializedLevel LastPlayedLevel;
    public int LifePrice = 100;
    public int AddTurnsPrice = 70;

    private void Awake()
    {
        Instance = this;
    }

    public LevelInfo GetLevelInfo(string levelName)
    {
        var levelInfo = Levels.FirstOrDefault(x => x.Name == levelName);
        if (levelInfo == null)
        {
            levelInfo = new LevelInfo {Name = levelName};
            Levels.Add(levelInfo);
            levelInfo.Number = Convert.ToInt32(levelName);
        }
        return levelInfo;
    }

    public JSONObject Serialize()
    {
        var jsonObject = new JSONObject(JSONObject.Type.OBJECT);
        var levelInfo = new JSONObject(JSONObject.Type.ARRAY);
        foreach (var level in Levels)
        {
            levelInfo.Add(level.Serialize());
        }
        jsonObject.AddField("LevelInfo", levelInfo);
        jsonObject.AddField("Lifes", Lifes.Serialize());
        return jsonObject;
    }

    public void Unserialize(JSONObject jsonObject)
    {
        Levels.Clear();
        var levelInfo = jsonObject.GetField("LevelInfo");
        foreach (var o in levelInfo.list)
        {
            Levels.Add(LevelInfo.Unserialize(o));
        }
        Lifes = LifeSystem.Unserialize(jsonObject.GetField("Lifes"));
    }
}