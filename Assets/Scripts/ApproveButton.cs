using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections;

public class ApproveButton : MonoBehaviour
{
    public GameObject YesButton;
    public GameObject NoButton;
    public GameObject DefaultButton;

    public string defaultMessage;
    public string approveMessage;

    public UIPlayTween showTween;

    public bool ShowOnStart;

    void Awake()
    {
        if (ShowOnStart)
        {
            OnEnable();
        }
        else
        {
            HideAll();
        }
    }

    private void HideAll()
    {
        YesButton.SetActive(false);
        NoButton.SetActive(false);
        DefaultButton.SetActive(false);
    }

    void OnEnable()
    {
        YesButton.SetActive(false);
        NoButton.SetActive(false);
        DefaultButton.SetActive(true);
    }

    public void OnDefaultClick()
    {
        showTween.Play(true);
    }

    public void OnYesClick()
    {
        Debug.LogError("Yes click should be custom!");
    }

    public void OnNoClick()
    {
        showTween.Play(false);
    }

}
