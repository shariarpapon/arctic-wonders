using System;
using Arctic.Utilities.Serialization;

namespace Arctic.Gameplay.Items
{
    public sealed class SerializableItemProperty : IProperty
    {
        private string key;
        private object value;
        private Type type;

        public SerializableItemProperty(string key, object value, Type type)
        {
            this.key = key; ;
            this.value = value;
            this.type = type;
        }
        public T ValueAs<T>() => (T)value;
        public override string ToString() => $"[{key} : {value.ToString()}]";
        public string GetKey() => key;
        public object GetValue() => value;
        public Type GetValueType() => type;
        public void SetKey(string key) => this.key = key;  
        public void SetValue(object value) => this.value = value;
        public void SetValueType(Type type) => this.type = type;
    }
}