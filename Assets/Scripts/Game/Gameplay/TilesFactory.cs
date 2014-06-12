using System;
using System.Collections.Generic;
using System.Linq;
using Game;
using Game.GameMode;
using UnityEngine;
using Random = UnityEngine.Random;

public class TilesFactory : MonoBehaviour
{
    public static TilesFactory Instance;
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
    public int NumberOfColors;
    public GameObject ShineParticle;

    public GameObject CellSprite(Cell cell)
    {
        GameObject prefab = null;
        CellSprite[] prefabs = CellPrefabs.Where(c => c.Type == cell.Type).ToArray();

        if (cell.Type == CellTypes.Block)
        {
            prefab = prefabs[(Math.Abs(cell.x) + Math.Abs(cell.y))%2].CellPrefab;
        }

        prefab = prefabs.First().CellPrefab;

        var cellSprite = Instantiate(prefab) as GameObject;

        if (cell.CreationType == CreationType.Place)
        {
            GameObject place = NGUITools.AddChild(cellSprite, PlacePrefab);
            place.transform.localPosition = Vector3.zero;
            cell.PlaceSprite = place;
        }

        return cellSprite;
    }

    private void Awake()
    {
        if (Instance)
        {
            Debug.Log("Tiles factory already created");
            return;
        }

        Instance = this;

        foreach (GameObject chuzzlePrefab in ChuzzlePrefabs)
        {
            ChuzzlePool.Instance.RegisterChuzzlePrefab(chuzzlePrefab.GetComponent<Chuzzle>().Color,
                typeof (ColorChuzzle), chuzzlePrefab);
        }

        foreach (GameObject chuzzlePrefab in ChuzzleLockPrefabs)
        {
            ChuzzlePool.Instance.RegisterChuzzlePrefab(chuzzlePrefab.GetComponent<Chuzzle>().Color, typeof (LockChuzzle),
                chuzzlePrefab);
        }

        foreach (GameObject chuzzlePrefab in ChuzzleTwoTimesPrefabs)
        {
            ChuzzlePool.Instance.RegisterChuzzlePrefab(chuzzlePrefab.GetComponent<Chuzzle>().Color,
                typeof (TwoTimeChuzzle), chuzzlePrefab);
        }

        foreach (GameObject chuzzlePrefab in ChuzzleCounterPrefabs)
        {
            ChuzzlePool.Instance.RegisterChuzzlePrefab(chuzzlePrefab.GetComponent<Chuzzle>().Color,
                typeof (CounterChuzzle), chuzzlePrefab);
        }

        foreach (GameObject chuzzlePrefab in HorizontalLineChuzzlePrefabs)
        {
            ChuzzlePool.Instance.RegisterChuzzlePrefab(chuzzlePrefab.GetComponent<Chuzzle>().Color,
                typeof (HorizontalLineChuzzle), chuzzlePrefab);
        }

        foreach (GameObject chuzzlePrefab in VerticalLineChuzzlePrefabs)
        {
            ChuzzlePool.Instance.RegisterChuzzlePrefab(chuzzlePrefab.GetComponent<Chuzzle>().Color,
                typeof (VerticalLineChuzzle), chuzzlePrefab);
        }

        foreach (GameObject chuzzlePrefab in BombChuzzlePrefabs)
        {
            ChuzzlePool.Instance.RegisterChuzzlePrefab(chuzzlePrefab.GetComponent<Chuzzle>().Color, typeof (BombChuzzle),
                chuzzlePrefab);
        }

        ChuzzlePool.Instance.RegisterChuzzlePrefab(InvaderPrefab.GetComponent<Chuzzle>().Color, typeof (InvaderChuzzle),
            InvaderPrefab);
    }

    public Chuzzle CreateRandomChuzzle(Cell cell, bool isUniq)
    {
        int colorsNumber = NumberOfColors == -1 ? ChuzzlePrefabs.Length : NumberOfColors;
        GameObject prefab = isUniq
            ? GetUniqRandomPrefabForCell(cell, new List<GameObject>(ChuzzlePrefabs))
            : ChuzzlePrefabs[Random.Range(0, colorsNumber)];
        return CreateChuzzle(cell, prefab);
    }

