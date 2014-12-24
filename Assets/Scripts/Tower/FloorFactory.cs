using System;
using System.Collections.Generic;
using System.Linq;
using Tower;
using UnityEngine;

public class FloorFactory : ScriptableObject
{
    public List<FloorPrefab> RegisteredPrefabs = new List<FloorPrefab>();

    public Floor CreateFloor(string floorDescName)
    {
        FloorPrefab prefab = RegisteredPrefabs.FirstOrDefault(x => x.FloorName == floorDescName);
        if (prefab != null)
        {
            GameObject floorPrefab = prefab.Prefab;
            var go = Instantiate(floorPrefab) as GameObject;
            return go.GetComponent<Floor>();
        }

        return null;
    }

    public Floor CreateFloor(IFloorDesc floorDesc)
    {
        Floor floor = CreateFloor(floorDesc.GetType().FullName);
        floor.Init(floorDesc);
        return floor;
    }

    public void AddPrefabForType(string typeName, GameObject prefab)
    {
        if (RegisteredPrefabs.All(x => x.FloorName != typeName))
        {
            RegisteredPrefabs.Add(FloorPrefab.Create(typeName, prefab));
        }
    }

    #region Nested type: FloorPrefab
    [Serializable]
    public class FloorPrefab
    {
        public string FloorName;
        public GameObject Prefab;

        public static FloorPrefab Create(string floorName, GameObject prefab)
        {
            return new FloorPrefab
                   {
                       FloorName = floorName,
                       Prefab = prefab
                   };
        }
    }

    #endregion
}