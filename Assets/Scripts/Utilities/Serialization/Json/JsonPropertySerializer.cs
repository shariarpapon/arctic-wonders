using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace Arctic.Utilities.Serialization.Json
{
    public sealed class JsonPropertySerializer : ISerializer<IProperty, string>
    {
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

        public Output<string> Serialize(IProperty property)
        {
            try
            {
                if (TrySerializeProperty(property, out string json))
                    return new Output<string>(json, SerializerStatus.Successful);

                return new Output<string>($"ERROR: Could not serialize property of type <{property.GetValueType().FullName}>", SerializerStatus.Failed);
            }
            catch (Exception e)
            {
                return new Output<string>("ERROR: " + e.Message, SerializerStatus.Failed);
            }
        }

        //Thanks chatgpt, i did not feel like implementing this bs.
        public Output<IProperty> Deserialize(string json)
        {
            if (!IsValidJson(json))
                return new Output<IProperty>(null, SerializerStatus.JsonStringNotValid);

            try
            {
                json = json.Trim();
                json = json.Substring(1, json.Length - 2);

                int colonIndex = json.IndexOf(':');
                if (colonIndex < 0)
                    return new Output<IProperty>(null, SerializerStatus.JsonStringNotValid);
                string id = json.Substring(0, colonIndex).Trim().Trim('"');
                string rawValue = json.Substring(colonIndex + 1).Trim();

                object value = null;
                Type type;

                if (rawValue.StartsWith("\""))
                {
                    value = rawValue.Trim('"');
                    type = typeof(string);
                }
                else if (TryParseBoolValue(ref rawValue, out bool boolValue))
                {
                    value = boolValue;
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

                var property = new Property(id, value, type);
                return new Output<IProperty>(property, SerializerStatus.Successful);
            }
            catch (Exception ex) 
            {
                Debug.LogException(ex);
                return new Output<IProperty>(null, SerializerStatus.Failed);
            }
        }

        private bool TryParseBoolValue(ref string value, out bool boolValue) 
        {
            boolValue = default;
            switch (value.ToLower()) 
            {
                case "true":
                case "false":
                    return bool.TryParse(value, out boolValue);
                default:
                    return false;
            }
        }

        public List<IProperty> DeserializeList(string json) 
        {
            List<IProperty> properties = new List<IProperty>();
            try
            {
                string[] lines = json.Split("\n");
                foreach (var line in lines) 
                {
                    if (!IsValidJson(line))
                        continue;
                    Output<IProperty> output = Deserialize(line);
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

        public bool TrySerializeProperties(IEnumerable<IProperty> properties, out string json) 
        {
            json = null;
            try
            {
                StringBuilder sb = new StringBuilder();               
                foreach (var property in properties)
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

        private bool TrySerializeProperty(IProperty property, out string json) 
        {
            json = null;
            StringBuilder sb = new StringBuilder();
            sb.Append("{\""+property.GetKey() +"\":");
            string value = "";

            if (!SerializableTypes.Contains(property.GetValueType()))
                return false;
            if (property.GetValueType() == typeof(string))
                value = $"\"{property.ValueAs<string>()}\"";
            else if (property.GetValueType() == typeof(bool))
                value = property.ValueAs<bool>().ToString().ToLower();
            else if (property.GetValueType() == typeof(int))
                value = property.ValueAs<int>().ToString();
            else if (property.GetValueType() == typeof(float))
                value = property.ValueAs<float>().ToString(CultureInfo.InvariantCulture);

            sb.Append(value + "}");
            json = sb.ToString();
            return true;
        }
    }
}