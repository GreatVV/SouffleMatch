using UnityEngine;

public class GuiInAppPopup : Window
{
    public UILabel MoneyLabel;

    #region Event Handlers

    public void OnAdd140()
    {
        AddMoney(140);
    }

    public void OnAdd800()
    {
        AddMoney(800);
    }

    public void OnAdd1700()
    {
        AddMoney(1400);
    }

    private void AddMoney(int amount)
    {
        Economy.Instance.Add(amount);
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
        MoneyLabel.text = string.Format("{0}", money);
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

    public override void OnCloseButton()
    {
        Close();
    }
}