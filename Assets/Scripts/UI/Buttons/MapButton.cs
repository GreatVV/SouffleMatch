using UnityEngine;

public class MapButton : MonoBehaviour
{
    public GameObject pauseButton;

    public Window levelList;

    public void OnPress()
    {
        PanelManager.Show(levelList);
        if (pauseButton)
        {
            pauseButton.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
        }
    }
}