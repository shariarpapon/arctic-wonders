using UnityEngine;

namespace Arctic.Gameplay.Items
{
    [System.Serializable]
    public class ItemProperty<TValue> : IKeyValueProperty<string, TValue>
    {
        public string Key => key;
        public TValue Value => value;

        [SerializeField] private string key;
        [SerializeField] private TValue value;

        public ItemProperty() { }

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