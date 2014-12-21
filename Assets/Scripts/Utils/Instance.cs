using System.Collections.Generic;
using Assets.Game;
using Assets.Game.Gameplay;
using Assets.Game.Gameplay.Chuzzles.Utils;
using Assets.Game.Player;
using UnityEngine;

namespace Assets.Utils
{
    public static class Instance
    {
        private static readonly List<IClearable> Clearables = new List<IClearable>();
        private static Single<SessionRestorer> _sessionRestorer;
        private static Single<Player> _player;
        private static Single<Gamefield> _gamefield;
        private static Single<Profile> _profile;
        private static Single<ProgressionManager> _progressionManager;


        static Instance()
        {
            Find();
        }

        private static void Find()
        {
            //Find Assets
            TilesFactory = SafeLoadAsset<TilesFactory>("TilesFactory");
            ChuzzlePool.RegisterPrefabs();
        }

        public static SessionRestorer SessionRestorer
        {
            get
            {
                if (_sessionRestorer == null)
                {
                    _sessionRestorer = new Single<SessionRestorer>();
                }
                return _sessionRestorer.Get;
            }
        }

        public static Player Player
        {
            get
            {
                if (_player == null)
                {
                    _player = new Single<Player>("Player");
                }
                return _player.Get;
            }
        }

        public static Gamefield Gamefield
        {
            get
            {
                if (_gamefield == null)
                {
                    _gamefield = new Single<Gamefield>("Gamefield");
                }
                return _gamefield.Get;
            }
        }

        public static Profile Profile
        {
            get
            {
                if (_profile == null)
                {
                    _profile = new Single<Profile>("Profile");
                }
                return _profile.Get;
            }
        }

        public static ProgressionManager ProgressionManager
        {
            get
            {
                if (_progressionManager == null)
                {
                    _progressionManager = new Single<ProgressionManager>("ProgressionManager");
                }
                return _progressionManager.Get;
            }
        }

        public static TilesFactory TilesFactory;
        public static ChuzzlePool ChuzzlePool = new ChuzzlePool();

        private static T SafeLoadAsset<T>(string fileName) where T : ScriptableObject
        {
            //     Debug.Log("File name: "+fileName);
            Object resource = Resources.Load(fileName);
            if (resource == null)
            {
                Debug.LogError("Can't find file: " + fileName);
                return default(T);
            }

            var casted = resource as T;
            if (casted == null)
            {
                Debug.LogError("Asset " + fileName + " is not " + typeof (T).Name);
                return default(T);
            }

            /*
            var clearable = casted as IClearable;
            if (clearable != null && !_clearables.Contains(clearable))
            {
                _clearables.Add(clearable);
            }*/

            return casted;
        }

        public static void Clear()
        {
            foreach (IClearable clearable in Clearables)
            {
                if (clearable != null)
                {
                    clearable.Clear();
                }
                else
                {
                    Debug.Log("Clearable is null");
                }
            }
            Clearables.Clear();
            Find();
        }

        private static T SafeFindGameObject<T>(string gameObjectName) where T : MonoBehaviour
        {
            //   Debug.Log("File name: " + gameObjectName);
            GameObject go = GameObject.Find(gameObjectName);
            if (go == null)
            {
                Debug.LogError("Can't find go: " + gameObjectName);
                return default(T);
            }

            var component = go.GetComponent<T>();
            if (component == null)
            {
                Debug.LogError("Can't find component " + typeof (T).Name + " on go: " + go);
            }

            var clearable = component as IClearable;
            if (clearable != null && !Clearables.Contains(clearable))
            {
                Clearables.Add(clearable);
            }

            return component;
        }

        private class Single<T> where T : MonoBehaviour
        {
            private readonly string _name;
            private T _instance;

            private bool _byType;

            public Single()
            {
                _byType = true;
            }

            public Single(string name)
            {
                _name = name;
            }

            public T Get
            {
                get
                {
                    if (!_instance)
                    {
                        _instance = _byType ? Object.FindObjectOfType<T>() : SafeFindGameObject<T>(_name);
                    }
                    var clearable = _instance as IClearable;
                    if (!Clearables.Contains(clearable))
                    {
                        Clearables.Add(clearable);
                    }

                    return _instance;
                }
            }
        }
    }
}