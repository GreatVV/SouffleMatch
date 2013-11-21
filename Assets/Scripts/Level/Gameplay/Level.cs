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

[RequireComponent(typeof(Gamefield))]
public class Level : MonoBehaviour
{
    #region Set in editor

    public CellSprite[] CellPrefabs;

    public GameObject[] ChuzzlePrefabs;
    public GameObject[] ChuzzleLockPrefabs;
    public GameObject CounterPrefab;
  
    public GameObject PlacePrefab;

    [HideInInspector]
    public Gamefield Gamefield;

    #endregion

    public List<Cell> ActiveCells = new List<Cell>();
    public List<Chuzzle> ActiveChuzzles = new List<Chuzzle>();

    public List<GameObject> CellSprites = new List<GameObject>();
    public List<Cell> Cells = new List<Cell>();
    public List<Chuzzle> Chuzzles = new List<Chuzzle>();
    public int CurrentMaxY;
    public int CurrentMinY;
    public int Height = 6;
    public int NumberOfColors = 6;
    public SerializedLevel Serialized;
    public int Width = 6;

    void Awake()
    {
        Gamefield = GetComponent<Gamefield>();
    }

    public void InitRandom()
    {   
        var yStart = 0;
        var yEnd = Height;
        var xStart = 0;
        var xEnd = Width;

        var wDiff = 8 - Width;
        var hDiff = 16 - Height;
        yStart -= hDiff/2;
        yEnd += hDiff/2;
        xStart -= wDiff/2;
        xEnd += wDiff/2;  

        for (var y = yStart; y < yEnd; y++)
        {              
            for (var x = xStart; x < xEnd; x++)
            {
                if (x >= 0 && x < Width && y >= 0 && y < Height)
                {
                    if (GetCellAt(x, y).Type == CellTypes.Usual)
                    {
                        if (x == 2 && y == 2)// coordinates of lock chuzzle
                        {
                            CreateLockChuzzle(x, y);
                        }
                        else
                        {
                            CreateRandomChuzzle(x, y);
                        }
                    }
                }
                else
                {
                    var cellPrefab = GetCellSpritePrefab(x, y, CellTypes.Block);
                    var cellSprite = NGUITools.AddChild(Gamefield.gameObject, cellPrefab);
                    CellSprites.Add(cellSprite);
                    cellSprite.transform.position = GamefieldUtility.ConvertXYToPosition(x, y, Vector3.one);
                }
            }
        }
    }

    public Chuzzle At(int x, int y)
    {
        return Chuzzles.FirstOrDefault(c => c.Current.x == x && c.Current.y == y);
    }

    private GameObject GetCellSpritePrefab(int x, int y, CellTypes cellType)
    {
        var prefabs = CellPrefabs.Where(c => c.Type == cellType).ToArray();
        if (cellType == CellTypes.Block)
        {
            
            return prefabs[(Math.Abs(x) + Math.Abs(y))%2].CellPrefab;
        }

        return prefabs.First().CellPrefab;
    }

    private void CreateTileSprite(Cell cell)
    {
        var prefab = GetCellSpritePrefab(cell.x, cell.y, cell.Type);
        var cellSprite = NGUITools.AddChild(Gamefield.gameObject, prefab);
        CellSprites.Add(cellSprite);
        cellSprite.transform.position = GamefieldUtility.ConvertXYToPosition(cell.x, cell.y, Vector3.one);
     
        cell.Sprite = cellSprite;

        if (cell.HasPlace)
        {
            var place = NGUITools.AddChild(cellSprite, PlacePrefab);
            place.transform.localPosition = Vector3.zero;    
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

    public Chuzzle CreateLockChuzzle(int x, int y, bool toActive = false)
    {
        var colorsNumber = NumberOfColors == -1 ? ChuzzlePrefabs.Length : NumberOfColors;
        var prefab = ChuzzleLockPrefabs[Random.Range(0, colorsNumber)];
        Chuzzle c = CreateChuzzle(x, y, prefab, toActive);
        return c;
    }

    public Chuzzle CreateChuzzle(int x, int y, GameObject prefab, bool toActive = false)
    {
        var gameObject = NGUITools.AddChild(Gamefield.gameObject, prefab);
        gameObject.layer = prefab.layer;                                  
    
        var chuzzle = gameObject.GetComponent<Chuzzle>();
        chuzzle.Real = chuzzle.MoveTo = chuzzle.Current = GetCellAt(x, y);

        gameObject.transform.parent = Gamefield.transform;
        gameObject.transform.position = new Vector3(
            x*chuzzle.Scale.x,
            y*chuzzle.Scale.y, 
            0);

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