using System.Collections.Generic;
using System.Linq;
using Tower.Floors;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace Tower
{
    public class Tower : MonoBehaviour, IScrollHandler, IDragHandler
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

        #region IDragHandler Members

        public void OnDrag(PointerEventData eventData)
        {
            Vector3 position = Camera.main.transform.position;
            position.y += eventData.delta.y * Time.deltaTime;
            position.y = Mathf.Clamp(position.y, 0, _floors.Last().transform.position.y);
            Camera.main.transform.position = position;
        }

        #endregion

        #region IScrollHandler Members

        public void OnScroll(PointerEventData data)
        {
            Vector2 delta = data.scrollDelta;
            // Down is positive for scroll events, while in UI system up is positive.
            delta.y *= -1;

            Vector3 position = Camera.main.transform.position;
            position.y += delta.y;

            position.y = Mathf.Clamp(position.y, 0, _floors.Last().transform.position.y);
            Camera.main.transform.position = position;
        }

        #endregion

        public void Start()
        {
            AddFloor(new SimpleFloorDesc());
        }

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
    }
}