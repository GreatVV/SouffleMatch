using System;
using System.Collections.Generic;
using System.Linq;
using Game.Gameplay.Chuzzles;
using UnityEngine;
using Utils;

namespace Game.Player
{
    public class ManaManager : MonoBehaviour
    {
        public int CurrentPoints;

        #region Events

        public event Action<int> PointChangeDelta;
        public event Action<int, int> PointChanged;
        public event Action<IEnumerable<Chuzzle>, int> PointsForDestroy;

        #endregion

        public int TargetPoints { get; set; }

        protected virtual void InvokePointChangeDelta(int delta)
        {
            Action<int> handler = PointChangeDelta;
            if (handler != null)
            {
                handler(delta);
            }
        }

        protected virtual void InvokePointsForDestroy(IEnumerable<Chuzzle> comb, int pointsForComb)
        {
            Action<IEnumerable<Chuzzle>, int> handler = PointsForDestroy;
            if (handler != null)
            {
                handler(comb, pointsForComb);
            }
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
            int newPoints = combination.Count() * Instance.LevelFactory.CurrentLevel.PointsPerTile;
            InvokePointChangeDelta(newPoints);
            AddPoints(newPoints);
            InvokePointsForDestroy(combination, newPoints);
        }
    }
}