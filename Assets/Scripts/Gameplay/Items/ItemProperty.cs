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

        public ItemProperty() { }

        public ItemProperty(IProperty data) 
        {
            this.key = data.GetKey();
            this.value = (TValue)data.GetValue();
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
        public Type GetValueType() => typeof(TValue);

        public void SetValue(TValue value) => this.value = value;
        public void SetKey(string key) => this.key = key;
        public void SetValue(object value) => this.value = (TValue)value;
    }
}