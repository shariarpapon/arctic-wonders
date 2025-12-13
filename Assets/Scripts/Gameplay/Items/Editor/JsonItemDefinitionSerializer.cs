using Arctic.Utilities.Serialization;
using Arctic.Utilities.Serialization.Json;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Arctic.Gameplay.Items.Editor
{
    public class JsonItemDefinitionSerializer : ISerializationService<DeserializableItemDefintion, string>
    {
        public SerializerOutput<DeserializableItemDefintion> Deserialize(string json)
        {
            try
            {
                JsonPropertySerializer serializer = new JsonPropertySerializer();
                DeserializableItemDefintion itemDef = new();
                string[] lines = json.Split('\n');
                foreach (string line in lines) 
                {
                    var deserialized = serializer.Deserialize(line);
                    if(deserialized.Status == SerializerStatus.Successful)
                        itemDef.AddProperty(deserialized.Object);
                }
                return new SerializerOutput<DeserializableItemDefintion>(itemDef, SerializerStatus.Successful);
            }
            catch
            {
                return new SerializerOutput<DeserializableItemDefintion>(default, SerializerStatus.Failed);
            }
        }

        public SerializerOutput<string> Serialize(DeserializableItemDefintion itemDef)
        {
            try
            {
                JsonPropertySerializer serializer = new JsonPropertySerializer();
                StringBuilder sb = new StringBuilder();
                foreach (var kv in itemDef.source.UnifiedPropertyLookup)
                {
                    JsonProperty jsonProperty = new JsonProperty(kv.Key, kv.Value, kv.Value.GetType());
                    var serialized = serializer.Serialize(jsonProperty);
                    if (serialized.Status == SerializerStatus.Successful)
                        sb.AppendLine(serialized.Object);
                }
                return new SerializerOutput<string>(sb.ToString(), SerializerStatus.Successful);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return new SerializerOutput<string>("ERROR: " + e.Message, SerializerStatus.Failed);
            }
        }
    }
}