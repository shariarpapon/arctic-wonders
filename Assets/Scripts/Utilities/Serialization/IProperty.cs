namespace Arctic.Utilities.Serialization
{
    public interface IProperty
    {
        public string GetKey();
        public object GetValue();
        public System.Type GetValueType();
        
        public void SetKey(string key);
        public void SetValue(object value);
        public void SetValueType(System.Type type);

        public T ValueAs<T>() => (T)GetValue();
    }
}