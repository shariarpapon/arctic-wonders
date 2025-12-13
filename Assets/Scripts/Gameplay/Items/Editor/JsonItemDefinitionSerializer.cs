using Arctic.Utilities.Serialization;
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
                StringBuilder json = new StringBuilder();
                var lookup = serializableItemDef.source.UnifiedPropertyValueLookup;

                return new SerializerOutput<string>(json.ToString(), SerializerStatus.Successful);
            }
            catch (System.Exception e)
            {
                return new SerializerOutput<string>("ERROR: " + e.Message, SerializerStatus.Failed);
            }
        }
    }
}