using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[RequireComponent(typeof(Gamefield))]
public class PowerUpDestroyManager : MonoBehaviour
{
    public bool IsInDestroyState;
    private Chuzzle _firstPowerUp;
    public static PowerUpDestroyManager Instance { get; private set; }

    private Gamefield _gamefield;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
            return;
        }
        Instance = this;
        _gamefield = GetComponent<Gamefield>();
    }

    public void PrintCollection(IEnumerable<Chuzzle> chuzzles)
    {
        var m = chuzzles.Aggregate("", (current, chuzzle) => current + (chuzzle + Environment.NewLine));
        Debug.Log(m);
    }

    public void Destroy(IEnumerable<Chuzzle> chuzzlesToDestroy)
    {
        if (!IsInDestroyState)
        {
             IsInDestroyState = true;
             _firstPowerUp = chuzzlesToDestroy.FirstOrDefault(x=>x.IsPowerUp());
            _firstPowerUp.Destroy(true);
            var destroyedPowerUps = new List<Chuzzle>();
            var tilesToDestroy = new List<Chuzzle>();
            tilesToDestroy.AddRange(chuzzlesToDestroy);

            IEnumerable<Chuzzle> powerUps = new List<Chuzzle> {_firstPowerUp};
            do
            {
                foreach (var powerup in powerUps)
                {
                   // Debug.Log("Destroy powerUp: "+powerup);
                    destroyedPowerUps.Add(powerup);

                    if (powerup is BombChuzzle)
                    {
                        var toDestroy = (powerup as IPowerUp).ToDestroy.ToArray();
                     //   Debug.Log("Bomb to destroy: "+toDestroy.Count());
                        PrintCollection(toDestroy);
                        tilesToDestroy.AddUniqRange(toDestroy.Where(x => !x.IsDead));
                    }
                    else
                    {
                        var rowNumber = powerup.Real.y;
                        var row = GetRow(rowNumber).ToArray();
                      //  Debug.Log("Row:");
                      //  PrintCollection(row);

                       // Debug.Log("Column:");
                        var columnNumber = powerup.Real.x;
                        var column = GetColumn(columnNumber).ToArray();
                      //  PrintCollection(column);

                        if (powerup is HorizontalLineChuzzle)
                        {
                            //all tiles in row destroyed - destroy column otherwise row
                            tilesToDestroy.AddUniqRange(row.Any(chuzzle => !tilesToDestroy.Contains(chuzzle))
                                ? row
                                : column);
                        }
                        else
                        {
                            if (powerup is VerticalLineChuzzle)
                            {
                                //all tiles in column destroyed - destroy row otherwise - column
                                tilesToDestroy.AddUniqRange(column.Any(chuzzle => !tilesToDestroy.Contains(chuzzle))
                                    ? column
                                    : row);
                            }
                        }
                    }
                }

                powerUps = tilesToDestroy.Where(x => x.IsPowerUp() && !destroyedPowerUps.Contains(x) && !x.IsDead).ToArray();

            } while (powerUps.Any());

            Debug.Log("TtD: "+tilesToDestroy.Count);
            PrintCollection(tilesToDestroy);
            

            var destructionOrder = new List<List<Chuzzle>>() { new List<Chuzzle>() {_firstPowerUp}};

            //Debug.Log("First chuzzle: "+_firstPowerUp + " Pos: "+_firstPowerUp.Current.Position);
            int i = 1;
            var currentDistance = new List<Chuzzle>();
            do
            {
                currentDistance.Clear();
                foreach (var chuzzle in tilesToDestroy)
                {
                    var magnitude = (chuzzle.Current.Position - _firstPowerUp.Current.Position).magnitude;
                    //Debug.Log("Chuzzle: " + chuzzle + " magnitude: " + magnitude);
                    if (magnitude > i-1 && magnitude <= i)
                    {
                        currentDistance.Add(chuzzle);
                    }
                }
                //Debug.Log("CurrentDistance:" + currentDistance.Count + " for i: "+i);
                
                destructionOrder.Add(new List<Chuzzle>(currentDistance));
               // Debug.Log("Destruction order count: "+destructionOrder.Count);
                i++;
            } while (currentDistance.Any() || i < 10);
            
/*
            foreach (var order in destructionOrder)
            {
                Debug.Log("---: " + order.Count);
                PrintCollection(order);
            }
            Debug.Log("_______");*/

            var s = destructionOrder.Sum(order => order.Count);
            if (s != tilesToDestroy.Count)
            {
                Debug.LogError("Non equal: " + s + " toD: " + tilesToDestroy.Count);
            }
            _gamefield.InvokeCombinationDestroyed(tilesToDestroy);
            StartCoroutine(DestroyCollection(destructionOrder));
        }
     
    }

    private IEnumerator DestroyCollection(List<List<Chuzzle>> chuzzleToDestroy)
    {
        foreach (var listChuzzle in chuzzleToDestroy.ToArray())
        {
            if (listChuzzle.Count == 0)
            {
                continue;
            }

            foreach (var chuzzle in listChuzzle)
            {
                chuzzle.Destroy(true);
            }
            yield return new WaitForSeconds(0.05f);
        }
        IsInDestroyState = false;
        yield return new object();
    }

    public static IEnumerable<Chuzzle> GetColumn(int column)
    {
        return Gamefield.Chuzzles.Where(x => x.Current.x == column);
    }

    public static IEnumerable<Chuzzle> GetRow(int row)
    {
        return Gamefield.Chuzzles.Where(x => x.Current.y == row);
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