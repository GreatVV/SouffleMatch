using System;

[Serializable]
public class InitState : GamefieldState
{
    public InitState(Gamefield gamefield = null)
        : base(gamefield)
    {
    }

    #region Event Handlers

    public override void OnEnter()
    {
        Gamefield.PointSystem.Reset();
        Gamefield.Level.Reset();            
        
        Gamefield.Level.InitFromFile(Gamefield.LastLoadedLevel);
        Gamefield.StageManager.Init(Gamefield.LastLoadedLevel.Stages);


        Gamefield.NewTilesInColumns = new int[Gamefield.Level.Width];

        Gamefield.AddEventHandlers();

        Gamefield.InvokeGameStarted();
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