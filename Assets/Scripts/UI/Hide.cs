using UnityEngine;

public class Hide : MonoBehaviour {

    public void SetInActive()
    {
        gameObject.SetActive(false);
    }

    public void SetActive()
    {
        gameObject.SetActive(true);
    }
}
