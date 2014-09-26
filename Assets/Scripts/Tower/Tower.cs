using System;
using System.Collections.Generic;
using System.Linq;
using Game.Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Tower
{
    public class Tower : MonoBehaviour, IJsonSerializable
    {
        public Sprite TestSprite;
        [SerializeField] private List<Floor> _floors = new List<Floor>();

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
            for (int i = 0; i < 10; i++)
            {
                var floorDesc = new FloorDesc
                {
                    Visual = new VisualFloorDesc
                    {
                        sprite = TestSprite,
                        heightInUnits = Random.Range(1, 3),
                        widthInUnits = Random.Range(5, 10)
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
                towerDesc.ProcessFloor(floor);
            }
            return towerDesc;
        }

        public void AddFloor(FloorDesc floorDesc)
        {
            var go = new GameObject("Floor " + _floors.Count, typeof (Floor));
            go.transform.parent = transform;
            go.transform.localPosition = new Vector3(0, CountTowerHeight(), 0);
            go.transform.localScale = new Vector3(floorDesc.Visual.widthInUnits,
                floorDesc.Visual.heightInUnits, 0);
            var floor = go.GetComponent<Floor>();
            floor.Init(floorDesc);
            _floors.Add(floor);
        }

        private float CountTowerHeight()
        {
            return _floors.Sum(x => x.transform.localScale.y);
        }
    }
}