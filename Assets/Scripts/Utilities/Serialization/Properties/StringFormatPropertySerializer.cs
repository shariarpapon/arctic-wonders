using System;
using System.Globalization;
using System.Text;

namespace Arctic.Utilities.Serialization.Properties
{
    /// <summary>
    /// A primitive serializer that converts an <see cref="IProperty"/> into a simple
    /// string representation of the form { "key": value }.
    /// This format is not full JSON and is intended for basic, editor-only use.
    /// </summary>
    public class StringFormatPropertySerializer : IStringFormatSerializer<IProperty>
    {
        public Result<string> Serialize(IProperty property)
        {

            bool success = TrySerializeProperty(property, out string source);
            if (success) return new Result<string>(source, OutputStatus.Successful);
            else return new Result<string>("Could not serialize.", OutputStatus.ErrorSerializing);
        }

        public Result<IProperty> Deserialize(string propertySrc)
        {
            bool success = TryDeserializeProperty(propertySrc, out IProperty prop);
            if (success) return new Result<IProperty>(prop, OutputStatus.Successful);
            else return new Result<IProperty>(null, OutputStatus.ErrorDeserializing);
        }

        protected virtual bool TryDeserializeProperty(string propertySrc, out IProperty property) 
        {
            property = null;

            if (!IsValidSource(propertySrc))
                return false;
            try
            {
                propertySrc = propertySrc.Trim();
                propertySrc = propertySrc.Substring(1, propertySrc.Length - 2);

                int colonIndex = propertySrc.IndexOf(':');
                if (colonIndex < 0)
                    throw new System.Exception("Error: Cannot deserialize: invalid syntax");

                string key = propertySrc.Substring(0, colonIndex).Trim().Trim('"');
                string rawValue = propertySrc.Substring(colonIndex + 1).Trim();
                if (rawValue.StartsWith("\""))
                    property = new Property<string>(key, rawValue.Trim('"'));
                else if (bool.TryParse(rawValue, out bool boolValue))
                    property = new Property<bool>(key, boolValue);
                else if (int.TryParse(rawValue, out int intValue))
                    property = new Property<int>(key, intValue);
                else if (float.TryParse(rawValue, NumberStyles.Float, CultureInfo.InvariantCulture, out float floatValue))
                    property = new Property<float>(key, floatValue);
                else return false;
                return true;
            }
            catch (Exception e)
            {
                throw new System.Exception($"Cannot deserialize using <{GetType().FullName}> : " + e.Message);
            }
        }

        protected virtual bool TrySerializeProperty(IProperty property, out string propertySrc) 
        {
            propertySrc = null;
            StringBuilder sb = new StringBuilder();
            sb.Append("{\""+property.GetKey() +"\":");
            string value = "";

            if (property.GetValueType() == typeof(string))
                value = $"\"{property.GetValueAs<string>()}\"";
            else if (property.GetValueType() == typeof(bool))
                value = property.GetValueAs<bool>().ToString().ToLower();
            else if (property.GetValueType() == typeof(int))
                value = property.GetValueAs<int>().ToString();
            else if (property.GetValueType() == typeof(float))
                value = property.GetValueAs<float>().ToString(CultureInfo.InvariantCulture);

            sb.Append(value + "}");
            propertySrc = sb.ToString();
            return true;
        }

        protected virtual bool IsValidSource(string src)
        {
            if (string.IsNullOrEmpty(src) || src.Length <= 2)
                return false;
            return true;
        }
    }
}