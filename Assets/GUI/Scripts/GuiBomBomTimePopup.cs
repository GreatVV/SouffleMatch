using UnityEngine;
using System.Collections;
using System;

public class GuiBomBomTimePopup : MonoBehaviour 
{
    public event Action BomBomHided;

	void OnEnable () 
    {   
	    Invoke("Disable", 2);
    }

    void Disable()
    {
        if (BomBomHided != null)
        {
            BomBomHided();
        }
        gameObject.SetActive(false);
    }
}
