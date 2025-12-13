using Arctic.Utilities.Serialization;
using Arctic.Utilities.Serialization.Json;
using System.Text;
using UnityEngine;

namespace Arctic.Gameplay.Items.Editor
{
    public class JsonItemDefinitionSerializer : ISerializationService<SerializableItemDefinition, string>
    {
        public SerializerOutput<SerializableItemDefinition> Deserialize(string json)
        {
            try
            {
                SerializableItemDefinition serItemDef = JsonUtility.FromJson<SerializableItemDefinition>(json);
                return new SerializerOutput<SerializableItemDefinition>(serItemDef, serItemDef != null ? SerializerStatus.Successful : SerializerStatus.Failed);
            }
            catch
            {
                return new SerializerOutput<SerializableItemDefinition>(null, SerializerStatus.Failed);
            }
        }

        public SerializerOutput<string> Serialize(SerializableItemDefinition serializableItemDef)
        {
            try
            {
                JsonPropertySerializer serializer = new JsonPropertySerializer();
                StringBuilder sb = new StringBuilder();
                foreach (var kv in serializableItemDef.source.UnifiedPropertyLookup)
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