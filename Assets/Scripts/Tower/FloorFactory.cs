using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Tower;
using UnityEngine;

public class FloorFactory : ScriptableObject
{
    public int TurnsPerFloor = 5;
    public int PointsPerTile = 1;

    public List<FloorPrefab> RegisteredPrefabs = new List<FloorPrefab>();

    public Floor CreateFloor(string floorDescName)
    {
        var prefab = RegisteredPrefabs.FirstOrDefault(x => x.FloorName == floorDescName);
        if (prefab != null)
        {
            var floorPrefab = prefab.Prefab;
            var go = Instantiate(floorPrefab) as GameObject;

            var floor = go.GetComponent<Floor>();
            var floorType = (FloorType) Enum.Parse(typeof (FloorType), floorDescName);
            floor.Init(floorType);
            return floor;
        }

        return null;
    }

    public bool CanBuildFloor(Tower.Tower tower, string floorDescName)
    {
        var prefab = RegisteredPrefabs.FirstOrDefault(x => x.FloorName == floorDescName);
        if (prefab != null)
        {
            return prefab.Builder.CanCreateInTower(tower);
        }
        return true;
    }

    public Floor CreateFloor(FloorType floorType)
    {
        return CreateFloor(floorType.ToString());
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
        public FloorDescBuilder Builder;
        public GameObject Prefab;

        public static FloorPrefab Create(string floorName, GameObject prefab)
        {
            var floorType = (FloorType)Enum.Parse(typeof (FloorType), floorName);
            return new FloorPrefab
                   {
                       FloorName = floorName,
                       Builder = new FloorDescBuilder(floorType),
                       Prefab = prefab
                   };
        }
    }

    #endregion
}