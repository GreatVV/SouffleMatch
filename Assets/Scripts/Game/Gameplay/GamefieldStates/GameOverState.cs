using System;

namespace GamefieldStates
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
            PanelManager.Show(PanelManager.instance.LosePopup);
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