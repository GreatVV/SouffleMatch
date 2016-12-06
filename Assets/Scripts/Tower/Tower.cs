using System;
using System.Collections.Generic;
using System.Linq;
using Game.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace Tower
{
    public class Tower : MonoBehaviour, IDragHandler
    {
        [SerializeField]
        private List<Floor> _floors = new List<Floor>();

        public FloorDrawer Ground;

        private FloorDrawer roof;

        public IEnumerable<Floor> Floors
        {
            get
            {
                return _floors;
            }
        }

        public FloorDrawer firstFloorPrefab;
        public FloorDrawer lastFloorPrefab;
        private List<FloorType> _floorTypes = new List<FloorType>();
        private const string towerSeriazeConst = "Tower";

        #region IDragHandler Members

        public void OnDrag(PointerEventData eventData)
        {
            var position = Camera.main.transform.position;

            var deltaInWorld = Camera.main.ScreenToWorldPoint(eventData.position) - Camera.main.ScreenToWorldPoint(eventData.position - eventData.delta);
            position.y += deltaInWorld.y;
            var max = roof.transform.position.y + roof.UpperAnchor.y - Camera.main.aspect * Camera.main.orthographicSize;
            position.y = Mathf.Clamp(position.y, 0, max);
            Camera.main.transform.position = position;
        }

        #endregion

        

        void OnDestroy()
        {
            Serialize();
        }

        public void Start()
        {
            Deserialize();
            Create();
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
            _floorTypes.Add(floorType);
        }

        public void Create()
        {
            transform.RemoveChildren();
            transform.position = Ground.transform.position;
            _floors.Clear();

            var firstFloor = Instantiate(firstFloorPrefab);
            DrawFloor(Ground, firstFloor);
            var prev = firstFloor;
            foreach (var floorType in _floorTypes)
            {
                var floor = Instance.FloorFactory.CreateFloor(floorType);
                _floors.Add(floor);

                var floorDrawer = floor.GetComponent<FloorDrawer>();
                DrawFloor(prev, floorDrawer);
                prev = floorDrawer;
            }

            var lastFloor = Instantiate(lastFloorPrefab);
            roof = lastFloor;
            DrawFloor(prev, lastFloor);


            var size = (roof.transform.position + roof.UpperAnchor) - Ground.transform.position;

            var boxCollider = GetComponent<BoxCollider2D>();
            boxCollider.offset = new Vector2(0, size.y / 2f);
            boxCollider.size = new Vector2(10, size.y);

        }

        private void DrawFloor(FloorDrawer prev, FloorDrawer floor)
        {
            floor.transform.SetParent(transform, false);
            floor.transform.position = prev.transform.position + prev.UpperAnchor - floor.BottomAnchor;
        }

        public void AddFloor(string floorName)
        {
            AddFloor((FloorType) Enum.Parse(typeof (FloorType), floorName));
        }

        public void Serialize()
        {
            var towerState = new TowerState();
            towerState.Floors = Floors.Select(x => x.FloorType).ToArray();
            PlayerPrefs.SetString(towerSeriazeConst, JsonUtility.ToJson(towerState));
        }

        public void Deserialize()
        {
            if (!PlayerPrefs.HasKey(towerSeriazeConst))
            {
                return;
            }

            _floorTypes.Clear();
            _floors.Clear();

            var desc = PlayerPrefs.GetString(towerSeriazeConst);
            if (string.IsNullOrEmpty(desc))
            {
                var state = JsonUtility.FromJson<TowerState>(desc);
                foreach (var floorType in state.Floors)
                {
                    AddFloor(floorType);
                }
            }
        }
    }

    [Serializable]
    public class TowerState
    {
        public FloorType[] Floors;
    }
}