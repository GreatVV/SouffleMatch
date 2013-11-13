using System;

[Serializable]
public class GameOverState : GamefieldState
{
    public GameOverState(Gamefield gamefield = null)
        : base(gamefield)
    {
    }

    #region Event Handlers

    public override void OnEnter()
    {
    }

    public override void OnExit()
    {
    }

    #endregion

    public override void Update()
    {
    }

    public override void LateUpdate()
    {
    }
}