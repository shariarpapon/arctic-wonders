using System;
using System.Collections.Generic;

namespace Arctic.Utilities.Serialization
{
    public interface ISerializer<TDesrialized, TSerialized>
    {
        public Output<TSerialized> Serialize(TDesrialized deserialized);
        public Output<TDesrialized> Deserialize(TSerialized serialized);

        public virtual bool TrySerializeAll(IEnumerable<TDesrialized> enumerable, out List<TSerialized> serializedList)
        {
            serializedList = new List<TSerialized>();
            try
            {
                foreach (TDesrialized deserialized in enumerable)
                {
                    var output = Serialize(deserialized);
                    if (output.Status == OutputStatus.Successful)
                        serializedList.Add(output.Object);
                }
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public virtual bool TryDeserializeAll(IEnumerable<TSerialized> enumerable, out List<TDesrialized> deserializedList)
        {
            deserializedList = new List<TDesrialized>();
            try
            {
                foreach (TSerialized serialized in enumerable) 
                {
                    var output = Deserialize(serialized);
                    if (output.Status == OutputStatus.Successful)
                        deserializedList.Add(output.Object);
                }
                return true;
            }
            catch(Exception e)
            {
                throw e;
            }
        }
    }
}