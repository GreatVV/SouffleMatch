using System;

namespace Game.Data
{
    public class LevelStatus : IJsonSerializable
    {
        public string LevelId;

        public int NumberOfWin;
        public int WinAmount;
        public int LoseAmount;
        public bool IsUnlocked;
        public int MinimumTurns;
        public int BestScore;
        public bool HasWin;
        public JSONObject Serialize()
        {
            var jsonObject = new JSONObject();
            jsonObject.AddField("IsUnlocked", IsUnlocked);
            jsonObject.AddField("MinimumTurns",MinimumTurns);
            jsonObject.AddField("BestScore", BestScore);
            jsonObject.AddField("HasWin", HasWin);
            jsonObject.AddField("WinAmount", WinAmount);
            jsonObject.AddField("LoseAmount", LoseAmount);
            return jsonObject;
        }

        public void Deserialize(JSONObject json)
        {
            IsUnlocked = Convert.ToBoolean(json["IsUnlocked"]);
            HasWin = Convert.ToBoolean(json["HasWin"]);
            MinimumTurns = Convert.ToInt32(json["MinimumTurns"]);
            BestScore = Convert.ToInt32(json["BestScore"]);
            LoseAmount = Convert.ToInt32(json["LoseAmount"]);
            WinAmount = Convert.ToInt32(json["WinAmount"]);
        }

        public void Register(int score, int turns, bool isWin)
        {
            if (BestScore < score)
            {
                BestScore = score;
            }

            if (MinimumTurns > turns)
            {
                MinimumTurns = turns;
            }

            if (isWin)
            {
                WinAmount++;
            }
            else
            {
                LoseAmount++;
            }
            HasWin |= isWin;
        }
    }
}