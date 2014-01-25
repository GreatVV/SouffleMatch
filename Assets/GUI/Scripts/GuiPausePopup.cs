#region

using UnityEngine;

#endregion

public class GuiPausePopup : Window
{
    public UILabel TaskLabel;

    #region Event Handlers

    private void OnContinueClick()
    {
        Close();
        
    }

    private void OnEnable()
    {
        TaskLabel.text = GameModeToString.GetString(UI.Instance.Gamefield.GameMode);
            
        UI.Instance.Gamefield.IsPause = true;
    }

    protected override bool OnClose()
    {
        return true;
    }

    public void OnCloseAnimationComplete()
    {
        Disable();
        UI.Instance.Gamefield.IsPause = false;
    }

    private void OnMapClick()
    {
        UI.Instance.ShowMap();
        Player.Instance.Lifes.SpentLife();
    }

    private void OnRestartClick()
    {
        UI.Instance.Restart();
        Player.Instance.Lifes.SpentLife();
    }

    #endregion
}