using UnityEngine;
using System.Collections;

public class GuiSendLivesPopup : Window {

    private void OnEnable()
    {   
    }

    protected override bool OnClose()
    {
        Debug.Log("onclose");

        return false;
    }

    public void OnCloseAnimationComplete()
    {
        Disable();
    }

    public override void OnCloseButton()
    {
        Close();
    }

    private void OnSendButton()
    {
        Close();
    }
    private void OnCheckAllFriends()
    {
        Close();
    }
}
