using Game.Data;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof (LevelManager))]
    public class SessionRestorer : MonoBehaviour
    {
        public static SessionRestorer Instance;
        public Gamefield Gamefield;
        public Gameplay Gameplay;
        public int lastPlayedLevel;

        public Tutorial tutorialPrefab;

        private LevelManager levelManager;
        public int lastPlayedPack;

        #region Events Subscribers

        public void OnWindowChanged(Window currentActiveWindow)
        {
            //Debug.Log("Window changed: " + currentActiveWindow);
            Gamefield.IsPause = !PanelManager.IsCurrent(Instance.Gameplay);
        }

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
            if (Instance != null)
            {
                Destroy(this);
                return;
            }

            DontDestroyOnLoad(gameObject);

            Instance = this;

            levelManager = GetComponent<LevelManager>();

            lastPlayedLevel = PlayerPrefs.GetInt(Profile.CurrentPrefix+"LastPlayedLevelDescription", 0);

            levelManager.LevelsAreReady += OnLevelsAreReady;

            PanelManager.WindowChanged += OnWindowChanged;


            Gamefield.Paused += OnPause;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                levelManager.LevelsAreReady -= OnLevelsAreReady;
            }
        }

        private void OnApplicationPause()
        {
            if (ProgressionManager.Instance)
            {
                PlayerPrefs.SetInt(Profile.CurrentPrefix + "LastPlayedLevelDescription", lastPlayedLevel);
                PlayerPrefs.Save();

                ProgressionManager.Instance.SaveProgress();
            }
        }

        #endregion

        private void StartLevel(LevelDescription description)
        {
            Gamefield.StartGame(description);
            PanelManager.Show(Gameplay, true);
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