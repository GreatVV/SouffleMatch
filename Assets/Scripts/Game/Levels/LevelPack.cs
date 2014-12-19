using System;
using System.Collections.Generic;
using Assets.Game.Data;
using Assets.Game.Utility;

namespace Assets.Game.Levels
{
    [Serializable]
    public class LevelPack
    {
        public string Name;

        public List<LevelDescription> LoadedLevels = new List<LevelDescription>();

        public static LevelPack Deserialize(JSONObject json)
        {
            var levelDescription = new LevelPack();

            if (json.HasField("Name"))
            {
                levelDescription.Name = json.GetStringField("Name");
            }
            else
            {
                levelDescription.Name = "Default";
            }
            var levelArray = json.GetField("levelArray").list;
            foreach (var level in levelArray)
            {
                levelDescription.LoadedLevels.Add(LevelDescription.FromJson(level));
            }

            return levelDescription;
        }

        public LevelDescription this[int index]
        {
            get
            {
                //   Debug.Log("I:"+index);
                return LoadedLevels[index];
            }
        }

        public JSONObject Serialize()
        {
            var json = new JSONObject();
            json.AddField("Name", Name);
            var array = new JSONObject(JSONObject.Type.ARRAY);
            foreach (var levelDescription in LoadedLevels)
            {
                array.Add(levelDescription.Serialize());
            }

            json.AddField("levelArray", array);
            return json;
        }
    }
}