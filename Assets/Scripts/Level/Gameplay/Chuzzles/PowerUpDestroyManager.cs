using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PowerUpDestroyManager : MonoBehaviour
{
    public bool IsInDestroyState;
    private Chuzzle _firstChuzzle;
    public static PowerUpDestroyManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;
    }

    public void Destroy(Chuzzle powerUpChuzzle)
    {
        if (!IsInDestroyState)
        {
             IsInDestroyState = true;
            _firstChuzzle = powerUpChuzzle;
            var destroyedPowerUps = new List<Chuzzle>() { };
            var tilesToDestroy = new List<Chuzzle> {powerUpChuzzle as Chuzzle};
            int previousCount = 0;
            while (tilesToDestroy.Count != previousCount)
            {
                previousCount = tilesToDestroy.Count;

                for (int index = 0; index < previousCount; index++)
                {
                    var chuzzle = tilesToDestroy[index];
                    if (GamefieldUtility.IsPowerUp(chuzzle) && !destroyedPowerUps.Contains(chuzzle))
                    {
                        destroyedPowerUps.Add(chuzzle);
                        if (chuzzle is BombChuzzle)
                        {
                            tilesToDestroy.AddUniqRange(GetSquare(chuzzle.Current.x, chuzzle.Current.y));
                        }
                        else
                        {
                            var rowNumber = chuzzle.Current.y;
                            var row = GetRow(rowNumber).ToArray();

                            var columnNumber = chuzzle.Current.x;
                            var column = GetColumn(columnNumber).ToArray();

                            if (chuzzle is HorizontalLineChuzzle)
                            {
                                //all tiles in row destroyed - destroy column otherwise row
                                tilesToDestroy.AddUniqRange(row.All(tilesToDestroy.Contains) ? column : row);
                            }
                            else
                            {
                                if (chuzzle is VerticalLineChuzzle)
                                {
                                    //all tiles in column destroyed - destroy row otherwise - column
                                    tilesToDestroy.AddUniqRange(column.All(tilesToDestroy.Contains) ? row : column);
                                }
                            }
                        }

                       
                    }
                }
            }

            //TODO sort collection for correct removing
            //var destructionOrder = tilesToDestroy.Select(chuzzle => new KeyValuePair<Chuzzle, Chuzzle>(_firstChuzzle, chuzzle)).ToList();
            //destructionOrder.Sort(ChuzzleValueComparison);

            var destructionOrder = new List<List<Chuzzle>>() { new List<Chuzzle>() {_firstChuzzle}};

            for (int i = 1; i < 20; i++)
            {
                var currentDistance = tilesToDestroy.Where(chuzzle => Math.Abs((chuzzle.Current.Position - _firstChuzzle.Current.Position).magnitude - i) < float.Epsilon).ToList();
                destructionOrder.Add(currentDistance);
            }
            StartCoroutine(DestroyCollection(destructionOrder));
        }
     
    }

    private int ChuzzleValueComparison(KeyValuePair<Chuzzle, Chuzzle> x, KeyValuePair<Chuzzle, Chuzzle> y)
    {
        var difference = (x.Key.Current.Position - x.Value.Current.Position).magnitude -
                         (y.Key.Current.Position - y.Value.Current.Position).magnitude;
        if (difference == 0)
        {
            return 0;
        }

        return difference > 0 ? 1 : -1;
    }

    private IEnumerator DestroyCollection(List<List<Chuzzle>> chuzzleToDestroy)
    {
        foreach (var listChuzzle in chuzzleToDestroy.ToArray())
        {
            foreach (var chuzzle in listChuzzle)
            {
                chuzzle.Destroy(true);
            }
            yield return new WaitForSeconds(0.1f);
        }
        IsInDestroyState = false;
        yield return new WaitForEndOfFrame();
    }

    public static IEnumerable<Chuzzle> GetColumn(int column)
    {
        return Gamefield.Chuzzles.Where(x => x.Real.x == column);
    }

    public static IEnumerable<Chuzzle> GetRow(int row)
    {
        return Gamefield.Chuzzles.Where(x => x.Real.y == row);
    }

    public static IEnumerable<Chuzzle> GetSquare(int posX, int posY)
    {
        return Gamefield.Chuzzles.Where(
            x =>
                (x.Real.x == posX - 1 || x.Real.x == posX + 1 ||
                 x.Real.x == posX) &&
                (x.Real.y == posY - 1 || x.Real.y == posY ||
                 x.Real.y == posY + 1));
    }
}