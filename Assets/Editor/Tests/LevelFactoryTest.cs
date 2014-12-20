using Assets.Game.Data;
using Assets.Tower;
using NUnit.Framework;
using UnityEngine;
using System.Collections;

[TestFixture]
public class LevelFactoryTest 
{
    [Test]
    public void CreateSimpleLevelFromWidthAndHeight()
    {
        var levelFactory = GetLevelFactory();

        const ushort width = 10;
        const ushort height = 10;

        var desc = levelFactory.Create(width, height);
        Assert.IsNotNull(desc.Condition);
        Assert.IsNotNull(desc.Field);
        Assert.AreEqual(width, desc.Field.Width);
        Assert.AreEqual(height, desc.Field.Height);
    }

    [Test]
    public void CreateDescFromTowerDescriptionAndCheckSize()
    {
        var levelFactory = GetLevelFactory();
        
        const ushort minWidth = 9;
        const ushort minHeight = 10;

        levelFactory.MinFieldHeight = minHeight;
        levelFactory.MinFieldWidth = minWidth;

        const ushort widthFromTowerDesc = 5;
        const ushort heightFromTowerDesc = 6;

        const ushort expectedWidth = 14;
        const ushort expectedHeight = 16;

        var towerDescriptionForLevelWidthxHeight = new TowerDescription
                                                   {
                                                       Width = widthFromTowerDesc,
                                                       Height = heightFromTowerDesc
                                                   };

        var desc = levelFactory.Create(towerDescriptionForLevelWidthxHeight);

        Assert.IsNotNull(desc.Condition);
        Assert.IsNotNull(desc.Field);
        Assert.AreEqual(expectedWidth, desc.Field.Width);
        Assert.AreEqual(expectedHeight, desc.Field.Height);
    }

    [Test]
    public void CreateDescFromTowerDescriptionAndCheckTurns()
    {
        var levelFactory = GetLevelFactory();
        const ushort minTurns = 10;
        const ushort turnsFromTileDescription = 11;
        const ushort expectedAmount = 21;

        levelFactory.MinTurns = minTurns;

        var towerDescription = new TowerDescription()
                               {
                                   Turns = turnsFromTileDescription
                               };

        var desc = levelFactory.Create(towerDescription);

        Assert.AreEqual(expectedAmount, desc.Condition.GameMode.Turns);
    }

    [Test]
    public void CreateDescFromTowerDescriptionAndCheckPointsPerTile()
    {
        var levelFactory = GetLevelFactory();
        const int minPoinsPerTile = 10;
        levelFactory.MinPointsPerTile = minPoinsPerTile;

        const int poinstPerTileFromTowerDesc = 1;

        const int expectedPointsPerTile = 11;

        var towerDesc = new TowerDescription()
                        {
                            PointsPerTile = poinstPerTileFromTowerDesc
                        };

        var desc = levelFactory.Create(towerDesc);

        Assert.AreEqual(expectedPointsPerTile, desc.PointsPerTile);
    }

    [Test]
    public void CreateDoubleCoinsFloorAndCheckThatDescChanged()
    {
        var levelFactory = GetLevelFactory();

        var towerDesc = new TowerDescription()
                        {
                            IsCoinsDoubled = true
                        };

        var desc = levelFactory.Create(towerDesc);

        Assert.That(desc.IsCoinsDoubled);
    }

    [Test]
    public void CheckCalculationOfPointsForWin()
    {
        const int averageTileCoefficient = 4;
        const int width = 5;
        const int height = 6;
        const int turns = 20;
        const int winPointsCoefficient = 2;

        var pointsForWin =  LevelFactory.CalculateTargetScore(width, height, turns, winPointsCoefficient, averageTileCoefficient);

        const int expectedPoints = 4800;
        Assert.AreEqual(expectedPoints, pointsForWin);
    }

    [Test]
    public void CheckTargetScoreFromTowerDesc()
    {
        var levelFactory = GetLevelFactory();
        levelFactory.MinFieldHeight = 0;
        levelFactory.MinFieldWidth = 0;
        levelFactory.MinTurns = 0;
        levelFactory.AverageTileCoefficient = 4;

        var towerDesc = GetTowerDesc();

        towerDesc.Height = 6;
        towerDesc.Width = 5;
        towerDesc.Turns = 20;
        towerDesc.WinPointsСoefficient = 2;

        var desc = levelFactory.Create(towerDesc);

        const int expectedPoints = 4800;

        Assert.AreEqual(expectedPoints, desc.Condition.GameMode.TargetScore);

    }

    private TowerDescription GetTowerDesc()
    {
        return new TowerDescription();
    }

    private static LevelFactory GetLevelFactory()
    {
        return new LevelFactory();
    }
}