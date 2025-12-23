using Arctic.Utilities.Editor;
using Arctic.Utilities.Serialization;
using Arctic.Utilities.Serialization.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Arctic.Gameplay.Items.Editor
{
    public class ItemDataSerializer : ISerializer<ItemData, string> 
    {
        public enum DeserializeStatus 
        {
            Successful,
            Failed,
            InvalidInputString,
            CouldNotFindDeserializeTarget,
            CouldNotParseGUID,
        }

        public readonly struct DeserializeDetails
        {
            public static readonly DeserializeDetails FailedResultNoContext = new DeserializeDetails(null, DeserializeStatus.Failed, "", null, "Deserialization failed.");

            public readonly ItemData deserializedItemData;
            public readonly DeserializeStatus status;

            public readonly string guid;
            public readonly IReadOnlyList<IProperty> deserializedProperties;
            
            public readonly string message;
            internal DeserializeDetails(ItemData itemData, DeserializeStatus status, string guid, IEnumerable<IProperty> deserializedProperties, string message = "") 
            {
                this.deserializedItemData = itemData;
                this.status = status;
                this.guid = guid;
                this.deserializedProperties = deserializedProperties?.ToList();
                this.message = message;
            }
        }
        
        protected const string ITEM_GUID_KEY = "guid";
        protected IStringFormatSerializer<IProperty> propertySerializer;

        public ItemDataSerializer() 
        {
            this.propertySerializer = new StringFormatPropertySerializer();
        }

        public ItemDataSerializer(IStringFormatSerializer<IProperty> propertySerializer) 
        {
            this.propertySerializer = propertySerializer;
        }

        public virtual Result<string> Serialize(ItemData itemData)
        {  
            try
            {
                itemData.RebuildLookups();
                List<IProperty> properties = itemData.GetUnifiedPropertyLookup(true).Values.ToList();
                IProperty itemGuidProperty = new Property<string>(ITEM_GUID_KEY, itemData.GUID);
                properties.Add(itemGuidProperty);

                if (propertySerializer.TrySerializeAll(properties, out var serializedProperties)) 
                {
                    StringBuilder sb = new StringBuilder();
                    foreach(string serializedProp in serializedProperties)
                        sb.AppendLine(serializedProp);
                    return new Result<string>(sb.ToString(), OutputStatus.Successful);
                }
                else
                    return new Result<string>(null, OutputStatus.Failed);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return new Result<string>("Exception: " + e.Message, OutputStatus.Failed);
            }
        }


        //TODO: Refactor to detect when ItemData asset with deserialized GUID does not exist in project, and create new ItemData asset in that case.
        //      Create a seperate DeserializeOrCreate method for that.
        //      (i dont think its possible to make it contained within ISerializer unless the return status <OutputStatus> is generic)
        //    ** actually better idea is to create a DeserializeDetailed method that returns more detailed info about the deserialization process.
        //       (allowing us to access intermediate processes)
        //
        //TODO_LATER: handle property diffs, handle nested properties, add parser injection.
        public virtual Result<ItemData> Deserialize(string serializedString)
        {
            string[] lines = serializedString.Split("\n");
            if (!propertySerializer.TryDeserializeAll(lines, out var deserializedProperties)) 
            {
                return new Result<ItemData>(null, OutputStatus.ErrorDeserializing);
            }

            if (TryExtractGUIDFromDeserializedProperties(ref deserializedProperties, out string itemGuid)) 
            {
                if (string.IsNullOrEmpty(itemGuid)) 
                {
                    Debug.LogError($"Invalid item GUID parsed (must be a valid string)");
                    return new Result<ItemData>(null, OutputStatus.ErrorParsing);
                }

                ItemData itemData = ParseItemData(itemGuid, deserializedProperties);
                if (itemData == null)
                {
                    Debug.LogError($"Could not parse from raw data (guid: {itemGuid})");
                    return new Result<ItemData>(null, OutputStatus.ErrorParsing);
                }
                return new Result<ItemData>(itemData, OutputStatus.Successful);
            }
            else
                return new Result<ItemData>(default, OutputStatus.DataCorrupted);
        }

        public virtual DeserializeDetails DeserializeDetailed(string serializedString) 
        {
            if (string.IsNullOrEmpty(serializedString)) 
                return new DeserializeDetails(null, DeserializeStatus.InvalidInputString, null, null, "The serialized-form string passed into the argument was either null or empty.");

            string[] parsedStringProperties = ParseSerializedString(serializedString);
            if (propertySerializer.TryDeserializeAll(parsedStringProperties, out var deserializedProperties)) 
            {
                if (TryExtractGUIDFromDeserializedProperties(ref deserializedProperties, out string itemGuid)) 
                {
                    if (string.IsNullOrEmpty(itemGuid)) 
                        return new DeserializeDetails(null, DeserializeStatus.CouldNotParseGUID, null, deserializedProperties, "Parsed GUID string is either null or empty.");
                    ItemData itemData = ParseItemData(itemGuid, deserializedProperties);
                    if (itemData == null) 
                        return new DeserializeDetails(null, DeserializeStatus.CouldNotFindDeserializeTarget, itemGuid, deserializedProperties, $"Could not find ItemData asset with specified GUID ({itemGuid}) in project.");
                    return new DeserializeDetails(itemData, DeserializeStatus.Successful, itemGuid, deserializedProperties);
                }
                else 
                    return new DeserializeDetails(null, DeserializeStatus.CouldNotParseGUID, null, deserializedProperties, "Could not extract item GUID from deserialized properties.");
            }
            else 
                return new DeserializeDetails(null, DeserializeStatus.Failed, null, null, "Could not deserialize properties from serialized string.");
        }

        private string[] ParseSerializedString(string serializedString) 
        {
            return serializedString.Split("\n");
        }


        protected virtual ItemData ParseItemData(string itemGuid, List<IProperty> properties) 
        {
            try 
            {
                bool sourceFound = Helper.TryFindAssetOfType(out ItemData source, c => c.GUID == itemGuid);
                if (!sourceFound)
                    return null;
                source.SetGUID(itemGuid);
                foreach (IProperty prop in properties)
                {
                    if (!source.TryAddPropperty(prop, true))
                    {
                        Debug.LogWarning("Could not add item property.");
                        continue;
                    }
                }
                source.RebuildLookups();
                return source;
            }
            catch (Exception e) 
            {
                throw new Exception("Unable to parse raw data into ItemData asset: " + e.Message);
            }
        }

        protected virtual bool TryExtractGUIDFromDeserializedProperties(ref List<IProperty> deserializedProperties, out string itemGuid)
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