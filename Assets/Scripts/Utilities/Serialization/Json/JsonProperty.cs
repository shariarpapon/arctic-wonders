namespace Arctic.Utilities.Serialization.Json
{
    [System.Serializable] 
    public class JsonProperty
    {
        public readonly string id;
        public readonly object value;
        public readonly System.Type type;

        public JsonProperty(string id, object value, System.Type type) 
        {
            this.id = id;
            this.value = value;
            this.type = type;
        }
    }
}