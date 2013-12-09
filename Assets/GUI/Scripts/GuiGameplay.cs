using UnityEngine;

public class GuiGameplay : Window
{
    public static Phrase PointsString = new Phrase("Очки: {0}", "Gameplay_Points");
    public static Phrase TargetString = new Phrase("Цель: {0}", "Gameplay_Target");
    public static Phrase TurnsString = new Phrase("Ходов: {0}", "Gameplay_Turns");

    public UILabel PointsLabel;
    public UILabel TargetScoreLabel;
    public UILabel TurnsLabel;

    #region Event Handlers

    public void OnChoseLevelClick()
    {
        UI.Instance.ShowMap();
    }

    public void OnGameOver()
    {
        UI.Instance.ShowGameoverPopup();
    }

    public void OnGameStarted(Gamefield gamefield)
    {
        AddEventHandlers(gamefield);

        TargetScoreLabel.text = LocalizationStrings.GetString(TargetString, GameModeToString.GetString(gamefield.GameMode));
        UI.Instance.TaskPopup.Show(gamefield.GameMode);
        OnTurnsChanged(gamefield.GameMode.Turns);
        OnPointsChanged(gamefield.PointSystem.CurrentPoints);
        Camera.main.orthographicSize = gamefield.Level.Width;
    }

    private void OnPointsChanged(int points)
    {
        PointsLabel.text = LocalizationStrings.GetString(PointsString, points);
    }

    public void OnRestartClick()
    {
        UI.Instance.Restart();
    }

    public void OnPauseClick()
    {
        UI.Instance.ShowPausePopup();
    }

    private void OnTurnsChanged(int turns)
    {
        TurnsLabel.text = LocalizationStrings.GetString(TurnsString, turns);
    }

    public void OnWin()
    {
        UI.Instance.ShowWinPopup();
    }

    #endregion

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
}