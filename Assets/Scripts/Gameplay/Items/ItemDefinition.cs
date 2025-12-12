using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Arctic.Gameplay.Items
{
    [CreateAssetMenu(fileName = "ItemDefinition", menuName = "Items/Item Definition", order = -999)]
    public class ItemDefinition : ScriptableObject
    {
        public string GUID => guid;

        [Header("Identity")]
        [SerializeField] private bool randomGuid;
        [SerializeField] private string guid;
        [SerializeField] private string displayName;

        [Header("Item Propeties")]
        [SerializeField] private List<ItemProperty<string>> stringProperties;
        [SerializeField] private List<ItemProperty<bool>> boolProperties;
        [SerializeField] private List<ItemProperty<int>> intProperties;
        [SerializeField] private List<ItemProperty<float>> floatProperties;
        [SerializeField] private List<ItemProperty<GameObject>> gameObjectProperties;
        [SerializeField] private List<ItemProperty<UnityObject>> unityObjectProperties;


        protected virtual void OnValidate()
        {
            if (randomGuid)
            {
                randomGuid = false;
                guid = System.Guid.NewGuid().ToString("N");
            }
        }

        /// <returns>True if a valid output value was retrived from the property with the given key, false otherwise.</returns>
        public bool TryGetValue<TValue>(string key, out TValue value) 
        {
            value = default;
            if (!TryGetProperty(key, out ItemProperty<TValue> property))
                return false;

            try
            {
                value = property.Value;
                return true;
            }
            catch (System.Exception exception) 
            {
                Debug.LogError($"ERROR: Unable to convert retrieved value to specified type. (key: {key})\n" + exception.Message);
                return false;
            }
        }

        public bool TryGetProperty<TValue>(string key, out ItemProperty<TValue> property) 
        {
            property = GetProperty<TValue>(key);
            return property != null;
        }

        public ItemProperty<TValue> GetProperty<TValue>(string key)
        {
            System.Type valueType = typeof(TValue);
            object outputProperty = default;
            if (valueType == typeof(string))
                outputProperty = stringProperties?.Find(c => c.Key == key);
            else if (valueType == typeof(bool))
                outputProperty = boolProperties?.Find(c => c.Key == key);
            else if (valueType == typeof(int))
                outputProperty = intProperties?.Find(c => c.Key == key);
            else if (valueType == typeof(float))
                outputProperty = floatProperties?.Find(c => c.Key == key);
            else if (valueType == typeof(GameObject))
                outputProperty = gameObjectProperties.Find(c => c.Key == key);
            else if (typeof(UnityObject).IsAssignableFrom(valueType))
                outputProperty = unityObjectProperties?.Find(c => c.Key == key);
            else 
            {
                Debug.LogError("Could not find a list for specified property type in ItemDefinition.");
                return null;
            }
            return outputProperty as ItemProperty<TValue>; 
        }
    }
}