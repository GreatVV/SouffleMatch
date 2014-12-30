using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game.Player
{
    [ExecuteInEditMode]
    public class ProgressionManager : MonoBehaviour
    {
        private int _mana;

        #region Events

        public event Action<int> ManaChanged;

        #endregion

        public int Mana
        {
            get
            {
                return _mana;
            }
            set
            {
                _mana = value;
                if (ManaChanged != null)
                {
                    ManaChanged(_mana);
                }
            }
        }

        public void Init()
        {
            var points = FindObjectOfType<ManaManager>();
            if (points)
            {
                points.PointChangeDelta -= OnPointChangeDelta;
                points.PointChangeDelta += OnPointChangeDelta;
            }
        }

        private void OnPointChangeDelta(int delta)
        {
            //Debug.Log("Changed: "+delta);
            Mana += delta;
        }
    }
}