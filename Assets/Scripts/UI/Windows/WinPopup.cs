using UnityEngine.UI;

public class WinPopup : Window
{
    public Text pointLabel;

    void OnMapClick()
    {
        
    }

    public void Init(int currentPoints)
    {
        pointLabel.text = string.Format(Localization.Get("FinalWindow_Score"), currentPoints);
    }
}