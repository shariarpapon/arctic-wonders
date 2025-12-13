using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Arctic.Utilities.Serialization.Json
{
    public sealed class JsonKeyValueSerializer : KeyValueSerializer
    {
        public override SerializerOutput<SerializableKeyValue> Deserialize(string jsonData)
        {
            try 
            {
                SerializableKeyValue data = JsonUtility.FromJson<SerializableKeyValue>(jsonData);
                return new SerializerOutput<SerializableKeyValue>(data, SerializerStatus.Successful);
            }
            catch (System.Exception e) 
            {
                Debug.LogException(e);
                return new SerializerOutput<SerializableKeyValue>(default, SerializerStatus.Failed);
            }
        }

        public override SerializerOutput<string> Serialize(SerializableKeyValue deserialized)
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

        public SerializerOutput<string> Serialize<K,V>(Dictionary<K,V> dictionary) 
        {
            StringBuilder sb = new StringBuilder();
            foreach (var kv in dictionary) 
            {
                try 
                {
                    SerializableKeyValue skv = new SerializableKeyValue(kv.Key, kv.Value);
                    var serializerOutput = Serialize(skv);
                    if (serializerOutput.Status == SerializerStatus.Successful)
                        sb.AppendLine(serializerOutput.Object);
                    else
                        Debug.LogError($"Could not serialize key-value data. ({serializerOutput.Object})");
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                    return new SerializerOutput<string>("ERROR: " + e.Message, SerializerStatus.Failed); ;
                }
            }
            return new SerializerOutput<string>(sb.ToString(), SerializerStatus.Successful);
        }

        public SerializerOutput<string> Serialize(IEnumerable<SerializableKeyValue> enumerable) 
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                foreach (var kv in enumerable) 
                {
                    var serializerOutput = Serialize(kv);
                    if (serializerOutput.Status == SerializerStatus.Successful)
                        sb.AppendLine(serializerOutput.Object);
                    else
                        Debug.LogError($"Could not serialize key-value data. ({serializerOutput.Object})");
                }
                return new SerializerOutput<string>(sb.ToString(), SerializerStatus.Successful); ;
            }
            catch (System.Exception e) 
            {
                Debug.LogException(e); 
                return new SerializerOutput<string>("ERROR: " + e.Message, SerializerStatus.Failed); ;
            }
        }
    }   
}