namespace Arctic.Serialization.Properties
{
    [System.Serializable]
    public class Property<TValue> : IProperty
    {
        public string key;
        public TValue value;

        public Property() { }

        public Property(IProperty data)
        {
            this.key = data.GetKey();
            this.value = (TValue)data.GetValue();
        }

        public Property(string key, TValue value)
        {
            this.key = key;
            this.value = value;
        }

        public string GetKey() => key;
        public object GetValue() => value;
        public System.Type GetValueType() => typeof(TValue);
        public void SetValue(object value) => this.value = (TValue)value;


    }
}