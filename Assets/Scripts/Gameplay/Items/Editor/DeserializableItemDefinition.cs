using System.Collections.Generic;
using Arctic.Utilities.Serialization.Json;

namespace Arctic.Gameplay.Items.Editor
{
    public struct DeserializableItemDefintion
    {
        public string guid;

        [System.NonSerialized] public List<JsonProperty> properties;
        [System.NonSerialized] public ItemDefinition source;

        public DeserializableItemDefintion(ItemDefinition source)
        {
            this.guid = source.GUID;
            this.source = source;
            properties = null;
        }

        public void AddProperty(JsonProperty prop)
        {
            if (properties == null)
                properties = new List<JsonProperty>();
            properties.Add(prop);
        }
    }
}