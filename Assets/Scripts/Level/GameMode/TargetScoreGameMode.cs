
using System;

[Serializable]
public class TargetScoreGameMode : GameMode
{
    public int TargetScore;

    public override int TargetPoints { get { return TargetScore; }}

    public TargetScoreGameMode(GameModeDescription description) : base(description)
    {
        TargetScore = description.TargetScore;
    }                                 

    public override void OnDestroy()
    {
        PointSystem.PointChanged -= OnPointChanged;
    }

    public void OnPointChanged(int points, int i)
    {
        if (points >= TargetScore)
        {
            IsWin = true;
        }
    }

    protected override void OnInit()
    {
        PointSystem = Gamefield.PointSystem;
        PointSystem.PointChanged += OnPointChanged;
    }

    public override void OnReset()
    {
        Turns = StartTurns;
    }

    public override void HumanTurn()
    {
        SpendTurn();
    }

    public override string ToString()
    {
        return string.Format("You should get {0} points", TargetScore);
    }
}