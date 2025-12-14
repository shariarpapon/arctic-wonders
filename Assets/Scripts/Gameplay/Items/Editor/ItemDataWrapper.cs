using System;
using System.Collections.Generic;
using Arctic.Utilities.Serialization;
using Arctic.Utilities.Serialization.Json;
using UnityEngine;

namespace Arctic.Gameplay.Items.Editor
{ 
    public struct ItemDataWrapper
    {
        public string guid;
        public ItemDefinition source;
        public List<JsonProperty> properties;

        public ItemDataWrapper(string guid, List<JsonProperty> properties) 
        {
            this.guid = guid;
            this.properties = properties;
            this.source = null;
            this.source = LoadItemDefinitionAsset(guid);
        }

        public ItemDataWrapper(ItemDefinition source)
        {
            this.guid = source == null ? null : source.GUID;
            this.source = source;
            properties = new List<JsonProperty>();
            BuildJsonPropertyList(source);
        }

        //TODO
        public ItemDefinition LoadItemDefinitionAsset(string guid) 
        {
            return null;
        }

        public void AddProperty(JsonProperty prop)
        {
            if (properties == null)
                properties = new List<JsonProperty>();
            properties.Add(prop);
        }

        public bool TryParseIntoSource(ref ItemDefinition source)
        {
            if (source == null) 
                return false;
            source.SetGUID(guid);
            try 
            {
                ISerializer<ItemDataWrapper, string> serializer = new JsonItemDefinitionSerializer();
                foreach (JsonProperty prop in properties)
                {
                    if (prop == null) 
                        continue;
                    if (!source.PropertyListLookup.ContainsKey(prop.ValueType))
                        continue;
                    ItemPropertyData data = new ItemPropertyData(prop.Key, prop.Value, prop.ValueType);
                    if (!source.TryAddProperty(data, true)) 
                    {
                        Debug.LogError($"Unable to parse item property from deserialized wrapper (key: {prop.Key}) (type: {prop.ValueType.FullName})");
                        return false;
                    }
                }
                return true;
            }
            catch (Exception e) 
            {
                Debug.LogException(e);
                return false;
            }
        }

        private void BuildJsonPropertyList(ItemDefinition sourceRef)
        {
            if (sourceRef == null)
                return;
            properties = new List<JsonProperty>();
            foreach (var kv in sourceRef.UnifiedPropertyDataLookup)
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