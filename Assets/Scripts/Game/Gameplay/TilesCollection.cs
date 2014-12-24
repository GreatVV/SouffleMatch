﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.Data;
using Game.Gameplay.Cells;
using Game.Gameplay.Chuzzles;
using Game.Gameplay.Chuzzles.Types;
using Game.Utility;
using UnityEngine;
using Utils;
using Object = UnityEngine.Object;

namespace Game.Gameplay
{
    [Serializable]
    public class TilesCollection : IJsonSerializable, IEnumerable<Chuzzle>
    {
        #region Delegates

        public delegate bool Condition(Chuzzle chuzzle);

        #endregion

        [SerializeField]
        private List<Chuzzle> _chuzzles = new List<Chuzzle>();
        public int[] NewTilesInColumns;

        public TilesCollection()
        {
        }

        public TilesCollection(IEnumerable<Chuzzle> chuzzles)
        {
            this._chuzzles = new List<Chuzzle>(chuzzles);
            foreach (Chuzzle chuzzle in chuzzles)
            {
                chuzzle.AnimationStarted += OnAnimationStarted;
                chuzzle.AnimationFinished += OnAnimationFinished;
            }
        }

        public int Count
        {
            get { return _chuzzles.Count; }
        }

        public bool IsAnyAnimated
        {
            get { return _chuzzles.Any(x => x.IsAnimationStarted); }
        }

        public int AnimatedCount
        {
            get { return _chuzzles.Count(x => x.IsAnimationStarted); }
        }

        #region IEnumerable<Chuzzle> Members

        public IEnumerator<Chuzzle> GetEnumerator()
        {
            return _chuzzles.GetEnumerator();
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
            foreach (Chuzzle chuzzle in _chuzzles)
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
                    Add(Instance.TilesFactory.CreateChuzzle(chuzzleJson));
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
            if (chuzzle.IsDead)
            {
                //remove chuzzle from game logic
                RemoveChuzzle(chuzzle);
            }

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
        }

        #endregion

        public TilesCollection GetTiles(Func<Chuzzle, bool> condition = null)
        {
            if (condition == null)
            {
                condition = x => true;
            }
            return new TilesCollection(_chuzzles.Where(condition));
        }

        public TilesCollection GetVerticalLine(int column)
        {
            return GetTiles(x => x.Current.X == column);
        }

        public TilesCollection GetHorizontalLine(int row)
        {
            return GetTiles(x => x.Current.Y == row);
        }

        public Chuzzle GetTile(Condition condition)
        {
            return _chuzzles.FirstOrDefault(x => condition(x));
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
            return GetTile(c => c.Current.X == x && c.Current.Y == y);
        }

        public void Add(Chuzzle chuzzle)
        {
            if (_chuzzles.Contains(chuzzle))
            {
                Debug.LogWarning("Already contains chuzzle: " + chuzzle);
                return;
            }
            chuzzle.AnimationStarted += OnAnimationStarted;
            chuzzle.AnimationFinished += OnAnimationFinished;
            _chuzzles.Add(chuzzle);
        }

        public void DestroyChuzzles()
        {
            //  Debug.LogWarning("Remove all chuzzles from collection. Total: " + _chuzzles.Count);
            foreach (Chuzzle chuzzle in _chuzzles)
            {
                if (chuzzle)
                {
                    chuzzle.AnimationStarted -= OnAnimationStarted;
                    chuzzle.AnimationFinished -= OnAnimationFinished;
                    //ChuzzlePool.Instance.Release(chuzzle.Color, chuzzle.GetType(), chuzzle.gameObject);
                    if (Application.isEditor)
                    {
                        Object.DestroyImmediate(chuzzle.gameObject);
                    }
                    else
                    {
                        Object.Destroy(chuzzle.gameObject);
                    }
                }
            }
            _chuzzles.Clear();
        }

        public void Remove(Chuzzle chuzzle)
        {
            //Debug.Log("Remove chuzzle: " + chuzzle);
            if (!_chuzzles.Contains(chuzzle))
            {
                Debug.LogWarning("Chuzzles dont contains chuzzle: " + chuzzle);
                return;
            }
            chuzzle.AnimationStarted -= OnAnimationStarted;
            chuzzle.AnimationFinished -= OnAnimationFinished;
            _chuzzles.Remove(chuzzle);
        }

        public void SyncFromMoveTo()
        {
            foreach (Chuzzle chuzzle in _chuzzles)
            {
                chuzzle.Real = chuzzle.Current = chuzzle.MoveTo;
            }
        }

        public void RemoveChuzzle(Chuzzle chuzzle, bool invokeEvent = true)
        {
            Remove(chuzzle);
            //Debug.Log("Need create: "+chuzzle.NeedCreateNew);

            if (chuzzle.NeedCreateNew)
            {
                if (chuzzle is TwoTimeChuzzle)
                {
                    Debug.LogError("Error: Two time chuzzle creation!!");
                }
                NewTilesInColumns[chuzzle.Current.X]++;
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

        public void InitInCells(CellCollection cells)
        {
            foreach (Cell cell in cells)
            {
                if (cell.Type != CellTypes.Block)
                {
                    Instance.TilesFactory.CreateRandomChuzzle(cell, true);
                }
            }
        }
    }
}