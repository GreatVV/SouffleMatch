using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tower;
using Tower.Floors;
using UnityEngine;
using Utils;

public class ShopMenu : MonoBehaviour, IClearable
{
    public GameObject ShopButtonPrefab;

    public Transform root;

    public void Start()
    {
        var tower = FindObjectOfType<Tower.Tower>();

        Clear();

        IEnumerable<Type> types =
            Assembly.GetExecutingAssembly().GetTypes().Where(x => typeof (IFloorDesc).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);
        foreach (Type type in types)
        {
            var button = Instantiate(ShopButtonPrefab) as GameObject;
            var floorDescButton = button.GetComponent<FloorDescBuyButton>();
            floorDescButton.FloorName = type.FullName;
            floorDescButton.Tower = tower;
            button.transform.SetParent(root, false);
        }
    }

    public void Clear()
    {
        foreach (Transform child in root.transform)
        {
            DestroyImmediate(child.gameObject);
        }
    }
}