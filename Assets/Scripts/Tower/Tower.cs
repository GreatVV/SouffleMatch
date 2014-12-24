using System;
using System.Collections.Generic;
using System.Linq;
using Game.Data;
using Game.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace Tower
{
    public class Tower : MonoBehaviour, IJsonSerializable,IScrollHandler,IDragHandler
    {
        public GameObject Ground;

        [SerializeField]
        private List<Floor> _floors = new List<Floor>();

        public IEnumerable<Floor> Floors
        {
            get
            {
                return _floors;
            }
        }

        #region IJsonSerializable Members

        public JSONObject Serialize()
        {
            throw new NotImplementedException();
        }

        public void Deserialize(JSONObject json)
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            AddFloor(new SimpleFloorDesc());
        }

        #endregion

        public TowerDescription GetTowerDescription()
        {
            var towerDesc = new TowerDescription();
            foreach (Floor floor in _floors)
            {
                towerDesc.AddFloor(floor.FloorDescription);
            }
            towerDesc.Calculate();
            return towerDesc;
        }

        public void AddFloor(IFloorDesc floorDesc)
        {
            AddFloor(Instance.FloorFactory.CreateFloor(floorDesc));
        }

        private float CountTowerHeight()
        {
            return _floors.Sum(x => x.transform.GetComponentInChildren<BoxCollider2D>().size.y);
        }

        public void OnDrag(PointerEventData eventData)
        {
            var position = Camera.main.transform.position;
            position.y += eventData.delta.y * Time.deltaTime;
            position.y = Mathf.Clamp(position.y, 0, _floors.Last().transform.position.y);
            Camera.main.transform.position = position;
        }

        public void OnScroll(PointerEventData data)
        {
            Vector2 delta = data.scrollDelta;
            // Down is positive for scroll events, while in UI system up is positive.
            delta.y *= -1;

            var position = Camera.main.transform.position;
            position.y += delta.y;

            position.y = Mathf.Clamp(position.y, 0, _floors.Last().transform.position.y);
            Camera.main.transform.position = position;
        }

        public void AddFloor(Floor floor)
        {
            floor.transform.SetParent(transform);
            floor.transform.localPosition = new Vector3(0, CountTowerHeight(), 0);
            _floors.Add(floor);
        }
    }
}