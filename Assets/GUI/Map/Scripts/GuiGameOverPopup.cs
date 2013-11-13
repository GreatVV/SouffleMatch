public class GuiGameOverPopup : Window
{
    public static Phrase MoneyString = new Phrase("У вас {0} монет", "GameOverPopup_Money");

    public UILabel MoneyLabel;

    #region Event Handlers

    public void OnAddTurns()
    {
        UI.Instance.AddTurns();
    }

    private void OnEnable()
    {
        MoneyLabel.text = LocalizationStrings.GetString(MoneyString, Economy.Instance.CurrentMoney);
    }

    public void OnGameOverRestartClick()
    {
        UI.Instance.GameOverRestart();
    }

    #endregion
}