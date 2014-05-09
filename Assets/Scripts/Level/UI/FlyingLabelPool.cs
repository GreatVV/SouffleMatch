using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FlyingLabelPool : MonoBehaviour
{
    public UILabel Prefab;
    public List<UILabel> CurrentLabels = new List<UILabel>();

    public UILabel GetLabel
    {
        get
        {
            var label = CurrentLabels.FirstOrDefault(x => !x.gameObject.activeInHierarchy);
            if (label)
            {
                Debug.Log("Label: "+label.GetInstanceID());
                return label;
            }
            label = NGUITools.AddChild(gameObject,Prefab.gameObject).GetComponent<UILabel>();
            label.transform.localPosition = Vector3.zero;
            CurrentLabels.Add(label);
            return label;
        }
    }

    public void Return(UILabel label)
    {
        label.gameObject.SetActive(false);
    }
}