using Arctic.Utilities.Serialization;
using Arctic.Utilities.Serialization.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Arctic.Gameplay.Items.Editor
{
    [System.Serializable]
    public class SerializableItemDefinition
    {
        public string guid;

        [System.NonSerialized] public readonly ItemDefinition source;

        public SerializableItemDefinition(ItemDefinition source)
        {
            this.guid = source.GUID;
            this.source = source;
        }

        public SerializableItemDefinition() 
        {
            
        }

        public string ToJsonString() 
        {
            JsonPropertySerializer serializer = new JsonPropertySerializer();
            StringBuilder sb = new StringBuilder();
            foreach (var kv in source.UnifiedPropertyLookup) 
            {
                JsonProperty jsonProperty = new JsonProperty(kv.Key, kv.Value, kv.Value.GetType());
                var serialized = serializer.Serialize(jsonProperty);
                if (serialized.Status == SerializerStatus.Successful)
                    sb.AppendLine(serialized.Object);
            }
            return sb.ToString();
        }
    }
}