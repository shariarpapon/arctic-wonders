using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Arctic.Gameplay.Items
{
    [CreateAssetMenu(fileName = "NewItemData", menuName = "Items/Item Data", order = -999)]
    public class ItemData : ScriptableObject
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
                if (lookup.ContainsKey(property.GetKey())) 
                {
                    Debug.LogError($"Duplicate item property key found: (key: {property.GetKey()})  (guid: {guid}). Ignoring all except for the first property.");
                    continue;
                }
                string propertyKey = property.GetKey();
                object propertyValue = property.GetValue();
                System.Type propertyType = property.GetValueType();
                ItemPropertyData propertyData = new ItemPropertyData(property.GetKey(), property.GetValue(), property.GetValueType());
                lookup.Add(property.GetKey(), propertyData);
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

        public bool TryAddProperty(ItemPropertyData data, bool overwrite)
        {
            if (data.type == typeof(string))
                return TryAddPropertyOfType<string>(data, overwrite);
            else if (data.type == typeof(bool))
                return TryAddPropertyOfType<bool>(data, overwrite);
            else if (data.type == typeof(int))
                return TryAddPropertyOfType<int>(data, overwrite);
            else if (data.type == typeof(float))
                return TryAddPropertyOfType<float>(data, overwrite);
            else return false;
        }

        public bool TryAddPropertyOfType<TValue>(ItemPropertyData data, bool overwrite) 
        {
            System.Type type = typeof(TValue);
            if (!PropertyListLookup.ContainsKey(type))
            {
                Debug.LogError($"ItemData does not support properties of type <{data.type.FullName}>");
                return false;
            }
            try
            {
                List<ItemProperty<TValue>> propertyList = PropertyListLookup[type] as List<ItemProperty<TValue>>;
                if (propertyList != null)
                {
                    for (int i = 0; i < propertyList.Count; i++)
                    {
                        if (propertyList[i].GetKey() == data.key)
                            if (overwrite)
                            {
                                propertyList[i] = new ItemProperty<TValue>(data);
                                return true;
                            }
                    }
                    propertyList.Add(new ItemProperty<TValue>(data));
                    return true;
                }
                else 
                {
                    Debug.LogError($"Property list of type<{data.type.FullName}> not found.");
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