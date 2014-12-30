using NUnit.Framework;
using UnityEngine;
using Utils;

[TestFixture]
public class TestBase
{
    protected FloorFactory GetFloorFactory()
    {
        return Instance.FloorFactory;
    }

    protected Tower.Tower GetTower()
    {
        return new GameObject("Tower", typeof(Tower.Tower)).GetComponent<Tower.Tower>();
    }
}