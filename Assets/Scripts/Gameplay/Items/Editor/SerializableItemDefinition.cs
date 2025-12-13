using Arctic.Utilities.Serialization;
using System.Collections.Generic;
using UnityEngine;

namespace Arctic.Gameplay.Items.Editor
{
    [System.Serializable]
    public class SerializableItemDefinition
    {
        public string guid;

        [System.NonSerialized] public readonly ItemDefinition source;
        [System.NonSerialized] public readonly List<SerializableKeyValue> keyValues;

        public SerializableItemDefinition(ItemDefinition source)
        {
            this.guid = source.GUID;
            this.source = source;
            this.keyValues = KeyValuesFromLookup(source.UnifiedPropertyValueLookup);
        }

        private List<SerializableKeyValue> KeyValuesFromLookup(Dictionary<string, object> lookup) 
        {
            List<SerializableKeyValue> list = new List<SerializableKeyValue>();
            foreach (var kv in lookup) 
                list.Add(new SerializableKeyValue(kv.Key, kv.Value));
            return list;
        }

        public string ToJsonString() 
        {
            return JsonUtility.ToJson(this, true);
        }
    }
}