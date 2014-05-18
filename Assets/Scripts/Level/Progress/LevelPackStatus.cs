using System;
using System.Collections.Generic;
using System.Linq;

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

    public string Serialize()
    {
        var j = new JSONObject();
        j.AddField("PackId", PackId);
        j.AddField("Status", Status.ToString());
        j.AddField("LevelStatuses", SerializeLevels());
        return j.ToString();
    }

    private string SerializeLevels()
    {
        var j = new JSONObject();
        foreach (var levelStatus in LevelStatuses)
        {
            j.Add(new JSONObject(levelStatus.Serialize()));
        }
        return j.ToString();
    }

    public void Deserialize(string json)
    {
        var j = new JSONObject(json);
        PackId = j["PackId"].str;
        Status = (PackStatus) Enum.Parse(typeof (PackStatus), j["Status"].str);
    }

    public LevelStatus GetLevelById(string levelId)
    {
        return LevelStatuses.FirstOrDefault(x => x.LevelId == levelId);
    }
}