using Arctic.Utilities.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Arctic.Gameplay.Items.Editor
{
    public class ItemDataSerializer : ISerializer<ItemData, string> 
    {
        private const string ITEM_GUID_KEY = "guid";
        private ISerializer<IProperty, string> propertySerializer;

        public ItemDataSerializer() 
        {
            this.propertySerializer = new PropertySerializer();
        }

        public ItemDataSerializer(ISerializer<IProperty, string> propertySerializer) 
        {
            this.propertySerializer = propertySerializer;
        }

        public Output<string> Serialize(ItemData itemData)
        {  
            try
            {
                List<IProperty> properties = itemData.GetUnifiedPropertyLookup(true).Values.ToList();
                IProperty itemGuidProperty = new Property<string>(ITEM_GUID_KEY, itemData.GUID);
                properties.Add(itemGuidProperty);

                if (propertySerializer.TrySerializeAll(properties, out var serializedProperties)) 
                {
                    StringBuilder sb = new StringBuilder();
                    foreach(string serializedProp in serializedProperties)
                        sb.AppendLine(serializedProp);
                    return new Output<string>(sb.ToString(), OutputStatus.Successful);
                }
                else
                    return new Output<string>(null, OutputStatus.Failed);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return new Output<string>("ERROR: " + e.Message, OutputStatus.Failed);
            }
        }

        public Output<ItemData> Deserialize(string serializedString)
        {
            string[] lines = serializedString.Split("\n");
            if (!propertySerializer.TryDeserializeAll(lines, out var deserializedProperties))
            {
                return new Output<ItemData>(null, OutputStatus.ErrorDeserializing);
            }

            if (TryExtractGUIDFromDeserializedProperties(ref deserializedProperties, out string itemGuid)) 
            {
                if (string.IsNullOrEmpty(itemGuid)) 
                {
                    Debug.LogError($"Invalid item GUID parsed (must be a valid string)");
                    return new Output<ItemData>(null, OutputStatus.ErrorParsing);
                }

                ItemData itemData = FromRawData(itemGuid, deserializedProperties);
                if (itemData == null)
                {
                    Debug.LogError($"Could not parse from raw data (guid: {itemGuid})");
                    return new Output<ItemData>(null, OutputStatus.ErrorParsing);
                }
                return new Output<ItemData>(itemData, OutputStatus.Successful);
            }
            else
                return new Output<ItemData>(default, OutputStatus.DataCorrupted);
        }

        private ItemData FromRawData(string itemGuid, List<IProperty> properties)
        {
            try 
            {
                bool sourceFound = DataIO.TryFindAssetOfType(out ItemData source, c => c.GUID == itemGuid);
                if (!sourceFound) 
                {
                    Debug.LogError($"Source ItemData asset with specified GUID not found (guid: {itemGuid}) in project.");
                    return null;
                }
                source.SetGUID(itemGuid);
                foreach (IProperty prop in properties)
                {
                    if (!source.TryAddPropperty(prop, true))
                    {
                        Debug.LogWarning("Could not add item property.");
                        continue;
                    }
                }
                return source;
            }
            catch (Exception e) 
            {
                throw new Exception("Unable to parse raw data into ItemData asset: " + e.Message);
            }
        }

        private static bool TryExtractGUIDFromDeserializedProperties(ref List<IProperty> deserializedProperties, out string itemGuid)
        {
            itemGuid = ItemData.GenerateRandomGUID();
            try
            {
                IProperty itemGuidProperty = deserializedProperties.Find(p => p.GetKey() == ITEM_GUID_KEY);
                string itemGuidValue = itemGuidProperty.GetValueAs<string>();
                itemGuid = itemGuidValue;
                deserializedProperties.Remove(itemGuidProperty);
                return true;
            }
            catch (System.Exception e) 
            {
                Debug.LogError("Error: Could not extract item GUID from deserialized properties: " + e.Message);
                return false;
            }
        }
    }
}