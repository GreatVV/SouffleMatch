using UnityEngine;

public class GuiTaskPopup : Window
{   
    public UILabel TaskLabel;

    private void OnEnable()
    {       
    }

    protected override bool OnClose()
    {
        //Debug.Log("onclose");

        return false;
    }

    public void OnStartAnimationComplete()
    {
        Invoke("Close", 1);
    }

    public void OnCloseAnimationComplete()
    {
        Disable();
        UI.Instance.Gamefield.SwitchStateTo(UI.Instance.Gamefield.CheckSpecialState);
    }

    public void Show(GameMode gameMode)
    {
        TaskLabel.text = GameModeToString.GetString(gameMode);
        Show();
    }
}