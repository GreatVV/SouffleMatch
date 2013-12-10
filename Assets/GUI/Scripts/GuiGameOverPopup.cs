public class GuiGameOverPopup : Window
{   
    #region Event Handlers

    public void OnAddTurns()
    {
        UI.Instance.AddTurns();
    }

    private void OnEnable()
    {   
    }

    public void OnGameOverRestartClick()
    {
        UI.Instance.GameOverRestart();
    }

    #endregion
}