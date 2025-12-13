using System;
using UnityEngine;

namespace Arctic.Gameplay.Items
{
    [System.Serializable]
    public class ItemProperty<TValue> : IKeyValueProperty<string, TValue>
    {
        public string Key => key;
        public TValue Value => value;
        public System.Type ValueType => value.GetType();

        [SerializeField] private string key;
        [SerializeField] private TValue value;

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

    }
}