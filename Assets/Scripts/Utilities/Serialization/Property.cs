using System;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

namespace Arctic.Utilities.Serialization
{
    [System.Serializable]
    public class Property : IProperty
    {
        public string key;
        public object value;
        public Type type;

        public Property(string key, object value, System.Type type)
        {
            this.key = key;
            this.value = value;
            this.type = type;
        }
        public string GetKey() => key;
        public object GetValue() => value;
        public System.Type GetValueType() => value.GetType();

        public void SetValue(object value) => this.value = value;
        public void SetKey(string key) => this.key = key;
        public void SetValueType(Type type) => this.type = type;
    }
}