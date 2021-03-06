﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Data;
using Game.Gameplay.Cells;
using Game.Utility;
using UnityEngine;
using Utils;
using Object = UnityEngine.Object;

namespace Game.Gameplay
{
    [Serializable]
    public class CellCollection : IEnumerable<Cell>
    {
        public List<Cell> Cells = new List<Cell>();
        public int Height;
        public int Width;

        public Transform root;

        #region IEnumerable<Cell> Members

        IEnumerator<Cell> IEnumerable<Cell>.GetEnumerator()
        {
            return Cells.GetEnumerator();
        }

        public IEnumerator GetEnumerator()
        {
            return Cells.GetEnumerator();
        }

        #endregion

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
                Cell newCell = Instance.TilesFactory.Cell(
                                                          new CellDescription(x, y, CellTypes.Usual, CreationType.Usual));
                AddCell(x, y, newCell);
                return newCell;
            }
            return cell;
        }

        private void AddCell(int x, int y, Cell newCell)
        {
            Cells.Add(newCell);
            //set left
            Cell left = Cells.FirstOrDefault(c => c.X == x - 1 && c.Y == y);
            if (left != null)
            {
                newCell.Left = left;
                left.Right = newCell;
            }

            //set right
            Cell right = Cells.FirstOrDefault(c => c.X == x + 1 && c.Y == y);
            if (right != null)
            {
                newCell.Right = right;
                right.Left = newCell;
            }

            //set top
            Cell top = Cells.FirstOrDefault(c => c.X == x && c.Y == y + 1);
            if (top != null)
            {
                newCell.Top = top;
                top.Bottom = newCell;
            }

            //set bottom
            Cell bottom = Cells.FirstOrDefault(c => c.X == x && c.Y == y - 1);
            if (bottom != null)
            {
                newCell.Bottom = bottom;
                bottom.Top = newCell;
            }

            /*
        if (newCell.y < Height)
        {
            
        }*/
        }

        public void DestroyCells()
        {
            //Debug.Log("Destroy all cells:"+Cells.Count);
            foreach (Cell cell in Cells)
            {
                if (Application.isPlaying)
                {
                    if (cell)
                    {
                        Object.Destroy(cell.gameObject);
                    }
                    else
                    {
                        Debug.Log("Cell is null");
                    }
                }
                else
                {
                    if (cell)
                    {
                        Object.DestroyImmediate(cell.gameObject);
                    }
                    else
                    {
                        Debug.Log("Cell is null in destroyimmediate");
                    }
                }
            }
            Cells.Clear();
        }

        public void Init(FieldDescription levelDescription)
        {
            Width = levelDescription.Width;
            Height = levelDescription.Height;

            foreach (CellDescription cellDescription in levelDescription.SpecialCells)
            {
                Cell cell = Instance.TilesFactory.Cell(cellDescription);
                AddCell(cellDescription.X, cellDescription.Y, cell);
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
    }
}