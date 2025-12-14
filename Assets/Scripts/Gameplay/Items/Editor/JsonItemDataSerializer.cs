using Arctic.Utilities.Serialization;
using Arctic.Utilities.Serialization.Json;
using System.Collections.Generic;
using UnityEngine;

namespace Arctic.Gameplay.Items.Editor
{
    public class JsonItemDataSerializer : ISerializer<ItemDataWrapper, string>
    {
        private const string ITEM_GUID_KEY = "guid";

        public SerializerOutput<string> Serialize(ItemDataWrapper itemDef)
        {
            JsonPropertySerializer jsonPropretySerializer = new JsonPropertySerializer();
            try
            {
                JsonProperty itemGuidProperty = new JsonProperty(ITEM_GUID_KEY, itemDef.guid, typeof(string));
                itemDef.properties.Add(itemGuidProperty);
                if (jsonPropretySerializer.TrySerializeProperties(itemDef.properties, out string json))
                    return new SerializerOutput<string>(json, SerializerStatus.Successful);
                else
                    return new SerializerOutput<string>(null, SerializerStatus.Failed);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return new SerializerOutput<string>("ERROR: " + e.Message, SerializerStatus.Failed);
            }
        }

        public SerializerOutput<ItemDataWrapper> Deserialize(string json)
        {
            JsonPropertySerializer propretySerializer = new JsonPropertySerializer();
            List<JsonProperty> deserializedProperties = propretySerializer.DeserializeList(json);
            if (TryExtractGUIDFromDeserializedProperties(ref deserializedProperties, out string itemGuid)) 
            {
                ItemDataWrapper itemDataWrapper = new ItemDataWrapper(itemGuid, deserializedProperties);
                return new SerializerOutput<ItemDataWrapper>(itemDataWrapper, SerializerStatus.Successful);
            }
            else
                return new SerializerOutput<ItemDataWrapper>(default, SerializerStatus.Failed);
        }

        private static bool TryExtractGUIDFromDeserializedProperties(ref List<JsonProperty> deserializedProperties, out string itemGuid)
        {
            itemGuid = ItemData.GenerateRandomGUID();
            try
            {
                JsonProperty itemGuidProperty = deserializedProperties.Find(p => p.Key == ITEM_GUID_KEY);
                string itemGuidValue = itemGuidProperty.ValueAs<string>();
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