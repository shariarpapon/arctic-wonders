namespace Arctic.Utilities.Serialization
{
    public class SerializerOutput<T>
    {
        public readonly T Object;
        public readonly SerializerStatus Status;
        public SerializerOutput(T outputObject, SerializerStatus status) 
        {
            this.Object = outputObject;
            this.Status = status;
        }
    }
}