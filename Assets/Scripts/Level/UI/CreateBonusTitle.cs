using UnityEngine;
using System.Collections;
using System;

public class CreateBonusTitle : MonoBehaviour 
{
    public event Action WinTitleDestroyed;

	void Start () 
    {
        transform.position = new Vector3(0, 5, 0);
        Destroy(gameObject, 2);
	}

    void OnDestroy()
    {
        if (WinTitleDestroyed != null)
        {
            WinTitleDestroyed();
        }
    }
}
