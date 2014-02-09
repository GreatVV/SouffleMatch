
public class Gameplay : Window
{

    #region Localization
    
    public static Phrase PointsString = new Phrase("Очки: {0}", "Gameplay_Points");
    public static Phrase TargetString = new Phrase("Цель: {0}", "Gameplay_Target");
    public static Phrase TurnsString = new Phrase("Ходов: {0}", "Gameplay_Turns");

    #endregion

    public Gamefield gamefield;

    public UILabel PointsLabel;
    public UIProgressBar PointsBar;
    // public UILabel TargetScoreLabel;
    public UILabel TurnsLabel;
    public UIProgressBar TurnsBar;

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

    public void Awake()
    {
        gamefield.GameStarted += OnGameStarted;
    }

    public void OnDestroy()
    {
        gamefield.GameStarted -= OnGameStarted;
    }

    public void OnGameStarted(Gamefield gamefield)
    {
        AddEventHandlers(gamefield);

        //TargetScoreLabel.text = LocalizationStrings.GetString(TargetString, GameModeToString.GetString(gamefield.GameMode));
       // UI.Instance.TaskPopup.Show(gamefield.GameMode);
        OnTurnsChanged(gamefield.GameMode.Turns, gamefield.GameMode.StartTurns);
        OnPointsChanged(gamefield.PointSystem.CurrentPoints, gamefield.GameMode.TargetPoints);
        //  Camera.main.orthographicSize = gamefield.Level.Width;
    }

    private void OnPointsChanged(int points, int targetPoints)
    {
        PointsLabel.text = LocalizationStrings.GetString(PointsString, points);
        PointsBar.value = ((float) points)/targetPoints;
    }

    private void OnTurnsChanged(int turns, int maxTurns)
    {
        TurnsLabel.text = LocalizationStrings.GetString(TurnsString, turns);
        TurnsBar.value = ((float) turns)/maxTurns;
    }

    public void OnGameOver()
    {
//        UI.Instance.ShowGameoverPopup();
    }
}
