using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Data
{
    public class LevelPackStatus : IJsonSerializable
    {
        public enum PackStatus
        {
            Locked,
            UnlockedByRealMoney,
            UnlockedByMana
        }

        public string PackId;

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
            var j = new JSONObject();
            foreach (var levelStatus in LevelStatuses)
            {
                j.Add(levelStatus.Serialize());
            }
            return j;
        }

        public void Deserialize(JSONObject json)
        {
            PackId = json["PackId"].str;
            Status = (PackStatus) Enum.Parse(typeof (PackStatus), json["Status"].str);
        }

        public LevelStatus GetLevelById(string levelId)
        {
            return LevelStatuses.FirstOrDefault(x => x.LevelId == levelId);
        }
    }
}