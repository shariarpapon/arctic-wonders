using Arctic.Utilities.Serialization;
using Arctic.Utilities.Serialization.Json;
using System.Collections.Generic;
using UnityEngine;

namespace Arctic.Gameplay.Items.Editor
{
    public class JsonItemDefinitionSerializer : ISerializer<DeserializableItemDefintion, string>
    {
        private JsonPropertySerializer propertySerializer;

        public SerializerOutput<string> Serialize(DeserializableItemDefintion itemDef)
        {
            propertySerializer = new JsonPropertySerializer();
            try
            {
                if (propertySerializer.TrySerializeEnumerable(itemDef.guid, itemDef.properties, out string json))
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

        public SerializerOutput<DeserializableItemDefintion> Deserialize(string json)
        {
            DeserializableItemDefintion output = new();
            SerializerStatus status = SerializerStatus.Failed;
            try
            {
                JsonPropertySerializer serializer = new JsonPropertySerializer();
                List<JsonProperty> properties = serializer.ParseAsList(json);
              
                try
                {
                    JsonProperty guidProprety = properties.Find(c => c.id == JsonPropertySerializer.GUID_KEY);
                    if (guidProprety != null)
                    {
                        properties.Remove(guidProprety);
                        output.guid = guidProprety.ValueAs<string>();
                        output.properties = properties;
                        status = SerializerStatus.Successful;
                    }
                    else 
                    {
                        status = SerializerStatus.GuidKeyNotFound;
                        throw new System.InvalidOperationException("Cannot parse valid GUID property with key : " + JsonPropertySerializer.GUID_KEY); 
                    }
                }
                catch (System.InvalidOperationException)
                {
                    Debug.LogWarning($"Asigning random GUID to item definition.");
                    output.guid = ItemDefinition.GenerateRandomGUID();
                    status = SerializerStatus.Failed;
                }

                return new SerializerOutput<DeserializableItemDefintion>(output, status);
            }
            catch(System.Exception ex)
            {
                Debug.LogException(ex);
                status = SerializerStatus.Failed;
                return new SerializerOutput<DeserializableItemDefintion>(output, status);
            }
        }
    }
}