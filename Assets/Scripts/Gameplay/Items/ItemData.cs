using Arctic.Utilities.Serialization;
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
        [SerializeField] private string guid;
        [SerializeField] private bool randomGuid;

        [Header("Item Propeties")]
        [SerializeField] private List<ItemProperty<string>> stringProperties;
        [SerializeField] private List<ItemProperty<bool>> boolProperties;
        [SerializeField] private List<ItemProperty<int>> intProperties;
        [SerializeField] private List<ItemProperty<float>> floatProperties;
        [SerializeField] private List<ItemProperty<GameObject>> prefabProperties;
        [SerializeField] private List<ItemProperty<UnityObject>> unityObjectProperties;

        
        private Dictionary<string, SerializableItemProperty> unifiedPropertyLookup;
        /// <summary>
        /// Property lists for all types unified into a single lookup dictionary.
        /// </summary>
        public Dictionary<string, SerializableItemProperty> UnifiedPropertyDataLookup
        {
            get
            {
                if (unifiedPropertyLookup == null)
                    unifiedPropertyLookup = BuildUnifiedPropertyLookup();
                return unifiedPropertyLookup;
            }
        }

        /// <summary>
        /// The object value here is guranteed to be of type List&lt;ItemProperty&gt;.
        /// </summary>
        private Dictionary<System.Type, List<IProperty>> propertyListLookup;
        /// <summary>
        /// Lookup dictionary for retrieving the property list given the value-type.
        /// </summary>
        public Dictionary<System.Type, List<IProperty>> PropertyListLookup
        {
            get
            {
                if (propertyListLookup == null)
                    propertyListLookup = BuildPropretyListLookup();
                return propertyListLookup;
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
        public bool TryGetPropertyValue<TValue>(string key, out TValue value)
        {
            if (!UnifiedPropertyDataLookup.TryGetValue(key, out var data))
            {
                value = default;
                return false;
            }

            try
            {
                value = data.ValueAs<TValue>();
                return true;
            }
            catch
            {
                value = default;
                return false;
            }
        }


        private Dictionary<System.Type, List<IProperty>> BuildPropretyListLookup()
        {
            return new()
            {
                { typeof(string), new(stringProperties) },
                { typeof(bool), new(boolProperties) },
                { typeof(int), new(intProperties) },
                { typeof(float), new(floatProperties) },
                { typeof(GameObject), new(prefabProperties) },
                { typeof(UnityObject), new(unityObjectProperties) }
            };
        }

        private Dictionary<string, SerializableItemProperty> BuildUnifiedPropertyLookup() 
        {
            Dictionary<string, SerializableItemProperty> unifiedLookup = new();
            AddPropertiesToUnifiedLookup<string>(ref unifiedLookup);
            AddPropertiesToUnifiedLookup<bool>(ref unifiedLookup);
            AddPropertiesToUnifiedLookup<int>(ref unifiedLookup);
            AddPropertiesToUnifiedLookup<float>(ref unifiedLookup);
            AddPropertiesToUnifiedLookup<GameObject>(ref unifiedLookup);
            AddPropertiesToUnifiedLookup<UnityObject>(ref unifiedLookup);
            return unifiedLookup;
        }

        private void AddPropertiesToUnifiedLookup<TValue>(ref Dictionary<string, SerializableItemProperty> unifiedLookup) 
        {
            if(unifiedLookup == null) 
                unifiedLookup = new();
            List<IProperty> propertyList = PropertyListLookup[typeof(TValue)];
            foreach (var property in propertyList)
            {
                if (unifiedLookup.ContainsKey(property.GetKey())) 
                {
                    Debug.LogError($"Duplicate item property key found: (key: {property.GetKey()})  (guid: {guid}). Ignoring all except for the first property.");
                    continue;
                }
                string propertyKey = property.GetKey();
                object propertyValue = property.GetValue();
                System.Type propertyType = property.GetValueType();
                SerializableItemProperty propertyData = new SerializableItemProperty(property.GetKey(), property.GetValue(), property.GetValueType());
                unifiedLookup.Add(property.GetKey(), propertyData);
            }
        }

        private bool TryAddItemPropertyOfType<TProp>(IProperty newProp, bool overwrite) where TProp : IProperty
        {
            if (newProp == null)
                return false;

            try
            {
                System.Type valueType = newProp.GetValueType();
                List<TProp> itemPropertyList = null;
                if (valueType == typeof(string))
                    itemPropertyList = stringProperties as List<TProp>;
                else if (valueType == typeof(bool))
                    itemPropertyList = boolProperties as List<TProp>;
                else if (valueType == typeof(int))
                    itemPropertyList = intProperties as List<TProp>;
                else if (valueType == typeof(float))
                    itemPropertyList = floatProperties as List<TProp>;
                else if (itemPropertyList == null)
                {
                    Debug.LogError($"Error: Could not interpret type of property value. (valueType: {valueType.FullName})");
                    return false;
                }

                for (int i = 0; i < itemPropertyList.Count; i++)
                {
                    TProp existingProp = itemPropertyList[i];
                    if (existingProp == null)
                        return false;
                    if (existingProp.GetKey() == newProp.GetKey())
                    {
                        if (overwrite)
                        {
                            itemPropertyList[i].Copy(existingProp);
                            return true;
                        }
                        else return false;
                    }
                }

                TProp t = (TProp)newProp;
                itemPropertyList.Add(t);
                return true;

            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }

        public List<IProperty> GetPropertyList<TValue>() 
        {
            System.Type type = typeof(TValue);
            if (!PropertyListLookup.ContainsKey(type))
                return null;
            try
            {
                return PropertyListLookup[type];
            }
            catch(System.Exception e) 
            {
                Debug.LogException(e);
                return null;
            }
        }

        public bool TryAddItemProperty(IProperty data, bool overwrite)
        {
            if (data.GetValueType() == typeof(string))
                return TryAddItemPropertyOfType<ItemProperty<string>>(data, overwrite);
            else if (data.GetValueType() == typeof(bool))
                return TryAddItemPropertyOfType<ItemProperty<bool>>(data, overwrite);
            else if (data.GetValueType() == typeof(int))
                return TryAddItemPropertyOfType<ItemProperty<int>>(data, overwrite);
            else if (data.GetValueType() == typeof(float))
                return TryAddItemPropertyOfType<ItemProperty<float>>(data, overwrite);
            else 
            {
                Debug.LogError($"Error: Could not add item property of type <{data.GetValueType().FullName}>");
                return false;
            }
        }

        //public bool TryAddPropertyOfType<TValue>(ItemPropertyData data, bool overwrite) 
        //{
        //    System.Type type = typeof(TValue);
        //    if (!PropertyListLookup.ContainsKey(type))
        //    {
        //        Debug.LogError($"ItemData does not support properties of type <{data.type.FullName}>");
        //        return false;
        //    }
        //    try
        //    {
        //        List<IProperty> propertyList = PropertyListLookup[type];
        //        if (propertyList != null)
        //        {
        //            for (int i = 0; i < propertyList.Count; i++)
        //            {
        //                if (propertyList[i].GetKey() == data.key)
        //                    if (overwrite)
        //                    {
        //                        propertyList[i] = new ItemProperty<TValue>(data);
        //                        return true;
        //                    }
        //            }
        //            propertyList.Add(new ItemProperty<TValue>(data));
        //            return true;
        //        }
        //        else 
        //        {
        //            Debug.LogError($"Property list of type<{data.type.FullName}> not found.");
        //        }
        //        return false;
        //    }
        //    catch (System.Exception e)
        //    {
        //        Debug.LogException(e);
        //        return false;
        //    }
        //}
    }
}