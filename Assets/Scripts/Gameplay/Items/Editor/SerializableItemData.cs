using Arctic.Utilities.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Arctic.Gameplay.Items.Editor
{
    /// <summary>
    /// A serializable editor-only representation of <see cref="ItemData"/>,
    /// used to extract, modify, serialize, and reapply item properties
    /// without mutating the source asset directly.
    /// </summary>

    public struct SerializableItemData
    {
        public string guid;
        public ItemData source;
        public List<IProperty> properties;

        public SerializableItemData(string guid, List<IProperty> properties) 
        {
            this.guid = guid;
            this.properties = properties;
            this.source = null;
            this.source = LoadItemDataAsset(guid);
        }

        public SerializableItemData(ItemData source)
        {
            this.guid = source == null ? null : source.GUID;
            this.source = source;
            this.properties = null;
            BuildPropertyListFromSource(source);
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

        public readonly bool ApplyTo(ItemData target)
        {
            if (target == null) 
                return false;
            target.SetGUID(guid);
            try 
            {
                ISerializer<SerializableItemData, string> serializer = new ItemDataSerializer();
                foreach (IProperty prop in properties)
                {
                    if (prop == null) 
                        continue;
                    if (!target.GetPropertyListLookup(false).ContainsKey(prop.GetValueType()))
                        continue;
                    IProperty data = new ParsedProperty(prop.GetKey(), prop.GetValue(), prop.GetValueType());
                    if (!target.TryAddItemProperty(data, true)) 
                    {
                        Debug.LogError($"Unable to parse item property from deserialized wrapper (key: {prop.GetKey()}) (type: {prop.GetValueType().FullName})");
                        return false;
                    }
                }
                target.RebuildLookups();
                return true;
            }
            catch (Exception e) 
            {
                Debug.LogException(e);
                return false;
            }
        }

        private void BuildPropertyListFromSource(ItemData sourceRef)
        {
            properties = new List<IProperty>();
            foreach (var kv in sourceRef.GetUnifiedPropertyDataLookup(true))
                AddProperty(kv.Value);
        }
    }
}   