using Arctic.Utilities.Serialization;
using Arctic.Utilities.Serialization.Json;
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
            try
            {
                JsonPropertySerializer serializer = new JsonPropertySerializer();
                DeserializableItemDefintion desItemDef = new();
                string[] lines = json.Split('\n');
                bool serializedAtleastOne = false;
                foreach (string line in lines) 
                {
                    var deserialized = serializer.Deserialize(line);
                    if (deserialized.Status == SerializerStatus.Successful)
                    {
                        desItemDef.AddProperty(deserialized.Object);
                        serializedAtleastOne = true;
                    }
                }

                try
                {
                    JsonProperty guidProp = desItemDef.properties.Find(c => c.id == JsonPropertySerializer.GUID_KEY);
                    if (guidProp != null)
                    {
                        desItemDef.properties.Remove(guidProp);
                        desItemDef.guid = guidProp.ValueAs<string>();
                    }
                    else throw new System.InvalidOperationException("Cannot parse valid GUID property with key : " + JsonPropertySerializer.GUID_KEY);
                }
                catch (System.InvalidOperationException)
                {
                    Debug.LogWarning($"Asigning random GUID to item definition.");
                    desItemDef.guid = ItemDefinition.GenerateRandomGUID();
                }

                return new SerializerOutput<DeserializableItemDefintion>(desItemDef, serializedAtleastOne ? SerializerStatus.Successful : SerializerStatus.Failed);
            }
            catch(System.Exception ex)
            {
                Debug.LogException(ex);
                return new SerializerOutput<DeserializableItemDefintion>(default, SerializerStatus.Failed);
            }
        }
    }
}