using System.Collections.Generic;

namespace Arctic.Utilities.Serialization
{
    public interface ISerializer<TDesrialized, TSerialized>
    {
        public Output<TSerialized> Serialize(TDesrialized deserialized);
        public Output<TDesrialized> Deserialize(TSerialized serialized);
    }
}