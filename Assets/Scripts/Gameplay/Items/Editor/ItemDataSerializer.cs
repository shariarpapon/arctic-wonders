using Arctic.Utilities.Serialization;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Arctic.Gameplay.Items.Editor
{
    public class ItemDataSerializer : ISerializer<SerializableItemData, string> 
    {
        private const string ITEM_GUID_KEY = "guid";
        private ISerializer<IProperty, string> propertySerializer;

        public ItemDataSerializer() 
        {
            this.propertySerializer = new BasicPropertySerializer();
        }

        public ItemDataSerializer(ISerializer<IProperty, string> propertySerializer) 
        {
            this.propertySerializer = propertySerializer;
        }

        public Output<string> Serialize(SerializableItemData itemData)
        {  
            try
            {
                IProperty itemGuidProperty = new GenericProperty<string>(ITEM_GUID_KEY, itemData.guid);
                itemData.properties.Add(itemGuidProperty);

                if (propertySerializer.TrySerializeAll(itemData.properties, out var serializedProperties)) 
                {
                    StringBuilder sb = new StringBuilder();
                    foreach(string serializedProp in serializedProperties)
                        sb.AppendLine(serializedProp);
                    return new Output<string>(sb.ToString(), OutputStatus.Successful);
                }
                else
                    return new Output<string>(null, OutputStatus.Failed);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return new Output<string>("ERROR: " + e.Message, OutputStatus.Failed);
            }
        }

        public Output<SerializableItemData> Deserialize(string serializedString)
        {
            string[] lines = serializedString.Split("\n");
            if (!propertySerializer.TryDeserializeAll(lines, out var deserializedProperties))
                return new Output<SerializableItemData>(default, OutputStatus.CouldNotDeserializeEnumerable);

            if (TryExtractGUIDFromDeserializedProperties(ref deserializedProperties, out string itemGuid)) 
            {
                SerializableItemData itemDataWrapper = new SerializableItemData(itemGuid, deserializedProperties);
                return new Output<SerializableItemData>(itemDataWrapper, OutputStatus.Successful);
            }
            else
                return new Output<SerializableItemData>(default, OutputStatus.Failed);
        }

        private static bool TryExtractGUIDFromDeserializedProperties(ref List<IProperty> deserializedProperties, out string itemGuid)
        {
            itemGuid = ItemData.GenerateRandomGUID();
            try
            {
                IProperty itemGuidProperty = deserializedProperties.Find(p => p.GetKey() == ITEM_GUID_KEY);
                string itemGuidValue = itemGuidProperty.ValueAs<string>();
                itemGuid = itemGuidValue;
                deserializedProperties.Remove(itemGuidProperty);
                return true;
            }
            catch (System.Exception e) 
            {
                Debug.LogError("Error: Could not extract item GUID from deserialized properties: " + e.Message);
                return false;
            }
        }
    }
}