using UnityEngine;
using System.Collections;

public class TeleportableEntity : MonoBehaviour
{                                 
    public bool teleported;
    public GameObject teleportedBy;
    
    public bool isCopy;
    public TeleportableEntity duplicate;

    public Vector3 prevPosition;

    public void CreateDuplicate(Vector3 position)
    {
        if (duplicate != null)
        {
            return;
        }

        var gameObject = GameObject.Instantiate(this.gameObject) as GameObject;
        duplicate = gameObject.GetComponent<TeleportableEntity>();
        duplicate.transform.position = position;
        duplicate.isCopy = true;        
    }             
    

}