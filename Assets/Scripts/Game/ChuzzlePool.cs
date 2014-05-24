using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game
{
    public class ChuzzlePool
    {
        private static ChuzzlePool _instance;

        public static ChuzzlePool Instance
        {
            get { return _instance ?? (_instance = new ChuzzlePool()); }
        }

        public void RegisterChuzzlePrefab(ChuzzleColor color, Type type, GameObject prefab)
        {
            if (holders.Any(x => x.color == color && x.type == type))
            {
                Debug.LogWarning("Already registered: "+color+" : "+type);
                return;
            }

            var holder = new Holder()
            {
                color = color,
                type = type,
                prefab = prefab
            };
            holders.Add(holder);
            freeObjects[holder] = new List<GameObject>();
        }

        public GameObject Get(ChuzzleColor color, Type type)
        {
            var holder = holders.FirstOrDefault(x => x.color == color && x.type == type);
            if (holder == null)
            {
                //  Debug.LogError("Not registered prefab for: " + color + " of " + type);
                return null;
            }
            //Debug.Log("get: " + color + " of " + type );
            var first = freeObjects.FirstOrDefault(x => x.Key.type == type && x.Key.color == color);
            if (first.Value !=null && first.Value.Any())
            {
                var gameObject = first.Value.First();
                gameObject.SetActive(true);
                first.Value.RemoveAt(0);
                //Debug.Log("Gameobject: "+gameObject + " of id "+gameObject.GetInstanceID());
                return gameObject;
            }

            return ((GameObject) Object.Instantiate(holder.prefab));
        }

        public void Release(ChuzzleColor color, Type type, GameObject gameObject)
        {
            var holder = holders.FirstOrDefault(x => x.color == color && x.type == type);
            if (holder == null)
            {
                Debug.LogError("Not registered prefab for: " + color + " of " + type);
                return;
            }
            //Debug.Log("release: "+color + " of "+type + " : "+gameObject.name);
            gameObject.SetActive(false);
            freeObjects.FirstOrDefault(x => x.Key.type == type && x.Key.color == color).Value.Add(gameObject);
        }

        private List<Holder> holders = new List<Holder>();

        private Dictionary<Holder, List<GameObject>> freeObjects = new Dictionary<Holder, List<GameObject>>();

        private class Holder
        {
            public ChuzzleColor color;
            public Type type;
            public GameObject prefab;
        }
    }
}