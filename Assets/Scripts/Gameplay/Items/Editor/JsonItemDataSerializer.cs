using Arctic.Utilities.Serialization;
using Arctic.Utilities.Serialization.Json;
using System.Collections.Generic;
using UnityEngine;

namespace Arctic.Gameplay.Items.Editor
{
    public class JsonItemDataSerializer : ISerializer<ItemDataWrapper, string>
    {
        private const string ITEM_GUID_KEY = "guid";

        public Output<string> Serialize(ItemDataWrapper itemDef)
        {
            JsonPropertySerializer jsonPropretySerializer = new JsonPropertySerializer();
            try
            {
                Property itemGuidProperty = new Property(ITEM_GUID_KEY, itemDef.guid, typeof(string));
                itemDef.properties.Add(itemGuidProperty);
                if (jsonPropretySerializer.TrySerializeProperties(itemDef.properties, out string json))
                    return new Output<string>(json, SerializerStatus.Successful);
                else
                    return new Output<string>(null, SerializerStatus.Failed);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return new Output<string>("ERROR: " + e.Message, SerializerStatus.Failed);
            }
        }

        public Output<ItemDataWrapper> Deserialize(string json)
        {
            JsonPropertySerializer propretySerializer = new JsonPropertySerializer();
            List<IProperty> deserializedProperties = propretySerializer.DeserializeList(json);
            if (TryExtractGUIDFromDeserializedProperties(ref deserializedProperties, out string itemGuid)) 
            {
                ItemDataWrapper itemDataWrapper = new ItemDataWrapper(itemGuid, deserializedProperties);
                return new Output<ItemDataWrapper>(itemDataWrapper, SerializerStatus.Successful);
            }
            else
                return new Output<ItemDataWrapper>(default, SerializerStatus.Failed);
        }

        private static bool TryExtractGUIDFromDeserializedProperties(ref List<IProperty> deserializedProperties, out string itemGuid)
        {
            itemGuid = ItemData.GenerateRandomGUID();
            try
            {
                IProperty itemGuidProperty = deserializedProperties.Find(p => p.GetKey() == ITEM_GUID_KEY);
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