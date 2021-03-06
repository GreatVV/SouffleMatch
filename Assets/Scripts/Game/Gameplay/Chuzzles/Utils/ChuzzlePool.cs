using System;
using System.Collections.Generic;
using System.Linq;
using Game.Gameplay.Chuzzles.PowerUps;
using Game.Gameplay.Chuzzles.Types;
using UnityEngine;
using Utils;
using Object = UnityEngine.Object;

namespace Game.Gameplay.Chuzzles.Utils
{
    [ExecuteInEditMode]
    public class ChuzzlePool : IClearable
    {
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
            RegisterPrefabs();

            var holder = holders.FirstOrDefault(x => x.color == color && x.type == type);
            if (holder == null)
            {
                //Debug.LogError("Not registered prefab for: " + color + " of " + type);
                return null;
            }
            //Debug.Log("get: " + color + " of " + type );
            var freeObjectsOfTypeAndColor = freeObjects.FirstOrDefault(x => x.Key.type == type && x.Key.color == color);

            if (freeObjectsOfTypeAndColor.Value !=null && freeObjectsOfTypeAndColor.Value.Any())
            {
                var gameObject = freeObjectsOfTypeAndColor.Value.First();
                gameObject.SetActive(true);
                freeObjectsOfTypeAndColor.Value.RemoveAt(0);
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
        private bool registered;

        private class Holder
        {
            public ChuzzleColor color;
            public Type type;
            public GameObject prefab;
        }

        public void RegisterPrefabs()
        {
            if (registered)
            {
                return;
            }
            registered = true;

            foreach (GameObject chuzzlePrefab in Instance.TilesFactory.ChuzzlePrefabs)
            {
                RegisterChuzzlePrefab(chuzzlePrefab.GetComponent<Chuzzle>().Color,
                    typeof(ColorChuzzle), chuzzlePrefab);
            }

            foreach (GameObject chuzzlePrefab in Instance.TilesFactory.ChuzzleLockPrefabs)
            {
                RegisterChuzzlePrefab(chuzzlePrefab.GetComponent<Chuzzle>().Color, typeof(LockChuzzle),
                    chuzzlePrefab);
            }

            foreach (GameObject chuzzlePrefab in Instance.TilesFactory.ChuzzleTwoTimesPrefabs)
            {
                RegisterChuzzlePrefab(chuzzlePrefab.GetComponent<Chuzzle>().Color,
                    typeof(TwoTimeChuzzle), chuzzlePrefab);
            }

            foreach (GameObject chuzzlePrefab in Instance.TilesFactory.ChuzzleCounterPrefabs)
            {
                RegisterChuzzlePrefab(chuzzlePrefab.GetComponent<Chuzzle>().Color,
                    typeof(CounterChuzzle), chuzzlePrefab);
            }

            foreach (GameObject chuzzlePrefab in Instance.TilesFactory.HorizontalLineChuzzlePrefabs)
            {
                RegisterChuzzlePrefab(chuzzlePrefab.GetComponent<Chuzzle>().Color,
                    typeof(HorizontalLineChuzzle), chuzzlePrefab);
            }

            foreach (GameObject chuzzlePrefab in Instance.TilesFactory.VerticalLineChuzzlePrefabs)
            {
                RegisterChuzzlePrefab(chuzzlePrefab.GetComponent<Chuzzle>().Color,
                    typeof(VerticalLineChuzzle), chuzzlePrefab);
            }

            foreach (GameObject chuzzlePrefab in Instance.TilesFactory.BombChuzzlePrefabs)
            {
                RegisterChuzzlePrefab(chuzzlePrefab.GetComponent<Chuzzle>().Color, typeof(BombChuzzle),
                    chuzzlePrefab);
            }

            RegisterChuzzlePrefab(Instance.TilesFactory.InvaderPrefab.GetComponent<Chuzzle>().Color, typeof(InvaderChuzzle), Instance.TilesFactory.InvaderPrefab);
        }

        public void Clear()
        {
            foreach (var freeObject in freeObjects)
            {
                for (int index = 0; index < freeObject.Value.Count; index++)
                {
                    var go = freeObject.Value[index];
                    if (!go)
                    {
                        freeObject.Value.RemoveAt(index);
                        index--;
                    }
                }
            }
        }
    }
}