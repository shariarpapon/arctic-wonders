namespace Arctic.Utilities.Serialization
{
    [System.Serializable]
    public class SerializableKeyValue
    {
        public object key;
        public object value;

        public SerializableKeyValue(object key, object value)
        {
            this.key = key;
            this.value = value;

        }

        public bool TryGetKey<T>(out T key) 
        {
            key = default;
            try
            {
                key = GetKey<T>();
                return true;
            }
            catch 
            { 
                return false; 
            }
        }


        public bool TryGetValue<T>(out T value) 
        {
            value = default;
            try
            {
                value = GetValue<T>();
                return true;
            }
            catch 
            {
                return false; 
            }
        }

        public T GetKey<T>() => (T)key;
        public T GetValue<T>() => (T)value;
    }
}