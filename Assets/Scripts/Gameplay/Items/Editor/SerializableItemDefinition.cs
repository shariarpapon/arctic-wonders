using Arctic.Utilities.Serialization;
using System.Collections.Generic;
using UnityEngine;

namespace Arctic.Gameplay.Items.Editor
{
    [System.Serializable]
    public class SerializableItemDefinition
    {
        public string guid;
        public List<SerializableKeyValueData> properties;
        public SerializableItemDefinition(ItemDefinition itemDef)
        {
            this.guid = itemDef.GUID;
            this.properties = CreatePropertyListFromDictionary(itemDef.ItemPropertyLookup);
        }

        private List<SerializableKeyValueData> CreatePropertyListFromDictionary(Dictionary<string, object> dictionary) 
        {
            List<SerializableKeyValueData> list = new List<SerializableKeyValueData>();
            foreach (var kv in dictionary) 
                list.Add(new SerializableKeyValueData(kv.Key, kv.Value));
            return list;
        }

        public string ToJsonString() 
        {
            return JsonUtility.ToJson(this, true);
        }
    }
}