using System.Linq;
using NUnit.Framework;
using Tower;
using UnityEngine;

[TestFixture]
public class TowerConstructionTest : TestBase
{
    [Test]
    public void AddFloor()
    {
        var tower = GetTower();

        var floorGO = new GameObject("Floor", typeof (Floor)).GetComponent<Floor>();
        floorGO.Init(FloorType.PointerPerTile);

        tower.AddFloor(floorGO);

        Assert.AreEqual(1, tower.Floors.Count());
    }

    [Test]
    public void CreateFloorByFloorDescFromFactory()
    {
        var floorFactory = GetFloorFactory();

        floorFactory.AddPrefabForType(FloorType.DecreaseWinPoints.ToString(), new GameObject("Floor", typeof (Floor)));

        var floor = floorFactory.CreateFloor(FloorType.DecreaseWinPoints);

        Assert.That(floor.FloorType == FloorType.DecreaseWinPoints);
    }

    [Test]
    public void CreateFloorByFloorDescTypeNameFromFactory()
    {
        var floorFactory = GetFloorFactory();
        var floorName = FloorType.DecreaseWinPoints.ToString();

        var prefab = new GameObject("Floor", typeof (Floor));
        prefab.GetComponent<Floor>().Init(FloorType.DecreaseWinPoints);

        floorFactory.AddPrefabForType(floorName, prefab);

        var floor = floorFactory.CreateFloor(floorName);

        Assert.That(floor.FloorType == FloorType.DecreaseWinPoints);
    }

    [Test]
    public void RegisterFloorDescTypeTest()
    {
        var floorFactory = ScriptableObject.CreateInstance<FloorFactory>();

        var floorName = FloorType.DecreaseWinPoints.ToString();

        var tryFloor = floorFactory.CreateFloor(floorName);
        Assert.IsNull(tryFloor);

        floorFactory.AddPrefabForType(floorName, new GameObject("Floor", typeof (Floor)));

        tryFloor = floorFactory.CreateFloor(floorName);
        Assert.IsNotNull(tryFloor);
    }
}