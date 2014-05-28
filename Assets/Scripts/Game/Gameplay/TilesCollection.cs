using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Data;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class TilesCollection : IJsonSerializable, IEnumerable<Chuzzle>
{
    #region Delegates

    public delegate bool Condition(Chuzzle chuzzle);

    #endregion

    private readonly List<Chuzzle> chuzzles = new List<Chuzzle>();
    public int[] NewTilesInColumns;

    public TilesCollection()
    {
    }

    public TilesCollection(IEnumerable<Chuzzle> chuzzles)
    {
        this.chuzzles = new List<Chuzzle>(chuzzles);
        foreach (Chuzzle chuzzle in chuzzles)
        {
            chuzzle.AnimationStarted += OnAnimationStarted;
            chuzzle.AnimationFinished += OnAnimationFinished;
        }
    }

    public int Count
    {
        get { return chuzzles.Count; }
    }

    public bool IsAnyAnimated
    {
        get { return chuzzles.Any(x => x.IsAnimationStarted); }
    }

    #region IEnumerable<Chuzzle> Members

    public IEnumerator<Chuzzle> GetEnumerator()
    {
        return chuzzles.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    #endregion

    #region IJsonSerializable Members

    public JSONObject Serialize()
    {
        var json = new JSONObject();
        foreach (Chuzzle chuzzle in chuzzles)
        {
            json.Add(TilesFactory.Serialize(chuzzle));
        }
        return json;
    }

    public void Deserialize(JSONObject json)
    {
        if (json.type == JSONObject.Type.ARRAY)
        {
            foreach (JSONObject chuzzleJson in json.list)
            {
                Add(TilesFactory.Instance.CreateChuzzle(chuzzleJson));
            }
        }
        else
        {
            Debug.LogWarning("Json is not array: " + json);
        }
    }

    #endregion

    #region Events

    public event Action AnimationFinished;

    protected virtual void InvokeAnimationFinished()
    {
        Action handler = AnimationFinished;
        if (handler != null) handler();
    }

    public event Action AnimationStarted;

    protected virtual void InvokeAnimationStarted()
    {
        Action handler = AnimationStarted;
        if (handler != null) handler();
    }

    public event Action<Chuzzle> TileDestroyed;

    public void InvokeTileDestroyed(Chuzzle destroyedChuzzle)
    {
        if (TileDestroyed != null)
        {
            TileDestroyed(destroyedChuzzle);
        }
    }

    #endregion

    #region Events Subscribers

    private void OnAnimationFinished(Chuzzle chuzzle)
    {
        if (!IsAnyAnimated)
        {
            InvokeAnimationFinished();
        }
    }

    private void OnAnimationStarted(Chuzzle chuzzle)
    {
        if (!IsAnyAnimated)
        {
            InvokeAnimationStarted();
        }
    }

    public void OnChuzzleDeath(Chuzzle chuzzle)
    {
        chuzzle.Died -= OnChuzzleDeath;

        //remove chuzzle from game logic
        RemoveChuzzle(chuzzle);
    }

    #endregion

    public TilesCollection GetTiles(Func<Chuzzle, bool> condition = null)
    {
        if (condition == null)
        {
            condition = x => true;
        }
        return new TilesCollection(chuzzles.Where(condition));
    }

    public TilesCollection GetVerticalLine(int column)
    {
        return GetTiles(x => x.Current.x == column);
    }

    public TilesCollection GetHorizontalLine(int row)
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
            Debug.LogWarning("Already contains chuzzle: " + chuzzle);
            return;
        }
        chuzzles.Add(chuzzle);
    }

    public void DestroyChuzzles()
    {
        Debug.LogWarning("Remove all chuzzles from collection. Total: " + chuzzles.Count);
        foreach (Chuzzle chuzzle in chuzzles)
        {
            //ChuzzlePool.Instance.Release(chuzzle.Color, chuzzle.GetType(), chuzzle.gameObject);
            Object.Destroy(chuzzle.gameObject);
        }
        chuzzles.Clear();
    }

    public void Remove(Chuzzle chuzzle)
    {
        Debug.Log("Remove chuzzle: " + chuzzle);
        if (!chuzzles.Contains(chuzzle))
        {
            Debug.LogWarning("Chuzzles dont contains chuzzle: " + chuzzle);
            return;
        }
        chuzzles.Remove(chuzzle);
    }

    public void SyncFromMoveTo()
    {
        foreach (Chuzzle chuzzle in chuzzles)
        {
            chuzzle.Real = chuzzle.Current = chuzzle.MoveTo;
        }
    }

    public void RemoveChuzzle(Chuzzle chuzzle, bool invokeEvent = true)
    {
        Remove(chuzzle);

        if (chuzzle.NeedCreateNew)
        {
            if (chuzzle is TwoTimeChuzzle)
            {
                Debug.LogError("Error: Two time chuzzle creation!!");
            }
            NewTilesInColumns[chuzzle.Current.x]++;
        }
        if (invokeEvent)
        {
            InvokeTileDestroyed(chuzzle);
        }
    }

    public void Clear()
    {
        //TODO remove all tiles for logic and return to pool
        DestroyChuzzles();
    }
}