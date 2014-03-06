using System;

[Serializable]
public class InitState : GamefieldState
{
    #region Event Handlers

    public override void OnEnter()
    {
        Gamefield.Reset();
        Gamefield.InvokeGameStarted();
        Gamefield.SwitchStateTo(Gamefield.CheckSpecialState);
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