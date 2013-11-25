using System;
using UnityEngine;

[Serializable]
public class Cell
{   
    public CellTypes Type;

    public GameObject Sprite;
    public GameObject PlaceSprite;

    public int x;
    public int y;

    public Cell Left;
    public Cell Right;
    public Cell Top;
    public Cell Bottom;

    public bool IsTemporary;

    public bool NeedPlace;
    public bool NeedCounter;
    public bool NeedLock;

    public Cell Copy
    {
        get
        {
            return new Cell(x,y,Type)
            {
                NeedCounter = NeedCounter, 
                NeedPlace = NeedPlace,
                NeedLock = NeedLock
            };
        }
    }

    

    public Cell(int x, int y, CellTypes type = CellTypes.Usual)
    {
        this.x = x;
        this.y = y;
        this.Type = type;             
    }

    public Cell GetBottomWithType(CellTypes type = CellTypes.Usual)
    {
        var bottom = Bottom;
        while (bottom != null)
        {
            if (bottom.Type == type)
            {
                return bottom;
            }
            bottom = bottom.Bottom;
        }
        return null;
    }

    public Cell GetLeftWithType(CellTypes type = CellTypes.Usual)
    {
        var left = Left;
        while (left != null)
        {
            if (left.Type == type )
            {
                return left;
            }
            left = left.Left;
        }
        return null;
    }

    public Cell GetRightWithType(CellTypes type = CellTypes.Usual)
    {
        var right = Right;
        while (right != null)
        {
            //Debug.Log("Right" + right.ToString());
            if (right.Type == type)
            {
                return right;
            }
            right = right.Right;
        }
        return null;
    }

    public Cell GetTopWithType(CellTypes type = CellTypes.Usual)
    {
        var top = Top;
        while (top != null && !top.IsTemporary)
        {
            if (top.Type == type)
            {
                return top;
            }
            top = top.Top;
        }
        return null;
    }

    public override string ToString()
    {
        return string.Format("({0},{1}):{2} Temp:{3}", x,y, Type,IsTemporary);
    }                        
}