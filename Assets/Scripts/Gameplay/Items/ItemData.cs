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
        [SerializeField] private List<GenericProperty<string>> stringProperties;
        [SerializeField] private List<GenericProperty<bool>> boolProperties;
        [SerializeField] private List<GenericProperty<int>> intProperties;
        [SerializeField] private List<GenericProperty<float>> floatProperties;
        [SerializeField] private List<GenericProperty<GameObject>> prefabProperties;
        [SerializeField] private List<GenericProperty<UnityObject>> unityObjectProperties;

        private Dictionary<string, ExplicitProperty> unifiedPropertyLookup;

        /// <summary>
        /// The object value here is guranteed to be of type List&lt;ItemProperty&gt;.
        /// </summary>
        private Dictionary<System.Type, List<IProperty>> propertyListLookup;

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
                if (GetUnifiedPropertyDataLookup(false).TryGetValue(key, out var data))
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
            if (!GetUnifiedPropertyDataLookup(false).TryGetValue(key, out var data))
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
        /// <summary>
        /// Property lists for all types unified into a single lookup dictionary.
        /// </summary>
        public Dictionary<string, ExplicitProperty> GetUnifiedPropertyDataLookup(bool rebuild)
        {
            if (unifiedPropertyLookup == null || rebuild)
                RebuildUnifiedPropertyLookup();
            return unifiedPropertyLookup;
        }

        /// <summary>
        /// Updates the unified lookup to reflect the latest property changes.
        /// </summary>
        public void RebuildUnifiedPropertyLookup()
        {
            unifiedPropertyLookup = BuildUnifiedPropertyLookup();
        }

        public void RebuildPropertyListLookup()
        {
            propertyListLookup = BuildPropretyListLookup();
        }

        public void RebuildLookups()
        {
            RebuildPropertyListLookup();
            RebuildUnifiedPropertyLookup();
        }

        /// <summary>
        /// Lookup dictionary for retrieving the property list given the value-type.
        /// </summary>
        public Dictionary<System.Type, List<IProperty>> GetPropertyListLookup(bool rebuild)
        {
            if (propertyListLookup == null || rebuild)
                RebuildPropertyListLookup();
            return propertyListLookup;
        }

        private Dictionary<System.Type, List<IProperty>> BuildPropretyListLookup()
        {
            return new()
            {
                { typeof(string), new(stringProperties)},
                { typeof(bool), new(boolProperties)},
                { typeof(int), new(intProperties)},
                { typeof(float), new(floatProperties)},
                { typeof(GameObject), new(prefabProperties)},
                { typeof(UnityObject), new(unityObjectProperties)}
            };
        }

        private Dictionary<string, ExplicitProperty> BuildUnifiedPropertyLookup()
        {
            Dictionary<string, ExplicitProperty> unifiedLookup = new();
            AddPropertiesToUnifiedLookup<string>(ref unifiedLookup);
            AddPropertiesToUnifiedLookup<bool>(ref unifiedLookup);
            AddPropertiesToUnifiedLookup<int>(ref unifiedLookup);
            AddPropertiesToUnifiedLookup<float>(ref unifiedLookup);
            AddPropertiesToUnifiedLookup<GameObject>(ref unifiedLookup);
            AddPropertiesToUnifiedLookup<UnityObject>(ref unifiedLookup);
            return unifiedLookup;
        }

        private void AddPropertiesToUnifiedLookup<TValue>(ref Dictionary<string, ExplicitProperty> unifiedLookup)
        {
            if (unifiedLookup == null)
                unifiedLookup = new();
            List<IProperty> propertyList = GetPropertyListLookup(rebuild: false)[typeof(TValue)];
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
                ExplicitProperty propertyData = new ExplicitProperty(property.GetKey(), property.GetValue(), property.GetValueType());
                unifiedLookup.Add(property.GetKey(), propertyData);
            }
        }

        private bool TryAddItemPropertyOfType<TValue>(GenericProperty<TValue> newProp, bool overwrite)
        {
            List<GenericProperty<TValue>> list =
                typeof(TValue) == typeof(string) ? stringProperties as List<GenericProperty<TValue>> :
                typeof(TValue) == typeof(bool) ? boolProperties as List<GenericProperty<TValue>> :
                typeof(TValue) == typeof(int) ? intProperties as List<GenericProperty<TValue>> :
                typeof(TValue) == typeof(float) ? floatProperties as List<GenericProperty<TValue>> : null;

            if (list == null)
                return false;

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].GetKey() == newProp.GetKey())
                {
                    if (!overwrite)
                        return false;
                    list[i] = new GenericProperty<TValue>(newProp);
                    return true;
                }
            }

            list.Add(newProp);
            return true;
        }

        public List<IProperty> GetPropertyList<TValue>()
        {
            System.Type type = typeof(TValue);
            if (!GetPropertyListLookup(rebuild: false).ContainsKey(type))
                return null;
            try
            {
                return GetPropertyListLookup(rebuild: false)[type];
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

        public bool TryAddItemProperty(IProperty newProperty, bool overwrite)
        {
            if (newProperty.GetValueType() == typeof(string))
            {
                GenericProperty<string> itemPropertyRep = new GenericProperty<string>(newProperty);
                return TryAddItemPropertyOfType<string>(itemPropertyRep, overwrite);
            }
            else if (newProperty.GetValueType() == typeof(bool))
            {
                GenericProperty<bool> itemPropertyRep = new GenericProperty<bool>(newProperty);
                return TryAddItemPropertyOfType<bool>(itemPropertyRep, overwrite);
            }
            else if (newProperty.GetValueType() == typeof(int))
            {
                GenericProperty<int> itemPropertyRep = new GenericProperty<int>(newProperty);
                return TryAddItemPropertyOfType<int>(itemPropertyRep, overwrite);
            }
            else if (newProperty.GetValueType() == typeof(float))
            {
                GenericProperty<float> itemPropertyRep = new GenericProperty<float>(newProperty);
                return TryAddItemPropertyOfType<float>(itemPropertyRep, overwrite);
            }
            else
            {
                Debug.LogError($"Error: Could not add item property of type <{newProperty.GetValueType().FullName}>");
                return false;
            }

        }
    }
}