using UnityEngine;

public class GuiMoneyOnLevelList : MonoBehaviour
{
    public UILabel MoneyLabel;

    #region Event Handlers

    public void OnAddButtonClick()
    {
        UI.Instance.InAppPopup.Show();
    }

    private void OnDestroy()
    {
        Economy.Instance.MoneyChanged -= OnMoneyChanged;
    }

    private void OnMoneyChanged(int money)
    {
        MoneyLabel.text = string.Format("{0}", money);
    }

    #endregion

    public void Start()
    {
        Economy.Instance.MoneyChanged += OnMoneyChanged;
        OnMoneyChanged(Economy.Instance.Money);
    }
}