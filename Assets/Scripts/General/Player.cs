using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEditor;
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
        SerializeLevelInfo(jsonObject, Levels);
        jsonObject.AddField("Lifes", Lifes.Serialize());
        return jsonObject;
    }

    private static JSONObject SerializeLevelInfo(JSONObject jsonObject, IEnumerable<LevelInfo> levels)
    {
        var levelInfo = new JSONObject(JSONObject.Type.ARRAY);
        foreach (var level in levels)
        {
            levelInfo.Add(level.Serialize());
        }
        jsonObject.AddField("LevelInfo", levelInfo);
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

    [MenuItem("Utils/Unlock All Levels")]
    public static void UnlockAllLevels()
    {
        var levels = new List<LevelInfo>();
        for (int i = 0; i < 40; i++)
        {
            var levelInfo = new LevelInfo
            {
                IsCompleted = true, 
                BestScore = 100000,
                Name = i.ToString(CultureInfo.InvariantCulture),
                Number = i,
                NumberOfAttempts = 0
            };
            levels.Add(levelInfo);
        }
        var json = SerializeLevelInfo(new JSONObject(), levels);
        Profile.SavePlayer(Profile.GetPrefix(Profile.defaultProfileName), json);
        PlayerPrefs.Save();
        Debug.Log("Unlocked: "+json);
    }
}