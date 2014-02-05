
public class Gameplay : Window
{

    #region Localization
    
    public static Phrase PointsString = new Phrase("Очки: {0}", "Gameplay_Points");
    public static Phrase TargetString = new Phrase("Цель: {0}", "Gameplay_Target");
    public static Phrase TurnsString = new Phrase("Ходов: {0}", "Gameplay_Turns");

    #endregion

    public UILabel PointsLabel;
    // public UILabel TargetScoreLabel;
    public UILabel TurnsLabel;

    public void AddEventHandlers(Gamefield gamefield)
    {
        RemoveEventHandlers(gamefield);
        gamefield.PointSystem.PointChanged += OnPointsChanged;
        gamefield.GameMode.GameOver += OnGameOver;
        //gamefield.GameMode.Win += OnWin;
        gamefield.GameMode.TurnsChanged += OnTurnsChanged;
    }

    private void RemoveEventHandlers(Gamefield gamefield)
    {
        gamefield.PointSystem.PointChanged -= OnPointsChanged;
        gamefield.GameMode.GameOver -= OnGameOver;
        //  gamefield.GameMode.Win -= OnWin;
        gamefield.GameMode.TurnsChanged -= OnTurnsChanged;
    }

    public void OnGameStarted(Gamefield gamefield)
    {
        AddEventHandlers(gamefield);

        //TargetScoreLabel.text = LocalizationStrings.GetString(TargetString, GameModeToString.GetString(gamefield.GameMode));
       // UI.Instance.TaskPopup.Show(gamefield.GameMode);
        OnTurnsChanged(gamefield.GameMode.Turns);
        OnPointsChanged(gamefield.PointSystem.CurrentPoints);
        //  Camera.main.orthographicSize = gamefield.Level.Width;
    }

    private void OnPointsChanged(int points)
    {
        PointsLabel.text = LocalizationStrings.GetString(PointsString, points);
    }

    private void OnTurnsChanged(int turns)
    {
        TurnsLabel.text = LocalizationStrings.GetString(TurnsString, turns);
    }

    public void OnGameOver()
    {
//        UI.Instance.ShowGameoverPopup();
    }
	
}
