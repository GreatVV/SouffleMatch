using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Points : MonoBehaviour
{
    public int CurrentPoints;              
    public event Action<int, int> PointChanged;
    public event Action<IEnumerable<Chuzzle>, int> PointsForDestroy;

    public int TargetPoints { get; set; }

    protected virtual void InvokePointsForDestroy(IEnumerable<Chuzzle> comb, int pointsForComb)
    {
        Action<IEnumerable<Chuzzle>, int> handler = PointsForDestroy;
        if (handler != null) handler(comb, pointsForComb);
    }

    public void Reset()
    {
        CurrentPoints = 0;
        InvokePointChanged();
    }

    public void AddPoints(int points)
    {
        CurrentPoints += points;
        InvokePointChanged();
    }


    public void InvokePointChanged()
    {
        if (PointChanged != null)
        {
            PointChanged(CurrentPoints, TargetPoints);
        }
    }

    public void CountForCombinations(IEnumerable<Chuzzle> combination)
    {
        var newPoints = combination.Count()*25;
        AddPoints(newPoints);
        InvokePointsForDestroy(combination, newPoints);
    }
}