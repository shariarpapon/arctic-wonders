using System;
using System.Collections.Generic;
using UnityEngine;

namespace Arctic.Utilities.Serialization.Json
{
    public sealed class JsonPropertySerializer : ISerializationService<JsonProperty, string>
    {
        private static readonly Dictionary<Type, ISerializationService<JsonProperty, string>> Serializers = new()
        {
            { typeof(string), new JsonPropertySerializer() },
            { typeof(bool), new JsonPropertySerializer() },
            { typeof(int), new JsonPropertySerializer() },
            { typeof(float), new JsonPropertySerializer() }
        };


        public SerializerOutput<string> Serialize(JsonProperty property)
        {
            try
            {
                if (TrySerializeProperty(property, out string json))
                    return new SerializerOutput<string>(json, SerializerStatus.Successful);

                return new SerializerOutput<string>($"ERROR: Could not serialize property of type <{property.type.Name}>", SerializerStatus.Failed);
            }
            catch (Exception e)
            {
                return new SerializerOutput<string>("ERROR: " + e.Message, SerializerStatus.Failed);
            }
        }

        public SerializerOutput<JsonProperty> Deserialize(string json)
        {
            throw new NotImplementedException();
        }

        private bool TrySerializeProperty(JsonProperty property, out string json) 
        {
            json = null;
            return false;
        }
    }
}