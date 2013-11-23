using UnityEngine;
using System.Collections;

public class GuiSendLivesPopup : Window {

    private void OnEnable()
    {   
        transform.localPosition = new Vector3(0, -800, -5);
        iTween.MoveTo(gameObject, new Vector3(0, 0, -0.01f), 0.5f);
    }

    protected override bool OnClose()
    {
        Debug.Log("onclose");
        iTween.MoveTo(gameObject,
            iTween.Hash("x", 0, "y", 2, "z", -0.01f, "time", 0.5f,
                "oncomplete", "OnCloseAnimationComplete", "oncompletetarget", gameObject, "oncompleteparams", 0));

        return false;
    }

    public void OnCloseAnimationComplete()
    {
        Disable();
    }

    private void OnCloseButton()
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
