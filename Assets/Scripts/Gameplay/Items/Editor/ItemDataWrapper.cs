using System;
using System.Collections.Generic;
using UnityEngine;
using Arctic.Utilities.Serialization;
using Arctic.Utilities.Serialization.Json;

namespace Arctic.Gameplay.Items.Editor
{ 
    public struct ItemDataWrapper
    {
        public string guid;
        public ItemData source;
        public List<IProperty> properties;

        public ItemDataWrapper(string guid, List<IProperty> properties) 
        {
            this.guid = guid;
            this.properties = properties;
            this.source = null;
            this.source = LoadItemDataAsset(guid);
        }

        public ItemDataWrapper(ItemData source)
        {
            this.guid = source == null ? null : source.GUID;
            this.source = source;
            properties = new List<IProperty>();
            BuildPropertyListFromData(source);
        }

        //TODO
        public ItemData LoadItemDataAsset(string guid) 
        {
            return null;
        }

        public void AddProperty(IProperty prop)
        {
            if (properties == null)
                properties = new List<IProperty>();
            properties.Add(prop);
        }

        public bool ApplyChangesToSource(ref ItemData source)
        {
            if (source == null) 
                return false;
            source.SetGUID(guid);
            try 
            {
                ISerializer<ItemDataWrapper, string> serializer = new ItemDataSerializer(new JsonPropertySerializer());
                foreach (Property prop in properties)
                {
                    if (prop == null) 
                        continue;
                    if (!source.PropertyListLookup.ContainsKey(prop.GetValueType()))
                        continue;
                    IProperty data = new SerializableItemProperty(prop.GetKey(), prop.GetValue(), prop.GetValueType());
                    if (!source.TryAddItemProperty(data, true)) 
                    {
                        Debug.LogError($"Unable to parse item property from deserialized wrapper (key: {prop.GetKey()}) (type: {prop.GetValueType().FullName})");
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

        private void BuildPropertyListFromData(ItemData sourceRef)
        {
            if (sourceRef == null)
                return;
            properties = new List<IProperty>();
            foreach (var kv in sourceRef.UnifiedPropertyDataLookup)
            {
                string key = kv.Key;
                SerializableItemProperty data = kv.Value;

                object value = data.GetValue();
                System.Type valueType = data.GetType();

                AddProperty(CreateProperty(key, value, valueType));
            }
        }

        private IProperty CreateProperty(string key, object value, System.Type valueType)
        { 
            return new Property(key, value, valueType); 
        }
    }
}   