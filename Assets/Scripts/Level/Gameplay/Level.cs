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
    [HideInInspector]
    public Gamefield Gamefield;

    public List<Cell> ActiveCells = new List<Cell>();
    public List<Chuzzle> ActiveChuzzles = new List<Chuzzle>();

    public List<GameObject> CellSprites = new List<GameObject>();
    public List<Cell> Cells = new List<Cell>();
    public List<Chuzzle> Chuzzles = new List<Chuzzle>();
    public int CurrentMaxY;
    public int CurrentMinY;
    public int Height = 6;
    public SerializedLevel Serialized;
    public int Width = 6; 

    void Awake()
    {
        Gamefield = GetComponent<Gamefield>();
    }

    public void InitRandom()
    {   
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                TilesFactory.Instance.CreateChuzzle(GetCellAt(x, y));
            }
        }
    }


    public Chuzzle At(int x, int y)
    {
        return Chuzzles.FirstOrDefault(c => c.Current.x == x && c.Current.y == y);
    }

    

    private void CreateTileSprite(Cell cell)
    {
        var cellSprite = TilesFactory.Instance.CellSprite(cell);                                          
        
        cellSprite.transform.parent = gameObject.transform;
        cellSprite.transform.position = GamefieldUtility.ConvertXYToPosition(cell.x, cell.y, Vector3.one);

        cell.Sprite = cellSprite;
        CellSprites.Add(cellSprite);
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
        TilesFactory.Instance.NumberOfColors = level.NumberOfColors;

        Gamefield.GetComponent<Gamefield>().GameMode = GameModeFactory.CreateGameMode(level.GameMode);
        Gamefield.GetComponent<Gamefield>().GameMode.Init(Gamefield.GetComponent<Gamefield>());

        InitRandom();
    }


    public void OnChuzzleDeath(Chuzzle chuzzle)
    {
        chuzzle.Died -= OnChuzzleDeath;

        //remove chuzzle from game logic
        Gamefield.RemoveChuzzle(chuzzle);
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
            Destroy(chuzzle.gameObject);
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