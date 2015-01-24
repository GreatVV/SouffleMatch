using Game.Data;
using Game.Gameplay;
using Plugins;
using UnityEngine;
using Utils;

namespace Game.Player
{
    public class SessionRestorer : MonoBehaviour
    {
        public LevelDescription CurrentLevel;

        public Gamefield Gamefield;

        private void StartLevel(LevelDescription description)
        {
            CurrentLevel = description;
            Gamefield.StartGame(description);
        }

        public void Start()
        {
            Instance.ChuzzlePool.Clear();
            StartLevel(Instance.LevelFactory.CurrentLevel);
        }

        public void Restart()
        {
            iTween.Stop();
            StartLevel(CurrentLevel);
        }
    }
}