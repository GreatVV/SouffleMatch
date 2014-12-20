using Assets.Game.Data;
using Assets.Game.Gameplay;
using Assets.Plugins;
using Assets.Utils;
using UnityEngine;

namespace Assets.Game.Player
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
            if (Instance.SessionRestorer != this)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
            StartLevel(CurrentLevel);
        }

        public void Restart()
        {
            iTween.Stop();
            StartLevel(CurrentLevel);
        }
    }
}