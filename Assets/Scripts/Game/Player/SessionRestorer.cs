﻿using Assets.Game.Data;
using Assets.Game.Gameplay;
using Assets.Game.Levels;
using Assets.Plugins;
using Assets.Utils;
using UnityEngine;

namespace Assets.Game.Player
{
    [RequireComponent(typeof (LevelManager))]
    public class SessionRestorer : MonoBehaviour
    {
        public Gamefield Gamefield;
        public int lastPlayedLevel;

        public Tutorial.Tutorial tutorialPrefab;

        private LevelManager levelManager;
        public int lastPlayedPack;

        #region Events Subscribers

        private void OnPause(bool pause)
        {
            //Tutorial.SetActive(!pause);
        }

        private void OnLevelsAreReady()
        {
         //   Debug.Log("Start: "+lastPlayedLevel);
            StartLevel(lastPlayedPack,lastPlayedLevel);
        }

        #endregion

        #region Unity Methods

        private void Awake()
        {

            DontDestroyOnLoad(gameObject);

            levelManager = GetComponent<LevelManager>();

            lastPlayedLevel = PlayerPrefs.GetInt(Instance.Profile.CurrentPrefix+"LastPlayedLevelDescription", 0);

            levelManager.LevelsAreReady += OnLevelsAreReady;

            Gamefield.Paused += OnPause;
        }

        private void OnDestroy()
        {
            levelManager.LevelsAreReady -= OnLevelsAreReady;
        }

        private void OnApplicationPause()
        {
            if (Instance.ProgressionManager)
            {
                PlayerPrefs.SetInt(Instance.Profile.CurrentPrefix + "LastPlayedLevelDescription", lastPlayedLevel);
                PlayerPrefs.Save();

                Instance.ProgressionManager.SaveProgress();
            }
        }

        #endregion

        private void StartLevel(LevelDescription description)
        {
            Gamefield.StartGame(description);
        }

        public void StartLevel(int pack, int index)
        {
            lastPlayedPack = pack;
            lastPlayedLevel = index;
            StartLevel(levelManager.GetLevel(pack, index));
        
            /*
        if (index == 0)
        {
            if (!Tutorial.Instance)
            {
                Instantiate(tutorialPrefab);
            }
            Tutorial.Begin();
        }
        else
        {
            if (Tutorial.isActive)
            {
                Tutorial.End();
            }
        }*/
        }

        public void PlayNextLevel()
        {
          
            Debug.Log("Played Last played level:" + lastPlayedLevel);
            var count = levelManager.LevelPackManager.Packs[lastPlayedPack].LoadedLevels.Count - 1;
            if (lastPlayedPack < levelManager.LevelPackManager.Packs.Count - 1 &&
                lastPlayedLevel == count)
            {
                lastPlayedLevel = 0;
                lastPlayedPack++;
            }
            else
            {
                if (lastPlayedLevel < count)
                {
                    lastPlayedLevel++;
                }
                else
                {
                    lastPlayedPack = 0;
                    lastPlayedLevel = 0;
                }
            }
            Debug.Log("Last played level:" + lastPlayedLevel);
            StartLevel(lastPlayedPack, lastPlayedLevel);
        }

        public void Restart()
        {
            iTween.Stop();
            StartLevel(lastPlayedPack,lastPlayedLevel);
        }
    }
}