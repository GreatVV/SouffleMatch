public class GuiSendLivesPopup : Window
{
    private void OnEnable()
    {
    }

    protected override bool OnClose()
    {
        return true;
    }

    public void OnCloseAnimationComplete()
    {
        Disable();
    }

    public override void OnCloseButton()
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