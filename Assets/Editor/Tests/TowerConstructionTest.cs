using System.Linq;
using NUnit.Framework;
using Tower;
using Tower.Floors;
using UnityEngine;

[TestFixture]
public class TowerConstructionTest : TestBase
{
    [Test]
    public void AddFloor()
    {
        Tower.Tower tower = GetTower();

        var floorGO = new GameObject("Floor", typeof (Floor)).GetComponent<Floor>();
        floorGO.Init(new PointsPerTileFloorDesc());

        tower.AddFloor(floorGO);

        Assert.AreEqual(1, tower.Floors.Count());
    }

    [Test]
    public void CreateFloorByFloorDescFromFactory()
    {
        FloorFactory floorFactory = GetFloorFactory();

        floorFactory.AddPrefabForType(typeof(DecreaseWinPointsFloorDesc).FullName, new GameObject("Floor", typeof (Floor)));

        var floorDesc = new DecreaseWinPointsFloorDesc();
        Floor floor = floorFactory.CreateFloor(floorDesc);

        Assert.AreSame(floorDesc, floor.FloorDescription);
        Assert.That(floor.FloorDescription is DecreaseWinPointsFloorDesc);
    }

    [Test]
    public void CreateFloorByFloorDescTypeNameFromFactory()
    {
        FloorFactory floorFactory = GetFloorFactory();
        string floorName = typeof(DecreaseWinPointsFloorDesc).FullName;

        var prefab = new GameObject("Floor", typeof (Floor));
        prefab.GetComponent<Floor>().Init(new DecreaseWinPointsFloorDesc());

        floorFactory.AddPrefabForType(typeof(DecreaseWinPointsFloorDesc).FullName, prefab);

        Floor floor = floorFactory.CreateFloor(floorName);

        Assert.That(floor.FloorDescription is DecreaseWinPointsFloorDesc);
    }

    [Test]
    public void RegisterFloorDescTypeTest()
    {
        var floorFactory = ScriptableObject.CreateInstance<FloorFactory>();

        string floorName = typeof(DecreaseWinPointsFloorDesc).FullName;

        Floor tryFloor = floorFactory.CreateFloor(floorName);
        Assert.IsNull(tryFloor);

        floorFactory.AddPrefabForType(floorName, new GameObject("Floor", typeof (Floor)));

        tryFloor = floorFactory.CreateFloor(floorName);
        Assert.IsNotNull(tryFloor);
    }
}