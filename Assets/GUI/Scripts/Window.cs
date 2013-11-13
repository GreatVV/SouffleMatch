using UnityEngine;

public class Window : MonoBehaviour
{
    public Window ShowAfter;

    public void Show(Window showAfterAction = null)
    {
        ShowAfter = showAfterAction;
        gameObject.SetActive(true);
        OnShow();
    }

    protected virtual void OnShow()
    {
    }

    public void Close()
    {
        var needDisable = OnClose();
        if (needDisable)
        {
            Disable();
        }
    }

    protected void Disable()
    {
        gameObject.SetActive(false);
        if (ShowAfter != null)
        {
            ShowAfter.Show();
            ShowAfter = null;
        }
    }

    protected virtual bool OnClose()
    {
        return true;
    }
}