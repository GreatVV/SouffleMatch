using System;
using Game.Data;
using UnityEditor;
using UnityEngine;

public enum CreationType
{
    Usual,
    Place,
    Counter,
    Lock,
    TwoTimes,
    Invader
}

[ExecuteInEditMode]
[Serializable]
public class Cell : MonoBehaviour
{
    public CellDescription Description;

    public CellTypes Type
    {
        get { return Description.Type; }
    }

    public int X
    {
        get { return Description.X; }
    }

    public int Y
    {
        get { return Description.Y; }
    }

    public Cell Left;
    public Cell Right;
    public Cell Top;
    public Cell Bottom;

    [NonSerialized]
    public GameObject PlaceSpite;

    public bool IsTemporary
    {
        get { return _isTemporary; }
        set
        {
            _isTemporary = value;
            renderer.enabled = value;
        }
    }

    public CreationType CreationType { get; set; }

    public bool IsPlace
    {
        get { return Description.IsPlace; }
        set
        {
            Description.IsPlace = value;
            PlaceSpite.renderer.enabled = value;
        }
    }

    public Vector3 Position;
    private bool _isTemporary;
    private bool _isPlace;

    public void Init(CellDescription description)
    {
        Description = description;
        Position = GamefieldUtility.ConvertXYToPosition(X, Y, Chuzzle.Scale);
        CreationType = description.CreationType;
        transform.position = Position;
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
        return string.Format("({0},{1}):{2} Temp:{3}", X,Y, Type,IsTemporary);
    }

    public IntVector2 IntVector2Position
    {
        get
        {
            return new IntVector2(X,Y);
        }
    }
}