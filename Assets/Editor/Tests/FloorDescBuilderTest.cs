using NUnit.Framework;
using Tower;
using UnityEngine;

[TestFixture]
public class FloorDescBuilderTest : TestBase
{
    [Test]
    public void CheckCanBuildIfCanBuild()
    {
        var builder = new FloorDescBuilder(FloorType.First);
        builder.MaxAmount = 1;

        Assert.AreEqual(1, builder.MaxAmount);

        var tower = GetTower();

        Assert.IsTrue(builder.CanCreateInTower(tower));

        var floor = new GameObject("Floor", typeof (Floor)).GetComponent<Floor>();
        floor.Init(FloorType.First);


        Assert.IsFalse(builder.CanCreateInTower(tower));

        builder.MaxAmount = 2;

        Assert.IsTrue(builder.CanCreateInTower(tower));
    }
}