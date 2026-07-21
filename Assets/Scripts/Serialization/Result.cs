namespace Arctic.Serialization
{
    public class Result<T>
    {
        public readonly T Object;
        public readonly OutputStatus Status;
        public Result(T outputObject, OutputStatus status) 
        {
            this.Object = outputObject;
            this.Status = status;
        }
    }
}