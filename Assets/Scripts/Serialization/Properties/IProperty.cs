namespace Arctic.Serialization.Properties
{
    public interface IProperty
    {
        public string GetKey();
        public object GetValue();
        public System.Type GetValueType();
        public void SetValue(object newValue);

        public T GetValueAs<T>() 
        {
            try
            {
                return (T)GetValue();
            }
            catch (System.Exception e)
            {
                throw new System.Exception($"*** Unable to cast property value<{GetValueType()?.FullName}>to sepcified to type<{typeof(T).FullName}> *** " + e.Message);
            }
        }
    }
}