namespace Arctic.Utilities.Serialization
{
    public interface ISerializer<TDesrialized, TSerialized> 
    {
        public SerializerOutput<TSerialized> Serialize(TDesrialized deserialized);
        public SerializerOutput<TDesrialized> Deserialize(TSerialized serialized);
    }
}