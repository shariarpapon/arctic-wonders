namespace Arctic.Utilities.Serialization
{
    public abstract class KeyValueSerializer : ISerializationService<SerializableKeyValue, string>
    {
        public abstract SerializerOutput<string> Serialize(SerializableKeyValue deserialized);
        public abstract SerializerOutput<SerializableKeyValue> Deserialize(string jsonData);
    }
}