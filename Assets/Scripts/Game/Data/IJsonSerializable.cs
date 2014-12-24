using Game.Utility;

namespace Game.Data
{
    public interface IJsonSerializable
    {
        JSONObject Serialize();
        void Deserialize(JSONObject json);
    }
}