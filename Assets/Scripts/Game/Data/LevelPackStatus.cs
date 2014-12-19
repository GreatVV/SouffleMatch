using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Game.Utility;

namespace Assets.Game.Data
{
    public class LevelPackStatus : IJsonSerializable
    {
        public enum PackStatus
        {
            Locked,
            UnlockedByRealMoney,
            UnlockedByMana
        }

        public int PackId;

        public PackStatus Status;

        public List<LevelStatus> LevelStatuses = new List<LevelStatus>();

        public bool IsUnlocked
        {
            get { return Status != PackStatus.Locked; }
        }

        public JSONObject Serialize()
        {
            var j = new JSONObject();
            j.AddField("PackId", PackId);
            j.AddField("Status", Status.ToString());
            j.AddField("LevelStatuses", SerializeLevels());
            return j;
        }

        private JSONObject SerializeLevels()
        {
            var j = new JSONObject(JSONObject.Type.ARRAY);
            foreach (var levelStatus in LevelStatuses)
            {
                j.Add(levelStatus.Serialize());
            }
            return j;
        }

        public void Deserialize(JSONObject json)
        {
            PackId = (int) json["PackId"].n;
            Status = (PackStatus) Enum.Parse(typeof (PackStatus), json["Status"].str);
        }

        public LevelStatus GetLevelById(int levelId)
        {
            var level =  LevelStatuses.FirstOrDefault(x => x.LevelId == levelId);
            if (level == null)
            {
                level = new LevelStatus() {LevelId = levelId};
                LevelStatuses.Add(level);
            }
            return level;
        }
    }
}