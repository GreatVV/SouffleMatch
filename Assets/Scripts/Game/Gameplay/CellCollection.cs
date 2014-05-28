using System;
using System.Collections.Generic;
using System.Linq;
using Game.Data;
using UnityEngine;

public class CellCollection : MonoBehaviour, IJsonSerializable
{
    private List<Cell> Cells = new List<Cell>();
    private List<GameObject> CellSprites = new List<GameObject>();

    public JSONObject Serialize()
    {
        return new JSONObject();
    }

    public void Deserialize(JSONObject json)
    {
        
    }

    private void CreateTileSprite(Cell cell)
    {
        var cellSprite = TilesFactory.Instance.CellSprite(cell);

        cellSprite.transform.parent = gameObject.transform;
        cellSprite.transform.position = cell.Position;

        cell.Sprite = cellSprite;
        CellSprites.Add(cellSprite);
    }

    public int Height { get; private set; }
    public int Width {get; private set;}

    public Cell GetCellAt(IntVector2 pos, bool createIfNotFound = true)
    {
        return GetCellAt(pos.x, pos.y, createIfNotFound);
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

    public void DestroyCells()
    {
        Debug.Log("Destroy all cells");
        Cells.Clear();
        foreach (var cellSprite in CellSprites)
        {
            Destroy(cellSprite.gameObject);
        }
        CellSprites.Clear();
    }

    public void Init(FieldDescription levelDescription)
    {
        Width = levelDescription.Width;
        Height = levelDescription.Height;

        foreach (var newCell in levelDescription.SpecialCells)
        {
            AddCell(newCell.x, newCell.y, newCell.Copy);
        }
    }

    public IEnumerable<Cell> GetCells(Func<Cell, bool> func = null)
    {
        if (func == null)
        {
            func = cell => true;
        }
        return Cells.Where(func);
    }
}

[Serializable]
public class CellSprite
{
    public GameObject CellPrefab;
    public CellTypes Type;
}