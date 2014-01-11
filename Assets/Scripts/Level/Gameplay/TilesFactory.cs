using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class TilesFactory : MonoBehaviour {

    public CellSprite[] CellPrefabs;
    public GameObject[] ChuzzlePrefabs;
    public GameObject[] ChuzzleLockPrefabs;
    public GameObject[] ChuzzleTwoTimesPrefabs;
    public GameObject[] ChuzzleCounterPrefabs;
    public GameObject[] HorizontalLineChuzzlePrefabs;
    public GameObject[] VerticalLineChuzzlePrefabs;
    public GameObject[] BombChuzzlePrefabs;
    public GameObject InvaderPrefab;

    public GameObject PlacePrefab;
    public GameObject Explosion;

    public Gamefield Gamefield;

    public GameObject CellSprite(Cell cell)
    {
        GameObject prefab = null;
        var prefabs = CellPrefabs.Where(c => c.Type == cell.Type).ToArray();

        if (cell.Type == CellTypes.Block)
        {                                    
            prefab = prefabs[(Math.Abs(cell.x) + Math.Abs(cell.y)) % 2].CellPrefab;
        }

        prefab = prefabs.First().CellPrefab;

        var cellSprite = Instantiate(prefab) as GameObject;

        if (cell.CreationType == CreationType.Place)
        {
            var place = NGUITools.AddChild(cellSprite, PlacePrefab);
            place.transform.localPosition = Vector3.zero;
            cell.PlaceSprite = place;
        }

        return cellSprite;
    }

    public static TilesFactory Instance;

    void Awake()
    {
        Instance = this;
    }

    public int NumberOfColors;

    public Chuzzle CreateRandomChuzzle(Cell cell, bool isUniq)
    {
        var colorsNumber = NumberOfColors == -1 ? ChuzzlePrefabs.Length : NumberOfColors;
        var prefab = isUniq ? GetUniqRandomPrefabForCell(cell, new List<GameObject>(ChuzzlePrefabs)) : ChuzzlePrefabs[Random.Range(0, colorsNumber)];
        return CreateChuzzle(cell, prefab);
    }

    private GameObject GetUniqRandomPrefabForCell(Cell cell, List<GameObject> possiblePrefabs)
    {
        GameObject prefab;
        var leftCell = cell.Left;
        var rightCell = cell.Right;
        var topCell = cell.Top;
        var bottomCell = cell.Bottom;

        RemoveColorFromPossible(leftCell, possiblePrefabs);
        RemoveColorFromPossible(rightCell, possiblePrefabs);
        RemoveColorFromPossible(topCell, possiblePrefabs);
        RemoveColorFromPossible(bottomCell, possiblePrefabs);

        return possiblePrefabs[Random.Range(0, possiblePrefabs.Count)];
    }

    private void RemoveColorFromPossible(Cell cell, List<GameObject> possiblePrefabs)
    {
        if (cell != null)
        {
            var chuzzle = GamefieldUtility.GetChuzzleInCell(cell, Gamefield.Level.Chuzzles);
            if (chuzzle == null) return;
            var possible = possiblePrefabs.FirstOrDefault(x => x.GetComponent<Chuzzle>().Color == chuzzle.Color);
            if (possible != null)
            {
                possiblePrefabs.Remove(possible);
            }
        }
    }

    public Chuzzle CreateLockChuzzle(Cell cell, bool isUniq)
    {
        var colorsNumber = NumberOfColors == -1 ? ChuzzlePrefabs.Length : NumberOfColors;
        var prefab = isUniq ? GetUniqRandomPrefabForCell(cell, new List<GameObject>(ChuzzleLockPrefabs)) : ChuzzleLockPrefabs[Random.Range(0, colorsNumber)];
        Chuzzle c = CreateChuzzle(cell, prefab);
        return c;
    }

    public Chuzzle CreateTwoTimeChuzzle(Cell cell, bool isUniq)
    {
        var colorsNumber = NumberOfColors == -1 ? ChuzzlePrefabs.Length : NumberOfColors;
        var prefab = isUniq ? GetUniqRandomPrefabForCell(cell, new List<GameObject>(ChuzzleTwoTimesPrefabs)) : ChuzzleTwoTimesPrefabs[Random.Range(0, colorsNumber)];
        Chuzzle c = CreateChuzzle(cell, prefab);
        return c;
    }

    public Chuzzle CreateInvader(Cell cell)
    {
        return CreateChuzzle(cell, InvaderPrefab);
    }

    public Chuzzle CreateBomb(Cell cell)
    {
        var colorsNumber = NumberOfColors == -1 ? ChuzzlePrefabs.Length : NumberOfColors;
        var prefab = BombChuzzlePrefabs[Random.Range(0, colorsNumber)];
        Chuzzle ch = CreateChuzzle(cell, prefab);
        return ch;
    }


    public Chuzzle CreateCounterChuzzle(Cell cell, bool isUniq)
    {
        var colorsNumber = NumberOfColors == -1 ? ChuzzlePrefabs.Length : NumberOfColors;

        var prefab = isUniq ? GetUniqRandomPrefabForCell(cell, new List<GameObject>(ChuzzleCounterPrefabs)) : ChuzzleCounterPrefabs[Random.Range(0, colorsNumber)];
        var c = CreateChuzzle(cell, prefab);

        var chuzzle = c as CounterChuzzle;
        if (chuzzle == null)
        {
            Debug.LogError("Incorrect prefabs for counters");
        }
        else
        {
            ((TargetChuzzleGameMode) Gamefield.GameMode).UpdateCounter();
        }
        cell.CreationType = CreationType.Usual;

        return c;
    }

    public Chuzzle CreateChuzzle(Cell cell, ChuzzleColor color)
    {
        return CreateChuzzle(cell, PrefabOfColor(color));
    }

    public Chuzzle CreateChuzzle(Cell cell, GameObject prefab)
    {
        var chuzzleObject = Instantiate(prefab) as GameObject;
        var chuzzle = chuzzleObject.GetComponent<Chuzzle>();
        chuzzle.Real = chuzzle.MoveTo = chuzzle.Current = cell;

        if (!chuzzle.Explosion)
        {
            chuzzle.Explosion = Explosion;
        }

        chuzzle.gameObject.transform.parent = Gamefield.transform;
        chuzzle.gameObject.transform.position = cell.Position;

        Gamefield.Level.Chuzzles.Add(chuzzle);
        Gamefield.Level.ActiveChuzzles.Add(chuzzle);

        chuzzle.Died += Gamefield.Level.OnChuzzleDeath;

        return chuzzle;
    }

    public Chuzzle CreateChuzzle(Cell cell, bool isUniq = false)
    {
        if (cell.Type == CellTypes.Usual)
        {
            switch (cell.CreationType)
            {
                case CreationType.Usual:
                case CreationType.Place:
                    return CreateRandomChuzzle(cell, isUniq);
                case CreationType.Counter:
                    if (Gamefield.GameMode is TargetChuzzleGameMode)
                    {
                        return CreateCounterChuzzle(cell, isUniq);
                    }
                    return CreateRandomChuzzle(cell, isUniq);
                case CreationType.Lock:
                    return CreateLockChuzzle(cell, isUniq);
                case CreationType.TwoTimes:
                    return CreateTwoTimeChuzzle(cell,isUniq);
                case CreationType.Invader:
                    return CreateInvader(cell);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        return null;
    }

    public GameObject PrefabOfColor(ChuzzleColor color)
    {
        return ChuzzlePrefabs.FirstOrDefault(x => x.GetComponent<Chuzzle>().Color == color);
    }

}
