using NUnit.Framework;
using Tower;

[TestFixture]
public class TowerDescriptionTest
{
    [Test]
    public void CreateWidthFloorAndCheckThatWidthChanged()
    {
        var towerDesc = GetTowerDescription();

        var floor = FloorType.Width;
        towerDesc.AddFloor(floor);
        towerDesc.Calculate();

        const ushort newWidth = 1;

        Assert.AreEqual(newWidth, towerDesc.Width);
    }

    [Test]
    public void CreateHeightFloorCheckThatHeightChanged()
    {
        var towerDesc = GetTowerDescription();

        var floor = FloorType.Height;
        towerDesc.AddFloor(floor);
        towerDesc.Calculate();

        const ushort newHeight = 1;

        Assert.AreEqual(newHeight, towerDesc.Height);
    }

    [Test]
    public void CreateTurnFloorAndCheckTurnsChanged()
    {
        var towerDesc = GetTowerDescription();
        const ushort additionalTurnsPerFloor = 1;

        var floor = FloorType.AddTurns;
        towerDesc.AddFloor(floor);
        towerDesc.Calculate();
        Assert.AreEqual(additionalTurnsPerFloor, towerDesc.Turns);
    }

    [Test]
    public void CreatePointsPerTileFloorAndCheckPointsPerTileChanged()
    {
        var towerDesc = GetTowerDescription();
        const ushort pointsPerTile = 1;

        var poinsPerTileFloor = FloorType.PointerPerTile;

        towerDesc.AddFloor(poinsPerTileFloor);
        towerDesc.Calculate();

        Assert.AreEqual(pointsPerTile, towerDesc.PointsPerTile);
    }

    [Test]
    public void DoublePointsFloorAddOnlyOneTime()
    {
        var towerDesc = GetTowerDescription();

        var doubleCoinsFloor = FloorType.DoubleCoins;
        towerDesc.AddFloor(doubleCoinsFloor);
        towerDesc.Calculate();

        Assert.That(towerDesc.IsCoinsDoubled);
    }

    [Test]
    public void DecreaseOfWinPointsFloor()
    {
        //with floor
        var description = GetTowerDescription();
        var expectedWPC = 99;
        description.WinPoinstCoeffiecientWithFloor = expectedWPC;

        var decreaseWinPointsFloor = FloorType.DecreaseWinPoints;
        description.AddFloor(decreaseWinPointsFloor);
        description.Calculate();

        Assert.AreEqual(expectedWPC, description.WinPoinstCoeffiecientWithFloor);

        //without
        description = GetTowerDescription();
        expectedWPC = 100;
        description.WinPoinstCoeffiecientWithOutFloor = expectedWPC;

        description.Calculate();

        Assert.AreEqual(expectedWPC, description.WinPoinstCoeffiecientWithFloor);
    }

    private TowerDescription GetTowerDescription()
    {
        return new TowerDescription();
    }
}