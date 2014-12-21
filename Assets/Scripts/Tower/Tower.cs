using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Game.Data;
using Assets.Game.Utility;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace Assets.Tower
{
    public class Tower : MonoBehaviour, IJsonSerializable,IScrollHandler,IDragHandler
    {
        public GameObject FloorPrefab;

        public GameObject Ground;

        [SerializeField]
        private List<Floor> _floors = new List<Floor>();

        #region IJsonSerializable Members

        public JSONObject Serialize()
        {
            throw new NotImplementedException();
        }

        public void Deserialize(JSONObject json)
        {
            throw new NotImplementedException();
        }

        #endregion

        public void Start()
        {
            for (int i = 0; i < 10; i++)
            {
                var floorDesc = new FloorDesc
                                {
                                    Visual = new VisualFloorDesc
                                             {
                                                 heightInUnits = 1,
                                                 widthInUnits = 1
                                             }
                                };
                AddFloor(floorDesc);
            }
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

        public void AddFloor(FloorDesc floorDesc)
        {
            var go = Instantiate(FloorPrefab) as GameObject;
            go.transform.parent = transform;
            go.transform.localPosition = new Vector3(0, CountTowerHeight(), 0);
            go.transform.localScale = new Vector3(floorDesc.Visual.widthInUnits, floorDesc.Visual.heightInUnits, 0);
            var floor = go.GetComponent<Floor>();
            floor.Init(floorDesc);
            floor.SpriteRenderer.color = new Color(Random.value, Random.value, Random.value);
            _floors.Add(floor);
        }

        private float CountTowerHeight()
        {
            return _floors.Sum(x => x.transform.GetComponentInChildren<BoxCollider2D>().size.y);
        }

        public void OnDrag(PointerEventData eventData)
        {
            Debug.Log("Drag: " + eventData.delta);
            var position = Camera.main.transform.position;
            position.y += eventData.delta.y * Time.deltaTime;
            position.y = Mathf.Clamp(position.y, 0, _floors.Last().BoxCollider2D.transform.position.y);
            Camera.main.transform.position = position;
        }

        public void OnScroll(PointerEventData data)
        {
            Vector2 delta = data.scrollDelta;
            // Down is positive for scroll events, while in UI system up is positive.
            delta.y *= -1;

            var position = Camera.main.transform.position;
            position.y += delta.y;

            position.y = Mathf.Clamp(position.y, 0, _floors.Last().BoxCollider2D.transform.position.y);
            Camera.main.transform.position = position;
        }
    }
}