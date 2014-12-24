using System;

namespace Game.Gameplay.GamefieldStates
{
    [Serializable]
    public class GameOverState : GameState
    {
        #region Event Handlers

        public GameOverState(Gamefield gamefield) : base(gamefield)
        {
        }

        public override void OnEnter()
        {
        }

        public override void OnExit()
        {
        }

        #endregion

        public override void UpdateState()
        {
        }

        public override void LateUpdateState()
        {
        }
    }
}