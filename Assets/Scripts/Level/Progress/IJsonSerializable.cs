public interface IJsonSerializable
{
    string Serialize();
    void Deserialize(string json);
}