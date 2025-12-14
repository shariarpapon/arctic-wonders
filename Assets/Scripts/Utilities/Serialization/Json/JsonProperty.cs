using System;

namespace Arctic.Utilities.Serialization.Json
{
    [System.Serializable] 
    public class JsonProperty : IProperty
    {
        public string Key { get; set; }
        public object Value { get; set; }
        public Type ValueType { get; set; }

        public JsonProperty(string key, object value, System.Type type) 
        {
            Key = key;
            Value = value;
            ValueType = type;
        }

        public T ValueAs<T>() => (T)Value;
    }
}