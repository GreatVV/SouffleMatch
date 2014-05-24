using System;

namespace GamefieldStates
{
    [Serializable]
    public class GameOverState : GamefieldState
    {
        public LosePopup LosePopup;

        #region Event Handlers

        public override void OnEnter()
        {
            PanelManager.Show(LosePopup);
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