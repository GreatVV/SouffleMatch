using UnityEngine;

public class GuiTaskPopup : Window
{   
    public UILabel TaskLabel;

    private void OnEnable()
    {       
        transform.localPosition = new Vector3(0, -800, -5);
        iTween.MoveTo(gameObject, iTween.Hash("x", 0, "y", 0, "z", -0.01f, "time", 1f, "oncomplete", "OnStartAnimationComplete", "oncompletetarget", gameObject, "oncompleteparams", 0));
    }

    protected override bool OnClose()
    {
        //Debug.Log("onclose");
        iTween.MoveTo(gameObject,iTween.Hash("x", 0, "y", 2, "z", -0.01f, "time", 0.5f, "oncomplete", "OnCloseAnimationComplete", "oncompletetarget", gameObject, "oncompleteparams", 0));

        return false;
    }

    public void OnStartAnimationComplete()
    {
        Invoke("Close", 1);
    }

    public void OnCloseAnimationComplete()
    {
        Disable();
        UI.Instance.Gamefield.SwitchStateTo(UI.Instance.Gamefield.CheckSpecial);
    }

    public void Show(GameMode gameMode)
    {
        TaskLabel.text = GameModeToString.GetString(gameMode);
        Show();
    }
}