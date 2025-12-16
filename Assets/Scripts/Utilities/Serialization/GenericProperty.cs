namespace Arctic.Utilities.Serialization
{
    [System.Serializable]
    public class GenericProperty<TValue> : IProperty
    {
        public string key;
        public TValue value;

        public GenericProperty() { }

        public GenericProperty(IProperty data)
        {
            this.key = data.GetKey();
            this.value = (TValue)data.GetValue();
        }

        public GenericProperty(string key)
        {
            this.key = key;
        }

        public GenericProperty(string key, TValue value)
        {
            this.key = key;
            this.value = value;
        }

        public string GetKey() => key;
        public object GetValue() => value;
        public System.Type GetValueType() => typeof(TValue);

        public void SetValue(TValue value) => this.value = value;
        public void SetKey(string key) => this.key = key;
        public void SetValue(object value) => this.value = (TValue)value;
    }
}