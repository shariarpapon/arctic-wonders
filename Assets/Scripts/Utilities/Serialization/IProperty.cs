namespace Arctic.Utilities.Serialization
{
    public interface IProperty
    {
        public string Key { get; set; }
        public object Value { get; set; }
        public System.Type ValueType { get; set; }
    }
}