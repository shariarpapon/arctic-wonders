namespace Arctic.Utilities.Serialization
{
    [System.Serializable]
    public struct SerializableKeyValueData
    {
        public object key;
        public object value;
        public SerializableKeyValueData(object key, object value)
        {
            this.key = key;
            this.value = value;
        }
    }
}