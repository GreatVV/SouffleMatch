using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Data;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class CellCollection : IJsonSerializable, IEnumerable<Cell>
{
    public List<GameObject> CellSprites = new List<GameObject>();
    public List<Cell> Cells = new List<Cell>();
    public int Height;
    public int Width;

    public Transform root;

    #region IJsonSerializable Members

    public JSONObject Serialize()
    {
        return new JSONObject();
    }

    public void Deserialize(JSONObject json)
    {
    }

    #endregion

    private void CreateTileSprite(Cell cell)
    {
        GameObject cellSprite = TilesFactory.Instance.CellSprite(cell);

        cellSprite.transform.parent = root;
        cellSprite.transform.position = cell.Position;

        cell.Sprite = cellSprite;
        CellSprites.Add(cellSprite);
    }

    public Cell GetCellAt(IntVector2 pos, bool createIfNotFound = true)
    {
        return GetCellAt(pos.x, pos.y, createIfNotFound);
    }

    public Cell GetCellAt(int x, int y, bool createIfNotFound = true)
    {
       // Debug.Log("Get at "+x+":"+ y);
        Cell cell = GamefieldUtility.CellAt(Cells, x, y);
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
        Cell left = Cells.FirstOrDefault(c => c.x == x - 1 && c.y == y);
        if (left != null)
        {
            newCell.Left = left;
            left.Right = newCell;
        }

        //set right
        Cell right = Cells.FirstOrDefault(c => c.x == x + 1 && c.y == y);
        if (right != null)
        {
            newCell.Right = right;
            right.Left = newCell;
        }

        //set top
        Cell top = Cells.FirstOrDefault(c => c.x == x && c.y == y + 1);
        if (top != null)
        {
            newCell.Top = top;
            top.Bottom = newCell;
        }

        //set bottom
        Cell bottom = Cells.FirstOrDefault(c => c.x == x && c.y == y - 1);
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
        foreach (GameObject cellSprite in CellSprites)
        {
            Object.Destroy(cellSprite.gameObject);
        }
        CellSprites.Clear();
    }

    public void Init(FieldDescription levelDescription)
    {
        Width = levelDescription.Width;
        Height = levelDescription.Height;

        foreach (Cell newCell in levelDescription.SpecialCells)
        {
            AddCell(newCell.x, newCell.y, newCell.Copy);
        }

        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                GetCellAt(i, j);
            }
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

    IEnumerator<Cell> IEnumerable<Cell>.GetEnumerator()
    {
        return Cells.GetEnumerator();
    }

    public IEnumerator GetEnumerator()
    {
        return Cells.GetEnumerator();
    }
}

[Serializable]
public class CellSprite
{
    public GameObject CellPrefab;
    public CellTypes Type;
}