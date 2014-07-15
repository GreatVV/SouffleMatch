using System;
using System.Collections.Generic;
using Game.Data;
using UnityEngine;

namespace Game
{
    public class LevelManager : MonoBehaviour
    {

        public LevelPackManager LevelPackManager;

        public TextAsset LevelFile;
        public event Action LevelsAreReady;
        public bool IsLoaded;
        protected virtual void FireLevelsAreReady()
        {
            IsLoaded = true;
            Action handler = LevelsAreReady;
            if (handler != null) handler();
        }

        void Start()
        {
            LoadDefaultLevels();
        }

        public void LoadDefaultLevels()
        {
            LevelPackManager = LevelPackManager.Deserialize(LevelFile.text);
            FireLevelsAreReady();
        }

        public LevelDescription GetLevel(int pack, int index)
        {
            if (pack >= 0 && pack < LevelPackManager.Packs.Count && index >= 0 &&
                index < LevelPackManager.Packs[pack].LoadedLevels.Count)
            {
                return LevelPackManager.Packs[pack].LoadedLevels[index];
            }
            Debug.LogWarning("Can't find level: "+pack+ " - "+index);
            return LevelPackManager.Packs[0].LoadedLevels[0];
        }
    }
    [Serializable]
    public class LevelPack
    {
        public string Name;

        public List<LevelDescription> LoadedLevels = new List<LevelDescription>();

        public static LevelPack Deserialize(JSONObject json)
        {
            var levelDescription = new LevelPack();

            if (json.HasField("Name"))
            {
                levelDescription.Name = json.GetStringField("Name");
            }
            else
            {
                levelDescription.Name = "Default";
            }
            var levelArray = json.GetField("levelArray").list;
            foreach (var level in levelArray)
            {
                levelDescription.LoadedLevels.Add(LevelDescription.FromJson(level));
            }

            return levelDescription;
        }

        public LevelDescription this[int index]
        {
            get
            {
             //   Debug.Log("I:"+index);
                return LoadedLevels[index];
            }
        }

        public JSONObject Serialize()
        {
            var json = new JSONObject();
            json.AddField("Name", Name);
            var array = new JSONObject(JSONObject.Type.ARRAY);
            foreach (var levelDescription in LoadedLevels)
            {
                array.Add(levelDescription.Serialize());
            }

            json.AddField("levelArray", array);
            return json;
        }
    }

    [Serializable]
    public class LevelPackManager
    {
        public List<LevelPack> Packs = new List<LevelPack>();

        public JSONObject Serialize()
        {
            var json = new JSONObject(JSONObject.Type.ARRAY);
            foreach (var levelPack in Packs)
            {
                json.Add(levelPack.Serialize());
            }
            return json;
        }

        public static LevelPackManager Deserialize(string serialized)
        {
            var levelPackManager = new LevelPackManager();

            var json = new JSONObject(serialized);
            foreach (var jsonObject in json.list)
            {
                levelPackManager.Packs.Add(LevelPack.Deserialize(jsonObject));
            }

            return levelPackManager;
        }
    }
}