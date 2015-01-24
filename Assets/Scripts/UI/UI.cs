using Game.Gameplay;
using Game.Gameplay.GamefieldStates;
using UnityEngine;
using Utils;

namespace UI
{
    public class UI : MonoBehaviour
    {

        public Gamefield Gamefield;

        void Start()
        {
            Gamefield.StateChanged += OnStateChanged;
        }

        public WinScreen WinScreen;
        public MissionProgressUI MissionProgressUI;

        private void OnStateChanged(GameState oldState, GameState newState)
        {
            if (newState is WinState || newState is GameOverState)
            {
                WinScreen.Init(Instance.ProgressionManager.Mana);
                WinScreen.Show(true);
            }
            else
            {
                WinScreen.Show(false);
            }

            if (oldState == null)
            {
                MissionProgressUI.Init(Gamefield.GameMode);
            }
        
        }
    }
}