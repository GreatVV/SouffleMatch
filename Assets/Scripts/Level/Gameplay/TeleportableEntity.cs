using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Chuzzle))]
public class TeleportableEntity : MonoBehaviour
{   
    public GameObject Copy;   

    public bool HasCopy;

    public void CreateCopy()
    {   
        Copy = Instantiate(gameObject) as GameObject;
        Copy.transform.parent = gameObject.transform;
        Copy.gameObject.name += " is copy";
        HasCopy = true;
        //Debug.Log("Create copy "+gameObject);
    }

    public void DestroyCopy()
    {
        if (HasCopy)
        {
            Destroy(Copy);
            HasCopy = false;
            //Debug.Log("Destroy copy " + Copy);
        }
    }
}