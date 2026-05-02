using System;
using System.Globalization;

namespace Arctic.Utilities.Serialization.Properties
{
    public class StringPropertyParser : IParser<string, IProperty>
    { 
        public virtual bool TryParse(string src, out IProperty parsedProperty)
        {
            parsedProperty = default;
            try
            {
                src = src.Trim();
                src = src.Substring(1, src.Length - 2);

                int colonIndex = src.IndexOf(':');
                if (colonIndex < 0)
                    throw new Exception("Error: cannot parse property due to invalid syntax.");

                string key = src.Substring(0, colonIndex).Trim().Trim('"');
                string rawValue = src.Substring(colonIndex + 1).Trim();
                if (rawValue.StartsWith("\""))
                    parsedProperty = new Property<string>(key, rawValue.Trim('"'));
                else if (bool.TryParse(rawValue, out bool boolValue))
                    parsedProperty = new Property<bool>(key, boolValue);
                else if (int.TryParse(rawValue, out int intValue))
                    parsedProperty = new Property<int>(key, intValue);
                else if (float.TryParse(rawValue, NumberStyles.Float, CultureInfo.InvariantCulture, out float floatValue))
                    parsedProperty = new Property<float>(key, floatValue);
                else return false;
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}