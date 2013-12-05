using UnityEngine;

public class GuiInAppPopup : Window
{
    public UILabel MoneyLabel;

    #region Event Handlers

    public void OnAdd100()
    {
        Economy.Instance.Add(100);
        if (ShowAfter != null)
        {
            Close();
        }
    }

    public void OnAdd1000()
    {
        Economy.Instance.Add(1000);
        if (ShowAfter != null)
        {
            Close();
        }
    }

    public void OnAdd10000()
    {
        Economy.Instance.Add(10000);
        if (ShowAfter != null)
        {
            Close();
        }
    }

    private void OnDestroy()
    {
        RemoveEventHandlers();
    }

    private void OnDisable()
    {
        RemoveEventHandlers();
    }

    private void OnEnable()
    {
        AddEventHandlers();
        OnMoneyChanged(Economy.Instance.CurrentMoney);
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

    private void OnMoneyChanged(int money)
    {
        MoneyLabel.text = string.Format("Money: {0}", money);
    }

    #endregion

    public void AddEventHandlers()
    {
        RemoveEventHandlers();
        Economy.Instance.MoneyChanged += OnMoneyChanged;
    }

    private void RemoveEventHandlers()
    {
        Economy.Instance.MoneyChanged -= OnMoneyChanged;
    }

    public void OnCloseButton()
    {
        Close();
    }
}