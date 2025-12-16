using System;

namespace Arctic.Utilities.Serialization
{
    [System.Serializable]
    public sealed class ExplicitProperty : IProperty
    {
        public string key;
        public object value;
        public System.Type type;

        public ExplicitProperty(string key, object value, Type type)
        {
            this.key = key;
            this.value = value;
            this.type = type;
        }
        public string GetKey() => key;
        public object GetValue() => value;
        public System.Type GetValueType() => type;

        public void SetValue(object value) => this.value = value;
        public void SetKey(string key) => this.key = key;
        public T ValueAs<T>() => (T)value;
    }
}