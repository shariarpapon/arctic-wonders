using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Arctic.Utilities.Generics
{
    [System.Serializable]
    public sealed class UnityDictionaryView<TKey, TValue>
    {
        [System.Serializable]
        public struct SerializedKeyValuePair
        {
            public TKey key;
            public TValue value;

            public SerializedKeyValuePair(TKey key, TValue value)
            {
                this.key = key;
                this.value = value;
            }

            public static implicit operator SerializedKeyValuePair(KeyValuePair<TKey, TValue> kvp) 
                => new SerializedKeyValuePair(kvp.Key, kvp.Value);
        }

        [SerializeField] private List<SerializedKeyValuePair> keyValues;

        private Dictionary<TKey, TValue> sourceDictionary;

        public static implicit operator Dictionary<TKey, TValue> (UnityDictionaryView<TKey, TValue> serializedDict) 
        {
            Dictionary<TKey, TValue> converted = new Dictionary<TKey, TValue>();
            foreach (var kv in serializedDict.keyValues)
                converted.Add(kv.key, kv.value);
            return converted;
        }

        public static implicit operator UnityDictionaryView<TKey, TValue>(Dictionary<TKey, TValue> dict)
        {
            UnityDictionaryView<TKey, TValue> converted = new UnityDictionaryView<TKey, TValue>();
            if (dict == null || dict.Count <= 0)
                return converted;
            foreach (var kv in dict)
                converted.Add(kv.Key, kv.Value);
            return converted;
        }

        public static implicit operator UnityDictionaryView<TKey, TValue>(List<SerializedKeyValuePair> serializedKeyValues) 
        {
            UnityDictionaryView<TKey, TValue> converted = new UnityDictionaryView<TKey, TValue>();
            foreach (var kv in serializedKeyValues)
                converted.TryAdd(kv.key, kv.value);
            return converted;
        }

        public Dictionary<TKey, TValue> FromSerializedKeyValuePairs()
        {
            Dictionary<TKey, TValue> converted = new Dictionary<TKey, TValue>();
            foreach (var kv in keyValues)
                converted.TryAdd(kv.key, kv.value);
            return converted;
        }

        public override string ToString()
        {
            if (keyValues == null) 
                return "Error: No key-values set.";

            StringBuilder builder = new StringBuilder();
            builder.AppendLine(this.GetType().Name + $" ({keyValues.Count})");
            for (int i = 0; i < keyValues.Count; i++)
            {
                if (i == 0) continue;
                builder.Append("\n");
                builder.Append($"[{keyValues[i].key.ToString()}, {keyValues[i].value.ToString()}]");
            }
            return builder.ToString();
        }

        public void Serialize(Dictionary<TKey, TValue> dictionary) 
        {
            sourceDictionary = dictionary;
            if (keyValues == null) keyValues = new List<SerializedKeyValuePair>();
            else keyValues.Clear();

            foreach (KeyValuePair<TKey, TValue> kv in dictionary) 
                keyValues.Add(new SerializedKeyValuePair(kv.Key, kv.Value));
        }

        public void Deserialize() 
        {
            sourceDictionary = FromSerializedKeyValuePairs();
        }

        public void Overwrite(Dictionary<TKey, TValue> dictionary)
        {
            keyValues = new List<SerializedKeyValuePair>();
            foreach (KeyValuePair<TKey, TValue> kv in dictionary)
                keyValues.Add(kv);
        }

        public Dictionary<TKey, TValue> ReadSerializedChanges() => FromSerializedKeyValuePairs();

        /// <summary>
        /// Attemps to add the key without checking for duplicates.
        /// </summary>
        public void Add(TKey key, TValue value) => sourceDictionary.Add(key, value);

        /// <summary>
        /// Overwrites the value if the key exists, otherwise adds the key-value to the dictionary.
        /// </summary>
        public void Upsert(TKey key, TValue value) 
        {
            if (!TrySetValue(key, value))
                sourceDictionary.Add(key, value);
        }

        /// <summary>
        /// Copies the content data into a new Dictionary object.
        /// </summary>
        /// <returns>The new copy of the dictionary.</returns>
        public Dictionary<TKey, TValue> Copy() => new Dictionary<TKey, TValue>(sourceDictionary);
        
        public int Count => sourceDictionary.Count;
        /// <returns>True, if element with given key was found/removed, false otherwise.</returns>
        public bool Remove(TKey key) => sourceDictionary.Remove(key);

        public bool ContainsKey(TKey key) => sourceDictionary.ContainsKey(key);

        public bool ContainsValue(TValue value) => sourceDictionary.ContainsValue(value);

        public bool TryAdd(TKey key, TValue value) => sourceDictionary.TryAdd(key, value);

        public bool TryGetValue(TKey key, out TValue value) => sourceDictionary.TryGetValue(key, out value);

        public bool TrySetValue(TKey key, TValue value)
        {
            if (sourceDictionary.ContainsKey(key))
            {
                sourceDictionary[key] = value;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Attemps to set a key-value in the dictionary.
        /// </summary>
        public void Set(TKey key, TValue value) 
        {
            sourceDictionary[key] = value;
        }
    }

}