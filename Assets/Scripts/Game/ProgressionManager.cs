using System;
using System.Collections.Generic;
using System.Linq;
using Game.Data;
using UnityEngine;
using Utils;
using Object = UnityEngine.Object;

namespace Game
{
    [ExecuteInEditMode]
    public class ProgressionManager : MonoBehaviour
    {
        private int _mana;

        public event Action<int> ManaChanged;

        public int Mana
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

        public List<LevelPackStatus> PackStatuses;

        void Awake()
        {
            PackStatuses = new List<LevelPackStatus>();
        }
        void Start()
        {
            LoadGameProgress();
        }

        private void LoadGameProgress()
        {
            var gameStatus = PlayerPrefs.GetString(Instance.Profile.CurrentPrefix+"gameStatus", null);

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

        public void SaveProgress()
        {
            var json = new JSONObject();
            json.AddField("mana", Mana);
            json.AddField("packStatuses", SerializePackStatuses());
            PlayerPrefs.SetString(Instance.Profile.CurrentPrefix+"gameStatus", json.ToString());
            Debug.Log("Saved progress: "+json.ToString());
            PlayerPrefs.Save();
        }
        
        private JSONObject SerializePackStatuses()
        {
            var json = new JSONObject(JSONObject.Type.ARRAY);
            foreach (var status in PackStatuses)
            {
                json.Add(status.Serialize());
            }
            return json;
        }

        public void RegisterLevelFinish(int packId, int levelId, int score, int turns, bool isWin)
        {
            var pack = GetPackStatusById(packId);
            var level = pack.GetLevelById(levelId);
            level.Register(score, turns, isWin);
        }

        private LevelPackStatus GetPackStatusById(int packId)
        {
            var pack = PackStatuses.FirstOrDefault(x => x.PackId == packId);
            if (pack == null)
            {
               // Debug.LogWarning("Can't find pack with id: "+packId);
                var newPack = new LevelPackStatus()
                {
                    PackId = packId
                };
                PackStatuses.Add(newPack);
                return newPack;
            }
            return pack;
        }

        public void Init()
        {
            var points = Object.FindObjectOfType<ManaManager>();
            if (points)
            {
                points.PointChangeDelta -= OnPointChangeDelta;
                points.PointChangeDelta += OnPointChangeDelta;
            }
        }

        private void OnPointChangeDelta(int delta)
        {
            //Debug.Log("Changed: "+delta);
            Mana += delta;
        }
    }
}