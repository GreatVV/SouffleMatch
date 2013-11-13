#region

using System;
using System.Collections.Generic;

#endregion

public class PayWall
{
    public enum Unlock
    {
        Locked,
        Friends,
        Money
    }       

    public int AfterLevel;
    public int UnlockedLevel;

    public bool IsUnlocked
    {
        get { return Unlocked != Unlock.Locked; }
    }

    public Unlock Unlocked;

    public string[] FriendsIds = new string[3];

    public JSONObject Serialize()
    {
        var jsonObject = new JSONObject(JSONObject.Type.OBJECT);

        jsonObject.AddField("Unlocked", Unlocked.ToString());
        jsonObject.AddField("AfterLevel", AfterLevel);
        jsonObject.AddField("UnlockedLevel", UnlockedLevel);
        if (Unlocked == Unlock.Friends)
        {
            var friendsIdsObject = new JSONObject(JSONObject.Type.ARRAY);
            foreach (var friendsId in FriendsIds)
            {
                friendsIdsObject.list.Add(new JSONObject(friendsId));
            }
            jsonObject.AddField("FriendsIds", friendsIdsObject);
        }

        return jsonObject;
    }

    public void Unserialize(JSONObject jsonObject)
    {
        Unlocked = (Unlock)Enum.Parse(typeof(Unlock), jsonObject.GetField("Unlocked").str, true);
        if (Unlocked == Unlock.Friends)
        {
            for (int index = 0; index < jsonObject.GetField("FriendsIds").list.Count; index++)
            {
                var friendId = jsonObject.GetField("FriendsIds").list[index].str;
                FriendsIds[index] = friendId;
            }
        }

        AfterLevel = jsonObject.GetField("AfterLevel").integer;
        UnlockedLevel = jsonObject.GetField("UnlockedLevel").integer; 
    }
}