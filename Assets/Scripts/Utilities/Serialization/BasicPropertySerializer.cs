using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;

namespace Arctic.Utilities.Serialization
{
    /// <summary>
    /// A primitive serializer that converts an <see cref="IProperty"/> into a simple
    /// string representation of the form { "key": value }.
    /// This format is not full JSON and is intended for basic, editor-only use.
    /// </summary>
    public class BasicPropertySerializer : ISerializer<IProperty, string>
    {
        public static readonly HashSet<Type> SerializableTypes = new HashSet<Type>() 
        {
            typeof(string), 
            typeof(bool), 
            typeof(int), 
            typeof(float)
        };

        public Output<string> Serialize(IProperty property)
        {
            try
            {
                if (TrySerializeProperty(property, out string source))
                    return new Output<string>(source, OutputStatus.Successful);
                    Debug.LogError($"Could not serialize property<{property?.GetValueType()?.FullName}> into source string.");
                return new Output<string>($"ERROR: Could not serialize property of type <{property?.GetValueType()?.FullName}>", OutputStatus.Failed);
            }
            catch (Exception e)
            {
                Debug.LogError("Could not serialize source property: " + e.Message);
                return new Output<string>("ERROR: " + e.Message, OutputStatus.Failed);
            }
        }

        public Output<IProperty> Deserialize(string source)
        {
            if (!IsValidSourceString(source))
                return new Output<IProperty>(null, OutputStatus.StringNotValid);

            try
            {
                source = source.Trim();
                source = source.Substring(1, source.Length - 2);

                int colonIndex = source.IndexOf(':');
                if (colonIndex < 0)
                    return new Output<IProperty>(null, OutputStatus.StringNotValid);
                string key = source.Substring(0, colonIndex).Trim().Trim('"');
                string rawValue = source.Substring(colonIndex + 1).Trim();
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
                    Debug.LogError($"Cannot parse string using <{nameof(BasicPropertySerializer)}> (rawValue: {rawValue})");
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

        public virtual List<IProperty> DeserializeAsList(string source) 
        {
            List<IProperty> properties = new List<IProperty>();
            try
            {
                string[] lines = source.Split("\n");
                foreach (var line in lines) 
                {
                    if (!IsValidSourceString(line))
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

        protected virtual bool TrySerializeProperty(IProperty property, out string source) 
        {
            source = null;
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
            source = sb.ToString();
            return true;
        }

        protected virtual bool IsValidSourceString(string source)
        {
            if (string.IsNullOrEmpty(source) || source.Length <= 2)
                return false;
            return true;
        }
    }
}