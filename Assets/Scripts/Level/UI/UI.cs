using System.Linq;
using UnityEngine;

public class UI : MonoBehaviour
{
    public static UI Instance;
    public Gamefield Gamefield;

    public GuiGameplay GuiGameplay;
    public GuiGameOverPopup GameOverPopup;
    public GuiWinPanel WinPanel;
    public GuiLevelList GuiLevelList;
    public GuiBuyLivesPopup BuyLivesPopup;
    public GuiInAppPopup InAppPopup;
    public GuiPausePopup PausePopup;
    public GuiStartLevelPopup StartLevelPopup;
    public GuiTaskPopup TaskPopup;
    public GuiBomBomTimePopup BomBomPopup;

    #region Event Handlers

    private void OnDestroy()
    {
        RemoveEventHandlers();
    }

    public void TryStartLevel(SerializedLevel level=null)
    {
      //  DisableAllPanels();

        bool canStartLevel = Player.Instance.Lifes.HasLife;
        if (canStartLevel)
        {
            DisableAllPanels();
            GuiGameplay.Show();
            Gamefield.gameObject.SetActive(true);
            Gamefield.StartGame(level);
        }
        else
        {
            //TODO show buy life popup;
            BuyLivesPopup.Show();
        }

    }

    public void OnStartClick()
    {
        TryStartLevel();
    }

    public void OnWinRestartClick()
    {
        GameOverRestart();
    }

    #endregion

    public void Awake()
    {
        AddEventHandlers();

        Instance = this;

        //Application.RegisterLogCallback(CallBackLog);
        // Application.RegisterLogCallbackThreaded(CallBackLog);
    }

    public void Restart()
    {   
        TryStartLevel(Player.Instance.LastPlayedLevel);
    }

    private void AddEventHandlers()
    {
        RemoveEventHandlers();
        Gamefield.GameStarted += GuiGameplay.OnGameStarted;
    }

    private void RemoveEventHandlers()
    {
        Gamefield.GameStarted -= GuiGameplay.OnGameStarted;
    }

    private void CallBackLog(string condition, string stacktrace, LogType type)
    {
        NGUIDebug.Log("\nType: " + type);
        NGUIDebug.Log(stacktrace);
        NGUIDebug.Log(condition);
    }

    private void DisableAllPanels()
    {
        Gamefield.gameObject.SetActive(false);
        GuiLevelList.Close();
        GuiGameplay.Close();
        GameOverPopup.Close();
        WinPanel.Close();
        InAppPopup.Close();
        BuyLivesPopup.Close();
        PausePopup.Close();
        StartLevelPopup.Close();
        TaskPopup.Close();
    }

    public void ShowMap()
    {
        DisableAllPanels();
        GuiLevelList.Show();
    }

    public void ShowWinPopup()
    {
        DisableAllPanels();
        WinPanel.Show(Gamefield.GameMode.Turns, Gamefield.PointSystem.CurrentPoints, Player.Instance.GetLevelInfo(Gamefield.Level.Serialized.Name).BestScore, Player.Instance.GetLevelInfo(Gamefield.Level.Serialized.Name), Gamefield.Level.Serialized);
    }

    public void ShowGameoverPopup()
    {
        DisableAllPanels();
        GameOverPopup.Show();
    }

    public void AddTurns()
    {
        DisableAllPanels();
        if (Economy.Instance.Spent(Player.Instance.AddTurnsPrice))
        {
            Gamefield.GameMode.AddTurns(5);
            Gamefield.GameMode.IsGameOver = false;
            GuiGameplay.Show();
            Gamefield.gameObject.SetActive(true);
            Gamefield.SwitchStateTo(Gamefield.CheckSpecialState);
        }
        else
        {
            InAppPopup.Show(GameOverPopup);
        }
    }

    public void GameOverRestart()
    {
        DisableAllPanels();
        GuiGameplay.Show();
        Gamefield.gameObject.SetActive(true);
        Restart();
    }

    public void ShowInAppPopup(Window showOnClose = null)
    {
        if (showOnClose != null)
        {
            showOnClose.Close();
        }
        InAppPopup.Show(showOnClose);
    }

    public void ShowPausePopup()
    {   
        PausePopup.Show();
    }

    public void ShowStartLevelPopup(SerializedLevel levelToLoad)
    {
        StartLevelPopup.Show(levelToLoad);
    }

    public void ShowBomBomTime()
    {
        BomBomPopup.gameObject.SetActive(true);
    }
}