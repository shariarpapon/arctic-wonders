using System;
using System.Collections.Generic;
using Arctic.Utilities.Serialization;
using Arctic.Utilities.Serialization.Json;
using UnityEngine;

namespace Arctic.Gameplay.Items.Editor
{
    public struct DeserializableItemDefintion
    {
        public string guid;
        public List<JsonProperty> properties;

        [System.NonSerialized] public ItemDefinition source;

        public DeserializableItemDefintion(ItemDefinition source)
        {
            this.guid = source == null ? null : source.GUID;
            this.source = source;
            properties = new List<JsonProperty>();
            BuildJsonPropertyList();
        }
        public void AddProperty(JsonProperty prop)
        {
            if (properties == null)
                properties = new List<JsonProperty>();
            properties.Add(prop);
        }

        public bool TryParseIntoSource(ref ItemDefinition source)
        {
            if (source == null || properties == null)
                return false;
            try 
            {
                ISerializer<DeserializableItemDefintion, string> serializer = new JsonItemDefinitionSerializer();
                foreach (JsonProperty prop in properties) 
                {
                    if (source.UnifiedPropertyDataLookup.ContainsKey(prop.id))
                        continue;
                    ItemPropertyData data = new ItemPropertyData(prop.id, prop.value, prop.type);
                    if (!source.TryAddProperty(data)) 
                    {
                        Debug.LogError($"Unable to parse item property from deserialized wrapper (key: {prop.id}) (type: {prop.type.FullName})");
                        return false;
                    }
                    return true;
                }
                return true;
            }
            catch (Exception e) 
            {
                Debug.LogException(e);
                return false;
            }
        }

        private void BuildJsonPropertyList()
        {
            if (source == null)
                return;
            properties = new List<JsonProperty>();
            foreach (var kv in source.UnifiedPropertyDataLookup)
            {
                string key = kv.Key;
                ItemPropertyData data = kv.Value;
                object value = data.value;
                System.Type valueType = data.type;
                JsonProperty property = new JsonProperty(key, value, valueType);
                AddProperty(property);
            }
        }
    }
}   