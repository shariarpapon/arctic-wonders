using UnityEngine;

namespace Arctic.Utilities.Serialization
{
    public sealed class JsonKeyValueSerializer : KeyValueSerializer
    {
        public override SerializerOutput<SerializableKeyValueData> Deserialize(string jsonData)
        {
            try 
            {
                SerializableKeyValueData data = JsonUtility.FromJson<SerializableKeyValueData>(jsonData);
                return new SerializerOutput<SerializableKeyValueData>(data, SerializerStatus.Successful);
            }
            catch (System.Exception e) 
            {
                Debug.LogException(e);
                return new SerializerOutput<SerializableKeyValueData>(default, SerializerStatus.Failed);
            }
        }

        public override SerializerOutput<string> Serialize(SerializableKeyValueData deserialized)
        {
            try
            {
                string json = JsonUtility.ToJson(deserialized);
                return new SerializerOutput<string>(json, SerializerStatus.Successful);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return new SerializerOutput<string>(default, SerializerStatus.Successful);
            }
        }
    }
}