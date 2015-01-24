using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace Tower
{
    public class Tower : MonoBehaviour, IScrollHandler, IDragHandler
    {
        [SerializeField]
        private List<Floor> _floors = new List<Floor>();

        public GameObject Ground;

        public IEnumerable<Floor> Floors
        {
            get
            {
                return _floors;
            }
        }

        #region IDragHandler Members

        public void OnDrag(PointerEventData eventData)
        {
            var position = Camera.main.transform.position;
            position.y += eventData.delta.y * Time.deltaTime;
            position.y = Mathf.Clamp(position.y, 0, _floors.Last().transform.position.y);
            Camera.main.transform.position = position;
        }

        #endregion

        #region IScrollHandler Members

        public void OnScroll(PointerEventData data)
        {
            var delta = data.scrollDelta;
            // Down is positive for scroll events, while in UI system up is positive.
            delta.y *= -1;

            var position = Camera.main.transform.position;
            position.y += delta.y;

            position.y = Mathf.Clamp(position.y, 0, _floors.Last().transform.position.y);
            Camera.main.transform.position = position;
        }

        #endregion

        public void Start()
        {
            AddFloor(FloorType.First);
        }

        public TowerDescription GetTowerDescription()
        {
            var towerDesc = new TowerDescription();
            foreach (var floor in _floors)
            {
                towerDesc.AddFloor(floor.FloorType);
            }
            towerDesc.Calculate();
            return towerDesc;
        }

        public void AddFloor(FloorType floorType)
        {
            AddFloor(Instance.FloorFactory.CreateFloor(floorType));
        }

        private float CountTowerHeight()
        {
            return _floors.Sum(x => x.transform.GetComponentInChildren<BoxCollider2D>().size.y);
        }

        public void AddFloor(Floor floor)
        {
            if (!floor)
            {
                Debug.LogError("Floor is null");
            }

            floor.transform.SetParent(transform, false);
            floor.transform.localPosition = new Vector3(0, CountTowerHeight(), 0);
            _floors.Add(floor);
        }

        public void AddFloor(string floorName)
        {
            AddFloor((FloorType) Enum.Parse(typeof (FloorType), floorName));
        }
    }
}