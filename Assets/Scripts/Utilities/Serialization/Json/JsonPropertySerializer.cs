using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace Arctic.Utilities.Serialization.Json
{
    //TODO: create PropertySerializer base class with all universal property serialization functionality and inherit from that and override parsing.
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
                    return new Output<string>(json, OutputStatus.Successful);

                return new Output<string>($"ERROR: Could not serialize property of type <{property.GetValueType().FullName}>", OutputStatus.Failed);
            }
            catch (Exception e)
            {
                return new Output<string>("ERROR: " + e.Message, OutputStatus.Failed);
            }
        }

        //Thanks chatgpt, i did not feel like implementing this bs.
        public Output<IProperty> Deserialize(string json)
        {
            if (!IsValidJson(json))
                return new Output<IProperty>(null, OutputStatus.StringNotValid);

            try
            {
                json = json.Trim();
                json = json.Substring(1, json.Length - 2);

                int colonIndex = json.IndexOf(':');
                if (colonIndex < 0)
                    return new Output<IProperty>(null, OutputStatus.StringNotValid);
                string id = json.Substring(0, colonIndex).Trim().Trim('"');
                string rawValue = json.Substring(colonIndex + 1).Trim();

                object value = null;
                Type type;

                if (rawValue.StartsWith("\""))
                {
                    value = rawValue.Trim('"');
                    type = typeof(string);
                }
                else if (bool.TryParse(rawValue, out bool boolValue))
                {
                    value = boolValue;
                    type = typeof(bool);
                }
                else if (int.TryParse(rawValue, out int intValue))
                {
                    value = intValue;
                    type = typeof(int);
                }
                else
                {
                    value = float.Parse(rawValue, CultureInfo.InvariantCulture);
                    type = typeof(float);
                }

                var property = new Property(id, value, type);
                return new Output<IProperty>(property, OutputStatus.Successful);
            }
            catch (Exception ex) 
            {
                Debug.LogException(ex);
                return new Output<IProperty>(null, OutputStatus.Failed);
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
                    if (output.Status == OutputStatus.Successful)
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
                    if (serialized.Status == OutputStatus.Successful)
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