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

        public ItemProperty(ItemPropertyData data) 
        {
            this.key = data.key;
            this.value = (TValue)data.value;
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
        public System.Type GetValueType() => typeof(TValue);

        public void SetValue(TValue value) => this.value = value;
        public void SetKey(string key) => this.key = key;
        public void SetValue(object value) => this.value = (TValue)value;
        public void SetValueType(Type type) => this.type = type;
    }
}