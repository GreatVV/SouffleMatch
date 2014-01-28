using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Window : UIPanel
{
    public Window ShowAfter;

    public int Depth
    {
        get { return depth; }
        set { depth = value; }
    }

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

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnCloseButton();
        }
        OnUpdate();
    }

    public virtual void OnCloseButton()
    {
        
    }
    

    protected virtual void OnUpdate()
    {
        
    }

    public void StopRecieveTouches()
    {
        var colliders = GetComponentsInChildren<Collider>();
        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }
    }

    public void StartReceiveTouches()
    {
        
    }
}

public class PanelManager : MonoBehaviour
{
    public List<Window> windows  = new List<Window>();

    public void Show(Window newWindow)
    {
        foreach (var window in windows)
        {
            window.StopRecieveTouches();
        }
        windows.Add(newWindow);
        newWindow.Depth = windows.Count;
    }

    public void Close()
    {
        var toClose = windows.Last();
        windows.Remove(toClose);
        windows.Last().StartReceiveTouches();
    }

    public void OnBackButton()
    {
        Close();
    }
}