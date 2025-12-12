namespace Arctic.Utilities.Serialization
{
    public class SerializerOutput<T>
    {
        public readonly T Object;
        public readonly SerializerStatus Status;
        public SerializerOutput(T Object, SerializerStatus status) 
        {
            this.Object = Object;
            this.Status = status;
        }
    }
}