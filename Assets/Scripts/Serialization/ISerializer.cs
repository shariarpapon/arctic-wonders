using System;
using System.Collections.Generic;

namespace Arctic.Serialization
{
    public interface ISerializer<TDesrialized, TSerialized>
    {
        public Result<TSerialized> Serialize(TDesrialized deserialized);
        public Result<TDesrialized> Deserialize(TSerialized serialized);
        public bool TrySerializeAll(IEnumerable<TDesrialized> enumerable, out List<TSerialized> serializedList)
        {
            serializedList = new List<TSerialized>();
            try
            {
                foreach (TDesrialized deserialized in enumerable)
                {
                    var result = Serialize(deserialized);
                    if (result.Status == OutputStatus.Successful)
                        serializedList.Add(result.Object);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool TryDeserializeAll(IEnumerable<TSerialized> enumerable, out List<TDesrialized> deserializedList)
        {
            deserializedList = new List<TDesrialized>();
            try
            {
                foreach (TSerialized serialized in enumerable) 
                {
                    var result = Deserialize(serialized);
                    if (result.Status == OutputStatus.Successful)
                        deserializedList.Add(result.Object);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}