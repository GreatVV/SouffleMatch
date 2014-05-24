using UnityEngine;
using System.Collections;

public class BackButton : MonoBehaviour {

    void OnPress()
    {
        PanelManager.OnBackButton();
    }
}
