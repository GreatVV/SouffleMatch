using System;

[Serializable]
public class LevelInfo
{
    public bool IsCompleted;
    public string Name;
    public int BestScore;
    public int NumberOfAttempts;
    public int Number;

    public JSONObject Serialize()
    {
        var jsonObject = new JSONObject(JSONObject.Type.OBJECT);

        jsonObject.AddField("Name", Name);
        jsonObject.AddField("BestScore", BestScore);
        jsonObject.AddField("NumberOfAttempts", NumberOfAttempts);
        jsonObject.AddField("IsCompleted", IsCompleted);

        return jsonObject;
    }

    public static LevelInfo Unserialize(JSONObject jsonObject)
    {
        return new LevelInfo
        {
            BestScore = (int) jsonObject.GetField("BestScore").n,
            NumberOfAttempts = (int) jsonObject.GetField("NumberOfAttempts").n,
            Name = jsonObject.GetField("Name").str,
            IsCompleted = jsonObject.GetField("IsCompleted").b,
        };
    }

    public static int Comparer(LevelInfo x, LevelInfo y)
    {
        return String.CompareOrdinal(x.Name, y.Name);
    }
}