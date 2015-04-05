using System;
using Tower;
using UnityEngine;
using Utils;

public class ShopMenu : MonoBehaviour, IClearable
{
    public Transform root;
    public GameObject ShopButtonPrefab;

    public void Clear()
    {
        foreach (Transform child in root.transform)
        {
            DestroyImmediate(child.gameObject);
        }
    }

    public void Start()
    {
        var tower = FindObjectOfType<Tower.Tower>();

        Clear();

        var types = Enum.GetValues(typeof (FloorType));

        foreach (FloorType type in types)
        {
            if (type == FloorType.First)
            {
                continue;
            }

            var button = Instantiate(ShopButtonPrefab) as GameObject;
            var floorDescButton = button.GetComponent<FloorDescBuyButton>();
            floorDescButton.Init(type);
            floorDescButton.Tower = tower;
            button.transform.SetParent(root, false);
        }
    }
}