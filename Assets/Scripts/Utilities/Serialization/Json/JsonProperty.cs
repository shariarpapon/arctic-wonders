using System;

namespace Arctic.Utilities.Serialization.Json
{
    public interface IProp 
    {
        public string Guid { get; set; }
        public string Value { get; set; }
        public System.Type ValueType { get; set; }
    }

    [System.Serializable] 
    public class JsonProperty : IProp
    {
        public const string GUID_KEY = "guid";
        public readonly string guid;
        public readonly object value;
        public readonly System.Type type;

        public JsonProperty(string guid, object value, System.Type type) 
        {
            this.guid = guid;
            this.value = value;
            this.type = type;
        }

        public T ValueAs<T>() => (T)value;

        public string Guid { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public string Value { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public Type ValueType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

       
    }
}