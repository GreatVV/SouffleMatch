using UnityEngine;

public class MapButton : MonoBehaviour
{
    public Window levelList;

    public void OnPress()
    {
        PanelManager.Show(levelList);
    }
}