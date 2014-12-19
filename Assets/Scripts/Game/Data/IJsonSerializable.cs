using Assets.Game.Utility;

namespace Assets.Game.Data
{
    public interface IJsonSerializable
    {
        JSONObject Serialize();
        void Deserialize(JSONObject json);
    }
}