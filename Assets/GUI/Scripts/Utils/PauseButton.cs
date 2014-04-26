using UnityEngine;
using System.Collections;

public class PauseButton : MonoBehaviour
{

    public Gamefield gamefield;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnClick()
    {
        Debug.Log("Toogle pause");
    }
}
