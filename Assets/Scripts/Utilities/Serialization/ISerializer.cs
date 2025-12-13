namespace Arctic.Utilities.Serialization
{
    public interface ISerializer<TDesrialized, TSerialized> 
    {
        public SerializerOutput<TSerialized> Serialize(TDesrialized input);
        public SerializerOutput<TDesrialized> Deserialize(TSerialized input);
    }
}