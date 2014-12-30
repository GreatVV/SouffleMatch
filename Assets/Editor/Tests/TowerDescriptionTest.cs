using System.Reflection.Emit;
using NUnit.Framework;
using Tower;
using Tower.Floors;

[TestFixture]
public class TowerDescriptionTest
{
    [Test]
    public void CreateWidthFloorAndCheckThatWidthChanged()
    {
        var towerDesc = GetTowerDescription();

        var floor = new WidthFloorDesc();
        towerDesc.AddFloor(floor);
        towerDesc.Calculate();

        const ushort newWidth = 1;

        Assert.AreEqual(newWidth, towerDesc.Width);
    }

    [Test]
    public void CreateHeightFloorCheckThatHeightChanged()
    {
        var towerDesc = GetTowerDescription();

        var floor = new HeightFloorDesc();
        towerDesc.AddFloor(floor);
        towerDesc.Calculate();

        const ushort newHeight = 1;

        Assert.AreEqual(newHeight, towerDesc.Height);
    }

    [Test]
    public void CreateTurnFloorAndCheckTurnsChanged()
    {
        var towerDesc = GetTowerDescription();
        const ushort additionalTurnsPerFloor = 20;

        var floor = new TurnsFloorDesc()
                    {
                        AddTurns = additionalTurnsPerFloor
                    };
        towerDesc.AddFloor(floor);
        towerDesc.Calculate();
        Assert.AreEqual(additionalTurnsPerFloor, towerDesc.Turns);
    }

    [Test]
    public void CreatePointsPerTileFloorAndCheckPointsPerTileChanged()
    {
        var towerDesc = GetTowerDescription();
        const ushort pointsPerTile = 1;

        var poinsPerTileFloor = new PointsPerTileFloorDesc()
                                {
                                    PointsPerTile = pointsPerTile
                                };

        towerDesc.AddFloor(poinsPerTileFloor);
        towerDesc.Calculate();

        Assert.AreEqual(pointsPerTile, towerDesc.PointsPerTile);

    }

    [Test]
    public void DoublePointsFloorAddOnlyOneTime()
    {
        var towerDesc = GetTowerDescription();

        var doubleCoinsFloor = new DoubleCoinsFloorDesc();
        towerDesc.AddFloor(doubleCoinsFloor);
        towerDesc.Calculate();

        Assert.That(towerDesc.IsCoinsDoubled);
    }

    [Test]
    public void DecreaseOfWinPointsFloor()
    {
        //with floor
        var towerDesc = GetTowerDescription();
        var expectedWinPoints—oefficient= 99;
        towerDesc.WinPoinstCoeffiecientWithFloor = expectedWinPoints—oefficient;
        
        var decreaseWinPointsFloor = new DecreaseWinPointsFloorDesc();
        towerDesc.AddFloor(decreaseWinPointsFloor);
        towerDesc.Calculate();

        Assert.AreEqual(expectedWinPoints—oefficient, towerDesc.WinPoints—oefficient);

        //without
        towerDesc = GetTowerDescription();
        expectedWinPoints—oefficient = 100;
        towerDesc.WinPoinstCoeffiecientWithOutFloor = expectedWinPoints—oefficient;

        towerDesc.Calculate();

        Assert.AreEqual(expectedWinPoints—oefficient, towerDesc.WinPoints—oefficient);

    }

    private TowerDescription GetTowerDescription()
    {
        return new TowerDescription();
    }
}