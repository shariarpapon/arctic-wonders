namespace Arctic.Utilities.Serialization
{
    public abstract class PropertySerializer<TFormat> : ISerializer<ISerializableProperty, TFormat>
    {
        public virtual SerializerOutput<TFormat> Serialize(ISerializableProperty property)
        {
            throw new System.NotImplementedException();
        }

        public virtual SerializerOutput<ISerializableProperty> Deserialize(TFormat serialized)
        {
            throw new System.NotImplementedException();
        }
    }
}