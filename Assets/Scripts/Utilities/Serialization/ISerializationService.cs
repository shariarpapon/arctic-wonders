namespace Arctic.Utilities.Serialization
{
    public interface ISerializationService<TDesrialized, TSerialized> 
    {
        public SerializerOutput<TSerialized> Serialize(TDesrialized deserialized);
        public SerializerOutput<TDesrialized> Deserialize(TSerialized serialized);
    }
}