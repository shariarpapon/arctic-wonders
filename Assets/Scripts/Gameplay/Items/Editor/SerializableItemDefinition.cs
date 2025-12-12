using System.Collections.Generic;
using Arctic.Utilities.Serialization;

namespace Arctic.Gameplay.Items.Editor
{
    [System.Serializable]
    public class SerializableItemDefinition
    {
        public string guid;
        public List<SerializableKeyValueData> keyValues;
        public SerializableItemDefinition(ItemDefinition itemDef)
        {
            this.guid = itemDef.GUID;
            this.keyValues = CreatePropertyListFromDictionary(itemDef.PropertyLookup);
        }

        private List<SerializableKeyValueData> CreatePropertyListFromDictionary(Dictionary<string, object> dictionary) 
        {
            List<SerializableKeyValueData> list = new List<SerializableKeyValueData>();
            foreach (var kv in dictionary) 
                list.Add(new SerializableKeyValueData(kv.Key, kv.Value));
            return list;
        }
    }
}