using System;
using System.Collections.Generic;
using Game.Data;
using UnityEngine;

namespace Game
{
    public class LevelManager : MonoBehaviour {

        public List<LevelDescription> LoadedLevels = new List<LevelDescription>();

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
            var jsonObject = new JSONObject(LevelFile.text);
            var levelArray = jsonObject.GetField("levelArray").list;
            foreach (var level in levelArray)
            {
                LoadedLevels.Add(LevelDescription.FromJson(level));
            }

            FireLevelsAreReady();
        }

        public LevelDescription this[int index]
        {
            get
            {
                return LoadedLevels[index];
            }
        }
    }
}