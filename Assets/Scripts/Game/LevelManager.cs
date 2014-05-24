using System;
using System.Collections.Generic;
using Game.Data;
using UnityEngine;

namespace Game
{
    public class LevelManager : MonoBehaviour {

        public List<SerializedLevel> LoadedLevels = new List<SerializedLevel>();

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
                LoadedLevels.Add(SerializedLevel.FromJson(level));
            }

            FireLevelsAreReady();
        }

        public SerializedLevel this[int index]
        {
            get
            {
                return LoadedLevels[index];
            }
        }
    }
}