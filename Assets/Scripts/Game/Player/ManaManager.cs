using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Game.Gameplay.Chuzzles;
using UnityEngine;

namespace Assets.Game.Player
{
    public class ManaManager : MonoBehaviour
    {
        public int CurrentPoints;
        public event Action<int> PointChangeDelta;

        protected virtual void InvokePointChangeDelta(int delta)
        {
            Action<int> handler = PointChangeDelta;
            if (handler != null) handler(delta);
        }

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
            Economy.Instance.Add(points);
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
            InvokePointChangeDelta(newPoints);
            AddPoints(newPoints);
            InvokePointsForDestroy(combination, newPoints);
        }
    }
}