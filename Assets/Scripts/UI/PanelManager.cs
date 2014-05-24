using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    public List<Window> windows  = new List<Window>();
    public static PanelManager instance { get; private set; }

    public RuntimeAnimatorController windowAnimator;

    private void ShowWindow(Window newWindow)
    {
        if (windows.LastOrDefault() == newWindow)
        {
            return;
        }

        if (windows.Any())
        {
            windows.Last().Deactivate();
        }
        windows.Add(newWindow);
        newWindow.gameObject.SetActive(true);
        newWindow.Depth = windows.Count;
        newWindow.Activate();

        FireWindowChanged(newWindow);

        if (windowAnimator)
        {
            newWindow.Animator.runtimeAnimatorController = windowAnimator;
            newWindow.Animator.SetBool("Close", false);
        }
    }

    public static void Show(Window window, bool dropAllOtherWindows = false)
    {
        if (instance)
        {
            if (dropAllOtherWindows)
            {
                instance.CloseAll();
            } 
            instance.ShowWindow(window);
        }
        else
        {
            Debug.LogError("Not instance yet in PanelManager");
        }

    }

    public void Close()
    {
        var toClose = windows.LastOrDefault();
        if (toClose != null && windowAnimator)
        {
            var animator = toClose.gameObject.GetComponent<Animator>();
            animator.SetBool("Close", true);
            var length = animator.GetCurrentAnimationClipState(0)[0].clip.length;
            Invoke("CloseAnimationEnded", length);
        }
        else
        {
            CloseAnimationEnded();
        }
    }

    public void CloseAll()
    {
        foreach (var window in windows)
        {
            window.gameObject.SetActive(false);
        }
        windows.Clear();
    }

    public void CloseAnimationEnded()
    {
        var toClose = windows.LastOrDefault();
        windows.Remove(toClose);
        toClose.gameObject.SetActive(false);
        if (windows.Any())
        {
            windows.Last().Activate();
        }
    }

    public static void OnBackButton()
    {
        instance.Close();
    }

    void Awake()
    {
        if (instance)
        {
            Destroy(this);
            return;
        }

        instance = this;

    }

    void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    public static bool IsCurrent(Window window)
    {
        return instance.windows.Any() && instance.windows.Last() == window;
    }

    public static event Action<Window> WindowChanged;

    private static void FireWindowChanged(Window window)
    {
        Action<Window> handler = WindowChanged;
        if (handler != null) handler(window);
    }
}