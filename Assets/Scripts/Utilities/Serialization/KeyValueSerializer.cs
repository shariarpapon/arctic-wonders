namespace Arctic.Utilities.Serialization
{
    public abstract class KeyValueSerializer : ISerializer<SerializableKeyValueData, string>
    {
        public abstract SerializerOutput<string> Serialize(SerializableKeyValueData deserialized);
        public abstract SerializerOutput<SerializableKeyValueData> Deserialize(string jsonData);
    }
}