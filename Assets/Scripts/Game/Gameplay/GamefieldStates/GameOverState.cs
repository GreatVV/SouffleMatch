using System;

namespace GamefieldStates
{
    [Serializable]
    public class GameOverState : GamefieldState
    {
        #region Event Handlers

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