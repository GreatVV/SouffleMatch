using System;
using Assets.Game.Gameplay.Cells;
using Assets.Game.Utility;

namespace Assets.Game.Data
{
    [Serializable]
    public class CellDescription
    {
        public CreationType CreationType;
        public int X;
        public int Y;
        public CellTypes Type;
        public bool IsPlace;

        public CellDescription()
        {
            
        }

        public CellDescription(int x, int y)
        {
            X = x;
            Y = y;
        }

        public CellDescription(int x, int y, CellTypes type) : this (x,y)
        {
            Type = type;
        }

        public CellDescription(int x, int y, CellTypes type, CreationType creationType) : this(x,y,type)
        {
            CreationType = creationType;
        }

        public JSONObject Serialize()
        {
            var json = new JSONObject();
            json.AddField("CreationType",CreationType.ToString());
            json.AddField("Type", Type.ToString());
            json.AddField("X",X);
            json.AddField("Y",Y);
            json.AddField("IsPlace",IsPlace);

            return json;
        }

        public static CellDescription Deserialize(JSONObject jsonObject)
        {
            var cellDesc = new CellDescription
                           {
                               CreationType =
                                   (CreationType) Enum.Parse(typeof (CreationType), jsonObject.GetStringField("CreationType")),
                               Type = (CellTypes) Enum.Parse(typeof (CellTypes), jsonObject.GetStringField("Type")),
                               X = (int) jsonObject.GetField("X").n,
                               Y = (int) jsonObject.GetField("Y").n,
                               IsPlace = jsonObject.GetField("IsPlace").b
                           };
            return cellDesc;
        }
    }
}