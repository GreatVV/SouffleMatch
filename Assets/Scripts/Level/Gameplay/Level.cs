#region

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

#endregion

[Serializable]
public class CellSprite
{
    public GameObject CellPrefab;
    public CellTypes Type;
}

[Serializable]
public class Level
{
    #region Set in editor

    public CellSprite[] CellPrefabs;

    public GameObject[] ChuzzlePrefabs;
    public GameObject CounterPrefab;

    public GameObject Gamefield;
    public GameObject PlacePrefab;

    #endregion

    public List<Cell> ActiveCells = new List<Cell>();
    public List<Chuzzle> ActiveChuzzles = new List<Chuzzle>();

    public List<GameObject> CellSprites = new List<GameObject>();
    public List<Cell> Cells = new List<Cell>();
    public Vector3 ChuzzleSize = new Vector3(80, 80);
    public List<Chuzzle> Chuzzles = new List<Chuzzle>();
    public int CurrentMaxY;
    public int CurrentMinY;
    public int Height = 6;
    public int NumberOfColors = 6;
    public SerializedLevel Serialized;
    public int Width = 6;

    public void InitRandom()
    {
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                if (GetCellAt(x, y).Type == CellTypes.Usual)
                {
                    CreateRandomChuzzle(x, y);
                }
            }
        }
    }

    public Chuzzle At(int x, int y)
    {
        return Chuzzles.FirstOrDefault(c => c.Current.x == x && c.Current.y == y);
    }

    private void CreateTileSprite(Cell cell)
    {
        var prefab = CellPrefabs.First(x => x.Type == cell.Type).CellPrefab;
        var cellSprite = NGUITools.AddChild(Gamefield, prefab);
        CellSprites.Add(cellSprite);
        cellSprite.transform.localPosition = GamefieldUtility.ConvertXYToPosition(cell.x, cell.y, ChuzzleSize);
       /* var sprite = cellSprite as Sprite;
        ScaleSprite(sprite);*/
        cell.GameObject = cellSprite;
        if (cell.HasPlace)
        {
            var place = NGUITools.AddChild(cellSprite, PlacePrefab);
            place.transform.localPosition = Vector3.zero;

     /*       var placesprite = place.GetComponent<Sprite>();
            ScaleSprite(placesprite);*/
        }
    }

    public void InitFromFile(SerializedLevel level)
    {
        Serialized = level;

        Cells.Clear();

        Width = level.Width;
        Height = level.Height;

        CurrentMinY = 0;
        CurrentMaxY = Height;

        Random.seed = level.Seed;

        //BUG change 480 for other resolutiion
        ChuzzleSize = new Vector3(480, 480, 0)/Width;
        if (ChuzzleSize.x > 80)
        {
            ChuzzleSize = new Vector3(80,80,0);
        }
        Debug.Log("Add cells");
        foreach (var newCell in level.SpecialCells)
        {
            AddCell(newCell.x, newCell.y, newCell.Copy);
        }
        NumberOfColors = level.NumberOfColors;

        Gamefield.GetComponent<Gamefield>().GameMode = GameModeFactory.CreateGameMode(level.GameMode);
        Gamefield.GetComponent<Gamefield>().GameMode.Init(Gamefield.GetComponent<Gamefield>());

        InitRandom();
    }

    public Chuzzle CreateRandomChuzzle(int x, int y, bool toActive = false)
    {
        var colorsNumber = NumberOfColors == -1 ? ChuzzlePrefabs.Length : NumberOfColors;
        var prefab = ChuzzlePrefabs[Random.Range(0, colorsNumber)];
        return CreateChuzzle(x, y, prefab, toActive);
    }

    public Chuzzle CreateChuzzle(int x, int y, GameObject prefab, bool toActive = false)
    {
        var gameObject = NGUITools.AddChild(Gamefield.gameObject, prefab);
        gameObject.layer = prefab.layer;

      /*  var sprite = gameObject.GetComponent<Sprite>();
        ScaleSprite(sprite);*/

        ((BoxCollider) gameObject.collider).size = ChuzzleSize;
        ((BoxCollider) gameObject.collider).center = ChuzzleSize/2;
        var chuzzle = gameObject.GetComponent<Chuzzle>();
        chuzzle.Real = chuzzle.MoveTo = chuzzle.Current = GetCellAt(x, y);

        gameObject.transform.parent = Gamefield.transform;
        gameObject.transform.localPosition = new Vector3(x*gameObject.GetComponent<Chuzzle>().Scale.x,
            y*gameObject.GetComponent<Chuzzle>().Scale.y, 0);

        if (chuzzle.Current.HasCounter)
        {
            chuzzle.Counter = ((TargetChuzzleGameMode) Gamefield.GetComponent<Gamefield>().GameMode).Amount;

       /*     var counter = NGUITools.AddChild(gameObject, CounterPrefab).GetComponent<tk2dTextMesh>();
            counter.text = chuzzle.Counter.ToString(CultureInfo.InvariantCulture);*/

            chuzzle.Current.HasCounter = false;
        }
        Chuzzles.Add(chuzzle);
        if (toActive)
        {
            ActiveChuzzles.Add(chuzzle);
        }
        return chuzzle;
    }

    public void ScaleSprite(Sprite sprite)
    {
        var size = ChuzzleSize;
        GamefieldUtility.ScaleSprite(sprite, size);
    }

   

    public Cell GetCellAt(int x, int y, bool createIfNotFound = true)
    {
        var cell = GamefieldUtility.CellAt(Cells, x, y);
        if (cell == null && createIfNotFound)
        {
            var newCell = new Cell(x, y);
            AddCell(x, y, newCell);
            return newCell;
        }
        return cell;
    }

    private void AddCell(int x, int y, Cell newCell)
    {
        Cells.Add(newCell);
        //set left
        var left = Cells.FirstOrDefault(c => c.x == x - 1 && c.y == y);
        if (left != null)
        {
            newCell.Left = left;
            left.Right = newCell;
        }

        //set right
        var right = Cells.FirstOrDefault(c => c.x == x + 1 && c.y == y);
        if (right != null)
        {
            newCell.Right = right;
            right.Left = newCell;
        }

        //set top
        var top = Cells.FirstOrDefault(c => c.x == x && c.y == y + 1);
        if (top != null)
        {
            newCell.Top = top;
            top.Bottom = newCell;
        }

        //set bottom
        var bottom = Cells.FirstOrDefault(c => c.x == x && c.y == y - 1);
        if (bottom != null)
        {
            newCell.Bottom = bottom;
            bottom.Top = newCell;
        }

        if (newCell.y < Height)
        {
            CreateTileSprite(newCell);
        }
    }

    public void Reset()
    {
        foreach (var chuzzle in Chuzzles)
        {
            Object.Destroy(chuzzle.gameObject);
        }
        Chuzzles.Clear();
        ActiveChuzzles.Clear();

        Cells.Clear();
        ActiveCells.Clear();

        foreach (var cellSprite in CellSprites)
        {
            Object.Destroy(cellSprite.gameObject);
        }
        CellSprites.Clear();
    }

    public void ChoseFor(int minY, int maxY)
    {
        CurrentMinY = minY;
        CurrentMaxY = maxY;
        UpdateActive();
    }

    public void UpdateActive()
    {
        ActiveCells = Cells.Where(x => x.y >= CurrentMinY && x.y <= CurrentMaxY).ToList();
        ActiveChuzzles = Chuzzles.Where(x => x.Current.y >= CurrentMinY && x.Current.y <= CurrentMaxY).ToList();
        foreach (var chuzzle in Chuzzles)
        {
            chuzzle.Frozen = !ActiveChuzzles.Contains(chuzzle);
        }
    }
}