    private GameObject GetUniqRandomPrefabForCell(Cell cell, List<GameObject> possiblePrefabs)
    {
        GameObject prefab;
        Cell leftCell = cell.Left;
        Cell rightCell = cell.Right;
        Cell topCell = cell.Top;
        Cell bottomCell = cell.Bottom;

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
            Chuzzle chuzzle = GamefieldUtility.GetChuzzleInCell(cell, Gamefield.Level.Chuzzles);
            if (chuzzle == null) return;
            GameObject possible = possiblePrefabs.FirstOrDefault(x => x.GetComponent<Chuzzle>().Color == chuzzle.Color);
            if (possible != null)
            {
                possiblePrefabs.Remove(possible);
            }
        }
    }

    public Chuzzle CreateLockChuzzle(Cell cell, bool isUniq)
    {
        int colorsNumber = NumberOfColors == -1 ? ChuzzlePrefabs.Length : NumberOfColors;
        GameObject prefab = isUniq
            ? GetUniqRandomPrefabForCell(cell, new List<GameObject>(ChuzzleLockPrefabs))
            : ChuzzleLockPrefabs[Random.Range(0, colorsNumber)];
        Chuzzle c = CreateChuzzle(cell, prefab);
        return c;
    }

    public Chuzzle CreateTwoTimeChuzzle(Cell cell, bool isUniq)
    {
        int colorsNumber = NumberOfColors == -1 ? ChuzzlePrefabs.Length : NumberOfColors;
        GameObject prefab = isUniq
            ? GetUniqRandomPrefabForCell(cell, new List<GameObject>(ChuzzleTwoTimesPrefabs))
            : ChuzzleTwoTimesPrefabs[Random.Range(0, colorsNumber)];
        Chuzzle c = CreateChuzzle(cell, prefab);
        return c;
    }

    public Chuzzle CreateInvader(Cell cell)
    {
        return CreateChuzzle(cell, InvaderPrefab);
    }

    public Chuzzle CreateBomb(Cell cell)
    {
        int colorsNumber = NumberOfColors == -1 ? ChuzzlePrefabs.Length : NumberOfColors;
        GameObject prefab = BombChuzzlePrefabs[Random.Range(0, colorsNumber)];
        Chuzzle ch = CreateChuzzle(cell, prefab);
        return ch;
    }

    public Chuzzle CreateCounterChuzzle(Cell cell, bool isUniq)
    {
        int colorsNumber = NumberOfColors == -1 ? ChuzzlePrefabs.Length : NumberOfColors;

        GameObject prefab = isUniq
            ? GetUniqRandomPrefabForCell(cell, new List<GameObject>(ChuzzleCounterPrefabs))
            : ChuzzleCounterPrefabs[Random.Range(0, colorsNumber)];
        Chuzzle c = CreateChuzzle(cell, prefab);

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
        Type type = prefab.GetComponent<Chuzzle>().GetType();
        var chuzzle = //((GameObject) Instantiate(prefab)).GetComponent<Chuzzle>();
        ChuzzlePool.Instance.Get(prefab.GetComponent<Chuzzle>().Color, type).GetComponent<Chuzzle>();

        chuzzle.Real = chuzzle.MoveTo = chuzzle.Current = cell;

        if (!chuzzle.Explosion)
        {
            chuzzle.Explosion = Explosion;
        }
        if (!chuzzle.ShineParticlePrefab)
        {
            chuzzle.ShineParticlePrefab = ShineParticle;
        }

        chuzzle.gameObject.transform.parent = Gamefield.transform;
        chuzzle.gameObject.transform.position = cell.Position;

        Gamefield.Level.Chuzzles.Add(chuzzle);

        chuzzle.Died += Gamefield.Level.Chuzzles.OnChuzzleDeath;

        //Debug.Log("Chuzzle created: " + chuzzle);

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
                    cell.CreationType = CreationType.Usual;
                    return CreateRandomChuzzle(cell, isUniq);
                case CreationType.Counter:
                    cell.CreationType = CreationType.Usual;
                    if (Gamefield.GameMode is TargetChuzzleGameMode)
                    {
                        return CreateCounterChuzzle(cell, isUniq);
                    }
                    return CreateRandomChuzzle(cell, isUniq);
                case CreationType.Lock:
                    cell.CreationType = CreationType.Usual;
                    return CreateLockChuzzle(cell, isUniq);
                case CreationType.TwoTimes:
                    cell.CreationType = CreationType.Usual;
                    return CreateTwoTimeChuzzle(cell, isUniq);
                case CreationType.Invader:
                    cell.CreationType = CreationType.Usual;
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

    public Chuzzle CreateChuzzle(JSONObject serialized)
    {
        var x = serialized["x"].integer;
        var y = serialized["y"].integer;
        var color = (ChuzzleColor) Enum.Parse(typeof (ColorChuzzle), serialized["color"].str);
        return CreateChuzzle(new Cell(x, y), color);
    }

    

    public static JSONObject Serialize(Chuzzle chuzzle)
    {
        var tile = new JSONObject();
        tile.AddField("x", chuzzle.Current.x);
        tile.AddField("y", chuzzle.Current.y);
        tile.AddField("color", chuzzle.Color.ToString());
        tile.AddField("type", chuzzle.GetType().ToString());
        return tile;
    }

    public void ReplaceWithRandom(Chuzzle toReplace)
    {
        CreateChuzzle(toReplace.Current);
        toReplace.Destroy(false, false, true);
    }

    public void ReplaceWithColor(Chuzzle toReplace, ChuzzleColor color)
    {
        CreateChuzzle(toReplace.Current, color);
        toReplace.Destroy(false, false, true);
    }

    public void ReplaceWithOtherColor(Chuzzle toReplace)
    {
        ChuzzleColor exceptColor = toReplace.Color;
        List<ChuzzleColor> possibleColors = ((ChuzzleColor[]) Enum.GetValues(typeof (ChuzzleColor))).ToList();
        possibleColors.Remove(exceptColor);

        ReplaceWithColor(toReplace, possibleColors[Random.Range(0, possibleColors.Count)]);
    }
}