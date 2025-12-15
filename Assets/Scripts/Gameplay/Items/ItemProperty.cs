using Arctic.Utilities.Serialization;
using System;
using UnityEngine;

namespace Arctic.Gameplay.Items
{
    [System.Serializable]
    public class ItemProperty<TValue> : IProperty
    {
        [SerializeField] private string key;
        [SerializeField] private TValue value;
        private System.Type type;

        public ItemProperty() { }

        public ItemProperty(IProperty data) 
        {
            this.key = data.GetKey();
            this.value = (TValue)data.GetValue();
            this.type = data.GetType();
        }

        public ItemProperty(string key) 
        {
            this.key = key;
        }

        public ItemProperty(string key, TValue value)
        {
            this.key = key;
            this.value = value;
        }

        public string GetKey() => key;
        public object GetValue() => value;
        public Type GetValueType() => type;

        public void SetValue(TValue value) => this.value = value;
        public void SetKey(string key) => this.key = key;
        public void SetValue(object value) => this.value = (TValue)value;
        public void SetValueType(Type type) => this.type = type;
    }
}