namespace Arctic.Utilities.Serialization
{
    public class Output<T>
    {
        public readonly T Object;
        public readonly SerializerStatus Status;
        public Output(T outputObject, SerializerStatus status) 
        {
            this.Object = outputObject;
            this.Status = status;
        }
    }
}