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
    }          
  
    protected override bool OnClose()
    {
        return true;
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