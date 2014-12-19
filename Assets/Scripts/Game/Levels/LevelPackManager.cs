using System;
using System.Collections.Generic;
using Assets.Game.Utility;

namespace Assets.Game.Levels
{
    [Serializable]
    public class LevelPackManager
    {
        public List<LevelPack> Packs = new List<LevelPack>();

        public JSONObject Serialize()
        {
            var json = new JSONObject(JSONObject.Type.ARRAY);
            foreach (var levelPack in Packs)
            {
                json.Add(levelPack.Serialize());
            }
            return json;
        }

        public static LevelPackManager Deserialize(string serialized)
        {
            var levelPackManager = new LevelPackManager();

            var json = new JSONObject(serialized);
            foreach (var jsonObject in json.list)
            {
                levelPackManager.Packs.Add(LevelPack.Deserialize(jsonObject));
            }

            return levelPackManager;
        }
    }
}