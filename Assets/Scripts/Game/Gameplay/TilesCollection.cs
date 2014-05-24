using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Game.Data;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class TilesCollection : MonoBehaviour, IJsonSerializable
{

    public delegate bool Condition(Chuzzle chuzzle);

    private readonly List<Chuzzle> chuzzles = new List<Chuzzle>();

    public IEnumerable<Chuzzle> GetTiles(Func<Chuzzle,bool> condition = null)
    {
        if (condition == null)
        {
            condition = x=>true;
        }
        return chuzzles.Where(condition);
    }

    public IEnumerable<Chuzzle> GetVerticalLine(int column)
    {
        return GetTiles(x => x.Current.x == column);
    }

    public IEnumerable<Chuzzle> GetHorizontalLine(int row)
    {
        return GetTiles(x => x.Current.y == row);
    }

    public Chuzzle GetTile(Condition condition)
    {
        return chuzzles.FirstOrDefault(x => condition(x));
    }

    public Chuzzle GetTileAt(Cell cell)
    {
        return GetTile(x => x.Current == cell);
    }

    public Chuzzle GetTileAt(IntVector2 position)
    {
        return GetTileAt(position.x, position.y);
    }

    public Chuzzle GetTileAt(int x, int y)
    {
        return GetTile(c => c.Current.x == x && c.Current.y == y);
    }

    public void Add(Chuzzle chuzzle)
    {
        if (chuzzles.Contains(chuzzle))
        {
            Debug.LogWarning("Already contains chuzzle: "+chuzzle);
            return;
        }
        chuzzles.Add(chuzzle);
    }

    public void Clear()
    {
        Debug.LogWarning("Remove all chuzzles from collection. Total: "+chuzzles.Count);
        foreach (var chuzzle in chuzzles)
        {
            //ChuzzlePool.Instance.Release(chuzzle.Color, chuzzle.GetType(), chuzzle.gameObject);
            Destroy(chuzzle.gameObject);
        }
        chuzzles.Clear();
    }

    public JSONObject Serialize()
    {
        var json = new JSONObject();
        foreach (var chuzzle in chuzzles)
        {
            json.Add(TilesFactory.Serialize(chuzzle));
        }
        return json;
    }

    public void Deserialize(JSONObject json)
    {
        if (json.type == JSONObject.Type.ARRAY)
        {
            foreach (var chuzzleJson in json.list)
            {
                Add(TilesFactory.Instance.CreateChuzzle(chuzzleJson));
            }
        }
        else
        {
            Debug.LogWarning("Json is not array: "+json);
        }
    }

    public void Remove(Chuzzle chuzzle)
    {
        Debug.Log("Remove chuzzle: "+chuzzle);
        if (!chuzzles.Contains(chuzzle))
        {
            Debug.LogWarning("Chuzzles dont contains chuzzle: "+chuzzle);
            return;
        }
        chuzzles.Remove(chuzzle);
    }
}