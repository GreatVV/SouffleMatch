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
            
        transform.localPosition = new Vector3(0, -800, -5);
        iTween.MoveTo(gameObject, new Vector3(0, 0, 0), 0.5f);
        UI.Instance.Gamefield.IsPause = true;
    }

    protected override bool OnClose()
    {
        Debug.Log("onclose");
        iTween.MoveTo(gameObject,
            iTween.Hash("x", 0, "y", 2, "z", 0, "time", 0.5f,
                "oncomplete", "OnCloseAnimationComplete", "oncompletetarget", gameObject, "oncompleteparams", 0));

        return false;
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