using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FlyingLabelPool : MonoBehaviour
{
    public FlyingPoints Prefab;
    public List<FlyingPoints> CurrentLabels = new List<FlyingPoints>();

    public FlyingPoints GetLabel
    {
        get
        {
            var label = CurrentLabels.FirstOrDefault(x => !x.gameObject.activeInHierarchy);
            if (label)
            {
                //Debug.Log("Label: "+label.GetInstanceID());
                //label.gameObject.SetActive(true);
                return label;
            }
            label = NGUITools.AddChild(gameObject,Prefab.gameObject).GetComponent<FlyingPoints>();
            label.transform.localPosition = Vector3.zero;
            CurrentLabels.Add(label);
            return label;
        }
    }

    public void Return(object label)
    {
        (label as GameObject).gameObject.SetActive(false);
    }
}