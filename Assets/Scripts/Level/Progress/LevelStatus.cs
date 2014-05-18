using System;

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
    public string Serialize()
    {
        var jsonObject = new JSONObject();
        jsonObject.AddField("IsUnlocked", IsUnlocked);
        jsonObject.AddField("MinimumTurns",MinimumTurns);
        jsonObject.AddField("BestScore", BestScore);
        jsonObject.AddField("HasWin", HasWin);
        jsonObject.AddField("WinAmount", WinAmount);
        jsonObject.AddField("LoseAmount", LoseAmount);
        return jsonObject.ToString();
    }

    public void Deserialize(string json)
    {
        var j = new JSONObject(json);
        IsUnlocked = Convert.ToBoolean(j["IsUnlocked"]);
        HasWin = Convert.ToBoolean(j["HasWin"]);
        MinimumTurns = Convert.ToInt32(j["MinimumTurns"]);
        BestScore = Convert.ToInt32(j["BestScore"]);
        LoseAmount = Convert.ToInt32(j["LoseAmount"]);
        WinAmount = Convert.ToInt32(j["WinAmount"]);
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