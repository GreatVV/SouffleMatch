using System;
using System.Collections.Generic;

[Serializable]
public class Points
{
    public int CurrentPoints;              
    public event Action<int> PointChanged;

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
            PointChanged(CurrentPoints);
        }
    }

    public void CountForCombinations(List<Chuzzle> combination)
    {
        var newPoints = combination.Count*10;
        AddPoints(newPoints);
    }
}