using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;

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
                    Debug.LogError($"Could not serialize property<{property?.GetValueType()?.FullName}> into json string.");
                return new Output<string>($"ERROR: Could not serialize property of type <{property?.GetValueType()?.FullName}>", OutputStatus.Failed);
            }
            catch (Exception e)
            {
                Debug.LogError("Could not serialize json property: " + e.Message);
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
                string key = json.Substring(0, colonIndex).Trim().Trim('"');
                string rawValue = json.Substring(colonIndex + 1).Trim();
                IProperty property = null;
                if (rawValue.StartsWith("\""))
                    property = new GenericProperty<string>(key, rawValue.Trim('"'));
                else if (bool.TryParse(rawValue, out bool boolValue))
                    property = new GenericProperty<bool>(key, boolValue);
                else if (int.TryParse(rawValue, out int intValue))
                    property = new GenericProperty<int>(key, intValue);
                else if(float.TryParse(rawValue, NumberStyles.Float, CultureInfo.InvariantCulture, out float floatValue))
                        property = new GenericProperty<float>(key, floatValue);
                else
                {
                    Debug.LogError($"Cannot parse string uisng <{nameof(JsonPropertySerializer)}> (rawValue: {rawValue})");
                    return new Output<IProperty>(null, OutputStatus.UnableToParse);
                }
                    return new Output<IProperty>(property, OutputStatus.Successful);
            }
            catch (Exception ex) 
            {
                Debug.LogException(ex);
                return new Output<IProperty>(null, OutputStatus.Failed);
            }
        }

        private GenericProperty<TValue> CreateProperty<TValue>(string key, TValue value) 
        {
            return new GenericProperty<TValue>(key, value);
        }

        public List<IProperty> DeserializeAsListOfPropertiesSeperatedByNewLine(string json) 
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