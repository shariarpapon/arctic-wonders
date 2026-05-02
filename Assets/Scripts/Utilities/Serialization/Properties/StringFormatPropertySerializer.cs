using System.Collections.Generic;
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
        private IParser<string, IProperty> stringPropertyParser;

        public StringFormatPropertySerializer() 
        {
            SetParser(new StringPropertyParser());
        }

        public StringFormatPropertySerializer(IParser<string, IProperty> stringPropertyParser) 
        {
            SetParser(stringPropertyParser); 
        }

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

        public void SetParser(IParser<string, IProperty> parser) => stringPropertyParser = parser;

        protected virtual bool TryDeserializeProperty(string propertySrc, out IProperty property) 
        {
            property = null;

            if (!IsValidSource(propertySrc))
                return false;
            bool success = stringPropertyParser.TryParse(propertySrc, out property);
            return success;
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