namespace Arctic.Utilities.Serialization
{
    public interface IProperty
    {
        public string GetKey();
        public object GetValue();
        public System.Type GetValueType();
        
        public void SetKey(string key);
        public void SetValue(object value);

        public T ValueAs<T>() 
        {
            try
            {
                return (T)GetValue();
            }
            catch (System.Exception e)
            {
                throw new System.Exception($"Unable to cast property value<{GetValueType()?.FullName}>to sepcified to type<{typeof(T).FullName}> ### " + e.Message);
            }
        }

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