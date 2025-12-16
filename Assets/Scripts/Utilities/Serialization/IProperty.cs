namespace Arctic.Utilities.Serialization
{
    public interface IProperty
    {
        public string GetKey();
        public object GetValue();
        public System.Type GetValueType();
        
        public void SetKey(string key);
        public void SetValue(object value);

        public T ValueAs<T>() => (T)GetValue();

        public sealed void Copy(IProperty source) 
        {
            this.SetData(source.GetKey(), source.GetValue(), source.GetValueType());
        }

        public sealed void SetData(string key, object value, System.Type type) 
        {
            this.SetKey(key);
            this.SetValue(value);
        }
    }
}