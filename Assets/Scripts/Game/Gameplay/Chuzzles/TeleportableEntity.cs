#region

using System.Runtime.InteropServices;
using UnityEngine;

#endregion

//[RequireComponent(typeof (Chuzzle))]
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
        get { return Copy != null && Copy.activeSelf; }
    }

    public void Show()
    {
        if (Copy == null)
        {
            if (gameObject.GetComponent<iTween>())
            {
                Debug.Log("iTween on "+gameObject.name);
            }

            Copy = Instantiate(gameObject) as GameObject;
            Copy.gameObject.name += " is copy";
            Copy.transform.parent = gameObject.transform;
            iTween.Stop(Copy);
            Destroy(Copy.GetComponent<Chuzzle>());
        }
        

        if (!Copy.activeSelf)
        {
            Copy.SetActive(true);
        }
        
    }

    public void Hide()
    {
        if (HasCopy)
        {
            Copy.SetActive(false);
        }
    }
}