using UI;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class MenuButton : MonoBehaviour, IPointerClickHandler {

	// Use this for initialization

    public void OnPointerClick(PointerEventData eventData)
    {
        Application.LoadLevel(ScenesName.Menu);
    }
}
