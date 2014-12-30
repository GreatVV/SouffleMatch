using Game.Data;
using Game.Gameplay;
using Plugins;
using UnityEngine;
using Utils;

namespace Game.Player
{
    public class SessionRestorer : MonoBehaviour
    {
        public static LevelDescription CurrentLevel;

        public LevelDescription DebugCurrentLevel; 

        public Gamefield Gamefield;

        private void StartLevel(LevelDescription description)
        {
            CurrentLevel = description;
            Gamefield.StartGame(description);
        }

        public void Start()
        {
            if (Application.isEditor)
            {
                CurrentLevel = DebugCurrentLevel; //TODO REMOVE!
            }

            Instance.ChuzzlePool.Clear();
            StartLevel(CurrentLevel);
        }

        public void Restart()
        {
            iTween.Stop();
            StartLevel(CurrentLevel);
        }
    }
}