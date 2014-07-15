using System;
using System.Collections.Generic;
using System.Linq;
using Game.Data;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Game
{
    public static class ProgressionManager
    {
        private static int _mana;

        public static event Action<int> ManaChanged;

        public static int Mana
        {
            get { return _mana; }
            set
            {
                _mana = value;
                if (ManaChanged != null)
                {
                    ManaChanged(_mana);
                }
            }
        }

        public readonly static List<LevelPackStatus> PackStatuses;

        static ProgressionManager()
        {
            PackStatuses = new List<LevelPackStatus>();
            LoadGameProgress();
        }

        private static void LoadGameProgress()
        {
            var gameStatus = PlayerPrefs.GetString("gameStatus", null);

            if (!string.IsNullOrEmpty(gameStatus))
            {
                var json = new JSONObject(gameStatus);
                Debug.Log("S:"+gameStatus);
                Mana = (int)json["mana"].n;
                
                var packStatuses = json["packStatuses"];
                if (packStatuses.type == JSONObject.Type.ARRAY)
                {
                    foreach (var statusPacked in packStatuses.list)
                    {
                        var status = new LevelPackStatus();
                        status.Deserialize(statusPacked);
                        PackStatuses.Add(status);
                    }
                }
                else
                {
                    Debug.LogWarning("Incorrect packstatuses: " + packStatuses);
                }

                
            }
            

        }

        public static void SaveProgress()
        {
            var json = new JSONObject();
            json.AddField("mana", Mana);
            json.AddField("packStatuses", SerializePackStatuses());
            PlayerPrefs.SetString("gameStatus", json.ToString());
            Debug.Log("Saved progress: "+json);
            PlayerPrefs.Save();
        }
        
        private static string SerializePackStatuses()
        {
            var json = new JSONObject(JSONObject.Type.ARRAY);
            foreach (var status in PackStatuses)
            {
                json.Add(status.Serialize());
            }
            return json.ToString();
        }

        public static void RegisterLevelFinish(int packId, int levelId, int score, int turns, bool isWin)
        {
            var pack = GetPackStatusById(packId);
            var level = pack.GetLevelById(levelId);
            level.Register(score, turns, isWin);
        }

        private static LevelPackStatus GetPackStatusById(int packId)
        {
            var pack = PackStatuses.FirstOrDefault(x => x.PackId == packId);
            if (pack == null)
            {
                Debug.LogWarning("Can't find pack with id: "+packId);
                var newPack = new LevelPackStatus()
                {
                    PackId = packId
                };
                PackStatuses.Add(newPack);
                return newPack;
            }
            return pack;
        }

        public static void Init()
        {
            var points = Object.FindObjectOfType<Points>();
            if (points)
            {
                points.PointChangeDelta -= OnPointChangeDelta;
                points.PointChangeDelta += OnPointChangeDelta;
            }
        }

        private static void OnPointChangeDelta(int delta)
        {
            //Debug.Log("Changed: "+delta);
            Mana += delta;
        }
    }
}