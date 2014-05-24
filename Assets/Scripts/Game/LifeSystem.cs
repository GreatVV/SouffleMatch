using System;
using UnityEngine;

namespace Game
{
    public class LifeSystem : MonoBehaviour
    {
        public int CurrentLifes;

        public DateTime? LifeSpentDate = null;

        public int RegenarationTime;

        public int MaxLifes;

        public bool IsRegenerating
        {
            get { return CurrentLifes < MaxLifes; }
        }

        public int Lifes
        {
            get { return CurrentLifes; }
            private set
            {
                CurrentLifes = value;
                if (CurrentLifes == MaxLifes)
                {
                    LifeSpentDate = null;
                }
                InvokeLifesChanged();
            }
        }

        public bool HasLife
        {
            get { return Lifes > 0; }
        }

        #region Events

        public event Action<int> LifesChanged;

        #endregion

        #region Event Invokators

        protected virtual void InvokeLifesChanged()
        {
            var handler = LifesChanged;
            if (handler != null) handler(Lifes);
        }

        #endregion

        public bool SpentLife(int amount = 1)
        {
            if (amount <= Lifes)
            {
                if (!LifeSpentDate.HasValue)
                {
                    LifeSpentDate = DateTime.UtcNow;
                }
                Lifes -= amount;
                return true;
            }
            return false;
        }

        public void AddLife(int amount = 1)
        {
            Lifes += amount;
        }

        public JSONObject Serialize()
        {
            var jsonObject = new JSONObject(JSONObject.Type.OBJECT);
            jsonObject.AddField("Lifes", Lifes);
            //todo serialize
            return jsonObject;
        }

        public static LifeSystem Unserialize(JSONObject jsonObject)
        {
            if (Instance == null)
            {
                var lifeSystem = new GameObject();
                lifeSystem.AddComponent<LifeSystem>();
            }
        
            Instance.Lifes = jsonObject.GetField("Lifes", new JSONObject("0")).integer;
            return Instance;
        
        }

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
            }

            Instance = this;
        }

        public static LifeSystem Instance { get; set; }

        public void Update()
        {
            if (IsRegenerating)
            {
                if (!LifeSpentDate.HasValue)
                {
                    LifeSpentDate = DateTime.FromFileTime(0);
                }
                if (DateTime.UtcNow > LifeSpentDate + TimeSpan.FromSeconds(RegenarationTime))
                {
                    AddLife();
                    LifeSpentDate += TimeSpan.FromSeconds(RegenarationTime);
                }
            }
        }
    }
}