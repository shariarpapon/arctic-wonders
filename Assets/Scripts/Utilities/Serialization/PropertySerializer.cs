namespace Arctic.Utilities.Serialization
{
    public abstract class PropertySerializer<TFormat> : ISerializer<IProperty, TFormat>
    {
        public virtual SerializerOutput<TFormat> Serialize(IProperty property)
        {
            throw new System.NotImplementedException();
        }

        public virtual SerializerOutput<IProperty> Deserialize(TFormat serialized)
        {
            throw new System.NotImplementedException();
        }
    }
}