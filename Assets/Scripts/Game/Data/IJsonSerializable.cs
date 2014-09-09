namespace Game.Data
{
    public interface IJsonSerializable
    {
        JSONObject Serialize();
        void Deserialize(JSONObject json);
    }
}