using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace Arctic.Utilities.Serialization.Json
{
    public sealed class JsonPropertySerializer : ISerializer<JsonProperty, string>
    {
        public static string GUID_KEY = "guid";

        public static readonly HashSet<Type> SerializableTypes = new HashSet<Type>() 
        {
            typeof(string), 
            typeof(bool), 
            typeof(int), 
            typeof(float)
        };

        private bool IsValidJson(string json) 
        {
            if (string.IsNullOrEmpty(json) || json.Length <= 2)
                return false;
            return true;
        }

        public SerializerOutput<string> Serialize(JsonProperty property)
        {
            try
            {
                if (TrySerializeProperty(property, out string json))
                    return new SerializerOutput<string>(json, SerializerStatus.Successful);

                return new SerializerOutput<string>($"ERROR: Could not serialize property of type <{property.type.FullName}>", SerializerStatus.Failed);
            }
            catch (Exception e)
            {
                return new SerializerOutput<string>("ERROR: " + e.Message, SerializerStatus.Failed);
            }
        }

        //Thanks chatgpt, i did not feel like implementing this bs.
        public SerializerOutput<JsonProperty> Deserialize(string json)
        {
            if (!IsValidJson(json))
                return new SerializerOutput<JsonProperty>(null, SerializerStatus.JsonStringNotValid);

            try
            {
                json = json.Trim();
                json = json.Substring(1, json.Length - 2);

                int colonIndex = json.IndexOf(':');
                if (colonIndex < 0)
                    return new SerializerOutput<JsonProperty>(null, SerializerStatus.JsonStringNotValid);
                string id = json.Substring(0, colonIndex).Trim().Trim('"');
                string rawValue = json.Substring(colonIndex + 1).Trim();

                object value;
                Type type;

                if (rawValue.StartsWith("\""))
                {
                    value = rawValue.Trim('"');
                    type = typeof(string);
                }
                else if (rawValue == "true" || rawValue == "false")
                {
                    value = bool.Parse(rawValue);
                    type = typeof(bool);
                }
                else if (!rawValue.Contains("."))
                {
                    value = int.Parse(rawValue);
                    type = typeof(int);
                }
                else
                {
                    value = float.Parse(rawValue, CultureInfo.InvariantCulture);
                    type = typeof(float);
                }

                var property = new JsonProperty(id, value, type);
                return new SerializerOutput<JsonProperty>(property, SerializerStatus.Successful);
            }
            catch (Exception ex) 
            {
                Debug.LogException(ex);
                return new SerializerOutput<JsonProperty>(null, SerializerStatus.Failed);
            }
        }

        public List<JsonProperty> ParsePropertyList(string json) 
        {
            List<JsonProperty> properties = new List<JsonProperty>();
            try
            {
                string[] lines = json.Split("\n");
                foreach (var line in lines) 
                {
                    if (string.IsNullOrEmpty(line) || !IsValidJson(line))
                        continue;
                    SerializerOutput<JsonProperty> output = Deserialize(line);
                    if (output.Status == SerializerStatus.Successful)
                        properties.Add(output.Object);
                }
                return properties;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        //MUST: add TryDeserializeEnumerable for back and forth conversions of list.
        public bool TrySerializeEnumerable(string key, IEnumerable<JsonProperty> enumerable, out string json) 
        {
            json = null;
            try
            {
                StringBuilder sb = new StringBuilder();
                if (!TrySerializeProperty(new JsonProperty(GUID_KEY, key, typeof(string)), out string guidJson))
                {
                    Debug.LogError($"Could not serialize proprety GUID ({GUID_KEY} : {key})");
                    return false;
                }
                sb.AppendLine(guidJson);
                foreach (var property in enumerable)
                {
                    var serialized = Serialize(property);
                    if (serialized.Status == SerializerStatus.Successful)
                        sb.AppendLine(serialized.Object);
                }
                json = sb.ToString();
                return true;
            }
            catch (Exception e) 
            {
                Debug.LogException(e);
                return false;
            }
        }

        private bool TrySerializeProperty(JsonProperty property, out string json) 
        {
            json = null;
            StringBuilder sb = new StringBuilder();
            sb.Append("{\""+property.id +"\":");
            string value = "";

            if (!SerializableTypes.Contains(property.type))
            {
                Debug.LogError($"Serialization of type <{property.type.FullName}> is not supported.");
                return false;
            }

            if (property.type == typeof(string))
                value = $"\"{property.ValueAs<string>()}\"";
            else if (property.type == typeof(bool))
                value = property.ValueAs<bool>().ToString().ToLower();
            else if (property.type == typeof(int))
                value = property.ValueAs<int>().ToString();
            else if (property.type == typeof(float))
                value = property.ValueAs<float>().ToString(CultureInfo.InvariantCulture);

            sb.Append(value + "}");
            json = sb.ToString();
            return true;
        }
    }
}