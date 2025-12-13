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

        [Header("Item Propeties")]
        [SerializeField] private List<ItemProperty<string>> stringProperties;
        [SerializeField] private List<ItemProperty<bool>> boolProperties;
        [SerializeField] private List<ItemProperty<int>> intProperties;
        [SerializeField] private List<ItemProperty<float>> floatProperties;
        [SerializeField] private List<ItemProperty<GameObject>> prefabProperties;
        [SerializeField] private List<ItemProperty<UnityObject>> unityObjectProperties;


        private Dictionary<System.Type, object> propertyListLookup;
        /// <summary>
        /// The object value here is guranteed to be of type ItemProperty.
        /// </summary>
        private Dictionary<string, ItemPropertyData> unifiedPropertyDataLookup;

        /// <summary>
        /// Lookup dictionary for retrieving the property list given the value-type.
        /// </summary>
        public Dictionary<System.Type, object> PropertyListLookup
        {
            get
            {
                if (propertyListLookup == null)
                    propertyListLookup = BuildPropretyListLookup();
                return propertyListLookup;
            }
        }

        /// <summary>
        /// Property lists for all types unified into a single lookup dictionary.
        /// </summary>
        public Dictionary<string, ItemPropertyData> UnifiedPropertyDataLookup 
        {
            get 
            {
                if (unifiedPropertyDataLookup == null)
                    unifiedPropertyDataLookup = BuildUnifiedPropertyLookup();
                return unifiedPropertyDataLookup;
            }
        }

        protected virtual void OnValidate()
        {
            if (randomGuid)
            {
                randomGuid = false;
                SetGUID(GenerateRandomGUID());
            }
        }

        public static string GenerateRandomGUID() => System.Guid.NewGuid().ToString("N");

        public void SetGUID(string guid) => this.guid = guid;

        /// <returns>True if a valid output value was retrived from the property with the given key, false otherwise.</returns>
        public bool TryGetPropertyValue<TValue>(string key, out TValue propertyValue) 
        {
            propertyValue = GetPropertyValue<TValue>(key);
            return propertyValue != null;
        }

        public TValue GetPropertyValue<TValue>(string key)
        {
            try 
            {
                if (UnifiedPropertyDataLookup.TryGetValue(key, out var data))
                    return data.ValueAs<TValue>();
                else
                    return default;
            }
            catch (System.Exception e) 
            {
                Debug.LogError("Could not retrive property value as specified type.");
                Debug.LogException(e);
                return default;
            }
        }

        private Dictionary<System.Type, object> BuildPropretyListLookup()
        {
            return new()
            {
                { typeof(string), stringProperties },
                { typeof(bool), boolProperties },
                { typeof(int), intProperties},
                { typeof(float), floatProperties },
                { typeof(GameObject), prefabProperties },
                { typeof(UnityObject), unityObjectProperties }
            };
        }

        private Dictionary<string, ItemPropertyData> BuildUnifiedPropertyLookup() 
        {
            Dictionary<string, ItemPropertyData> lookup = new();
            AddPropertyListToLookup<string>(ref lookup);
            AddPropertyListToLookup<bool>(ref lookup);
            AddPropertyListToLookup<int>(ref lookup);
            AddPropertyListToLookup<float>(ref lookup);
            AddPropertyListToLookup<GameObject>(ref lookup);
            AddPropertyListToLookup<UnityObject>(ref lookup);
            return lookup;
        }

        private void AddPropertyListToLookup<TValue>(ref Dictionary<string, ItemPropertyData> lookup) 
        {
            if(lookup == null)
                lookup = new();
            var typeList = PropertyListLookup[typeof(TValue)];
            var propertyList = typeList as List<ItemProperty<TValue>>;
            foreach (var property in propertyList)
            {
                if (lookup.ContainsKey(property.Key)) 
                {
                    Debug.LogError($"Duplicate item property key found: (key: {property.Key})  (guid: {guid}). Ignoring all except for the first property.");
                    continue;
                }
                ItemPropertyData propertyData = new ItemPropertyData(property.Key, property.Value, property.ValueType);
                lookup.Add(property.Key, propertyData);
            }
        }

        public List<ItemProperty<TValue>> GetPropertyList<TValue>() 
        {
            System.Type type = typeof(TValue);
            if (PropertyListLookup.ContainsKey(type) == false)
                return null;
            try
            {
                return PropertyListLookup[type] as List<ItemProperty<TValue>>;
            }
            catch(System.Exception e) 
            {
                Debug.LogException(e);
                return null;
            }
        }

        public bool TryAddProperty(ItemPropertyData data) 
        {
            if (data.type == typeof(string))
                return TryAddPropertyOfType<string>(data);
            else if (data.type == typeof(bool))
                return TryAddPropertyOfType<bool>(data);
            else if (data.type == typeof(int))
                return TryAddPropertyOfType<int>(data);
            else if (data.type == typeof(float))
                return TryAddPropertyOfType<float>(data);
            else return false;
        }

        public bool TryAddPropertyOfType<TValue>(ItemPropertyData data) 
        {
            System.Type type = typeof(TValue);
            if (!PropertyListLookup.ContainsKey(type))
            {
                Debug.LogError($"ItemDefinition does not support properties of type <{data.type.FullName}>");
                return false;
            }
            try
            {
                List<ItemProperty<TValue>> list = PropertyListLookup[type] as List<ItemProperty<TValue>>;
                if (list != null)
                {
                    if (list.Find(c => c.Key == data.key) != null)
                    {
                        list.Add(new ItemProperty<TValue>(data));
                        return true;
                    }
                }
                return false;
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

    }
}