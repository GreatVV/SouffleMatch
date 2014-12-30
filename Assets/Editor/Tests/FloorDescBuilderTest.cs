using NSubstitute;
using NUnit.Framework;
using Tower;
using Tower.Floors;
using UnityEngine;

[TestFixture]
public class FloorDescBuilderTest : TestBase
{
    public class TestFloor : IFloorDesc 
    {
        
    }

    public class TestFloorDescBuilder : FloorDescBuilder<TestFloor> 
    {
    }

    [Test]
    public void CheckCanBuildIfCanBuild()
    {
        var builder = new TestFloorDescBuilder();
        builder.MaxAmount = 1;

        Assert.AreEqual(1, builder.MaxAmount);

        var tower = GetTower();

        Assert.IsTrue(builder.CanCreateInTower(tower));

        var floor = new GameObject("Floor", typeof(Floor)).GetComponent<Floor>();
        floor.Init(builder.Create());

        tower.AddFloor(floor);

        Assert.IsFalse(builder.CanCreateInTower(tower));

        builder.MaxAmount = 2;

        Assert.IsTrue(builder.CanCreateInTower(tower));
    }
}