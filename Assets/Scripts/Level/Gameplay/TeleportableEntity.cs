#region

using UnityEngine;

#endregion

[RequireComponent(typeof (Chuzzle))]
public class TeleportableEntity : MonoBehaviour
{
    public GameObject Copy;

    #region Event Handlers

    public void OnDestroy()
    {
        Destroy(Copy);
    }

    #endregion

    public bool HasCopy
    {
        get { return Copy && Copy.activeSelf; }
    }

    public void CreateCopy()
    {
        if (!HasCopy)
        {
            Copy = Instantiate(gameObject) as GameObject;
            Copy.gameObject.name += " is copy";
        }
        else
        {
            Copy.SetActive(true);
        }
        Copy.transform.parent = gameObject.transform;
    }

    public void DestroyCopy()
    {
        if (HasCopy)
        {
            Copy.SetActive(false);
        }
    }
}