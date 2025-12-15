namespace Arctic.Utilities.Serialization
{
    public class Output<T>
    {
        public readonly T Object;
        public readonly OutputStatus Status;
        public Output(T outputObject, OutputStatus status) 
        {
            this.Object = outputObject;
            this.Status = status;
        }
    }
}