using System.Linq;
using NUnit.Framework;
using Tower;
using UnityEngine;

[TestFixture]
public class TowerConstructionTest
{

    [Test]
    public void AddFloor()
    {
        var tower = new Tower.Tower();

        var floorGO = new GameObject("Floor",typeof(Floor)).GetComponent<Floor>();
        floorGO.Init(new PointsPerTileFloorDesc());

        tower.AddFloor(floorGO);

        Assert.AreEqual(1, tower.Floors.Count());
    }

    [Test]
    public void CreateFloorByFloorDescFromFactory()
    {
        var floorFactory = GetFloorFactory();

        floorFactory.AddPrefabForType("DecreaseWinPointsFloorDesc", new GameObject("Floor", typeof (Floor)));

        var floorDesc = new DecreaseWinPointsFloorDesc();
        var floor = floorFactory.CreateFloor(floorDesc);

        Assert.AreSame(floorDesc, floor.FloorDescription);
        Assert.That(floor.FloorDescription is DecreaseWinPointsFloorDesc);

    }

    [Test]
    public void CreateFloorByFloorDescTypeNameFromFactory()
    {
        var floorFactory = GetFloorFactory();
        var floorName = "DecreaseWinPointsFloorDesc";

        var prefab = new GameObject("Floor", typeof (Floor));
        prefab.GetComponent<Floor>().Init( new DecreaseWinPointsFloorDesc() );

        floorFactory.AddPrefabForType("DecreaseWinPointsFloorDesc", prefab );

        var floor = floorFactory.CreateFloor(floorName);

        Assert.That(floor.FloorDescription is DecreaseWinPointsFloorDesc);
    }

    [Test]
    public void RegisterFloorDescTypeTest()
    {
        var floorFactory = GetFloorFactory();

        var floorName = "DecreaseWinPointsFloorDesc";

        var tryFloor = floorFactory.CreateFloor(floorName);
        Assert.IsNull(tryFloor);
        
        floorFactory.AddPrefabForType(floorName, new GameObject("Floor", typeof(Floor)));

        tryFloor = floorFactory.CreateFloor(floorName);
        Assert.IsNotNull(tryFloor);
    }

    private FloorFactory GetFloorFactory()
    {
        return new FloorFactory();
    }
}