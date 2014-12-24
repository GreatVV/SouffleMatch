using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Utility
{
    public class ObjectPool
    {
        private static ObjectPool _instance;

        public static ObjectPool Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ObjectPool();
                }
                return _instance;
            }
        }

        public static GameObject Get<T>()
        {
            if (FreeGameObjects[typeof (T)].Any())
            {
                return FreeGameObjects[typeof (T)].First();
            }
            var newGameObject = GameObject.Instantiate(RegisterdPrefabs[typeof (T)]) as GameObject;
            return newGameObject;
        }

        public static void Release<T>(GameObject gameObject)
        {
            gameObject.SetActive(false);
            FreeGameObjects[typeof(T)].Add(gameObject);
        }

        private static readonly Dictionary<Type, GameObject> RegisterdPrefabs = new Dictionary<Type, GameObject>();

        public static void Register(Type type, GameObject prefab)
        {
            if (RegisterdPrefabs.ContainsKey(type))
            {
                Debug.LogWarning(type + " is already registerd");
                return;
            }

            RegisterdPrefabs[type] = prefab;
            FreeGameObjects[type] = new List<GameObject>();
        }

        static readonly Dictionary<Type, List<GameObject>> FreeGameObjects = new Dictionary<Type, List<GameObject>>();

        private ObjectPool()
        {
        
        }
    }
}