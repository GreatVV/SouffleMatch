﻿using System;
using Assets.Game.Data;
using Assets.Tower;
using UnityEngine;

namespace Assets.Game.Levels
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

        public LevelDescription GetLevel(TowerDescription towerDesc)
        {
            throw new NotImplementedException();
        }
    }
}