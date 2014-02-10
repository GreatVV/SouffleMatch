public class WinPopup : Window
{
    public UILabel pointLabel;

    void OnMapClick()
    {
        
    }

    public void Init(int currentPoints)
    {
        pointLabel.text = string.Format("{0} points", currentPoints);
    }
}