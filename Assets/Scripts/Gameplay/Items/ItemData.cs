using Arctic.Serialization.Properties;
using System.Collections.Generic;
using UnityEngine;
using UnityObject = UnityEngine.Object;

namespace Arctic.Gameplay.Item
{
    [CreateAssetMenu(fileName = "NewItemData", menuName = "Items/Item Data", order = -999)]
    public class ItemData : ScriptableObject
    {
        public string GUID => guid;

        [Header("Identity")]
        [SerializeField] private string guid;
        [SerializeField] private bool randomGuid;

        [Header("Item Propeties")]
        [SerializeField] private List<Property<string>> stringProperties;
        [SerializeField] private List<Property<bool>> boolProperties;
        [SerializeField] private List<Property<int>> intProperties;
        [SerializeField] private List<Property<float>> floatProperties;
        [SerializeField] private List<Property<GameObject>> prefabProperties;
        [SerializeField] private List<Property<UnityObject>> unityObjectProperties;

        private Dictionary<string, IProperty> unifiedPropertyLookup;
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
        public void SetRandomGUID() => SetGUID(GenerateRandomGUID());

        public TValue GetPropertyValue<TValue>(string key)
        {
            try
            {
                if (GetUnifiedPropertyLookup(false).TryGetValue(key, out var data))
                    return data.GetValueAs<TValue>();
            }
            catch (System.Exception e)
            {
                Debug.LogError("Could not retrieve property value AS specified type: " + e.Message);
            }
            return default;
        }

        public bool TryGetPropertyValue<TValue>(string key, out TValue value)
        {
            if (!GetUnifiedPropertyLookup(false).TryGetValue(key, out var data))
            {
                value = default;
                return false;
            }

            try
            {
                value = data.GetValueAs<TValue>();
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
        public Dictionary<string, IProperty> GetUnifiedPropertyLookup(bool rebuild)
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


        /// <summary>
        /// Updates the property list lookup to reflect the latest property changes.
        /// </summary>
        public void RebuildPropertyListLookup()
        {
            propertyListLookup = BuildPropertyListLookup();
        }

        public void RebuildLookups()
        {
            RebuildPropertyListLookup();
            RebuildUnifiedPropertyLookup();
        }

        public void InitPropertyLists() 
        {
            if(stringProperties == null)
                stringProperties = new List<Property<string>>();
            if (boolProperties == null)
                boolProperties = new List<Property<bool>>();
            if (intProperties == null)
                intProperties = new List<Property<int>>();
            if (floatProperties == null)
                floatProperties = new List<Property<float>>();
            if (prefabProperties == null)
                prefabProperties = new List<Property<GameObject>>();
            if (unityObjectProperties == null)
                unityObjectProperties = new List<Property<UnityObject>>();
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

        private Dictionary<System.Type, List<IProperty>> BuildPropertyListLookup()
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

        private Dictionary<string, IProperty> BuildUnifiedPropertyLookup()
        {
            Dictionary<string, IProperty> unifiedLookup = new();
            RegisterPropertyListsByType<string>(ref unifiedLookup);
            RegisterPropertyListsByType<bool>(ref unifiedLookup);
            RegisterPropertyListsByType<int>(ref unifiedLookup);
            RegisterPropertyListsByType<float>(ref unifiedLookup);
            RegisterPropertyListsByType<GameObject>(ref unifiedLookup);
            RegisterPropertyListsByType<UnityObject>(ref unifiedLookup);
            return unifiedLookup;
        }

        private void RegisterPropertyListsByType<TValue>(ref Dictionary<string, IProperty> refUnifiedLookup)
        {
            List<IProperty> propList = GetPropertyListLookup(rebuild: false)[typeof(TValue)];
            foreach (var data in propList)
            {
                if (refUnifiedLookup.ContainsKey(data.GetKey()))
                {
                    Debug.LogWarning($"Duplicate property keys (key: {data.GetKey()}) found. Even different types of properties (e.g bool, string, int, etc) cannot have the same key.");
                    continue;
                }
                refUnifiedLookup.Add(data.GetKey(), data);
            }
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

        public void ClearAllProperties()
        {
            stringProperties?.Clear();
            boolProperties?.Clear();
            intProperties?.Clear();
            floatProperties?.Clear();
            prefabProperties?.Clear();
            unityObjectProperties?.Clear();
            RebuildLookups();
        }

        public void SetData(string guid, IEnumerable<IProperty> properties) 
        {
            InitPropertyLists();
            SetGUID(guid);
            foreach (var prop in properties)
                TryAddPropperty(prop, true);
            RebuildLookups();
        }

        public bool TryAddPropperty(IProperty propertyToAdd, bool overwrite)
        {
            if (propertyToAdd.GetValueType() == typeof(string))
            {
                return TryAddPropertyOfType<string>(propertyToAdd, overwrite);
            }
            else if (propertyToAdd.GetValueType() == typeof(bool))
            {
                return TryAddPropertyOfType<bool>(propertyToAdd, overwrite);
            }
            else if (propertyToAdd.GetValueType() == typeof(int))
            {
                return TryAddPropertyOfType<int>(propertyToAdd, overwrite);
            }
            else if (propertyToAdd.GetValueType() == typeof(float))
            {
                return TryAddPropertyOfType<float>(propertyToAdd, overwrite);
            }
            else
            {
                Debug.LogError($"Error: Could not add item property of type <{propertyToAdd.GetValueType().FullName}>");
                return false;
            }

        }

        private bool TryAddPropertyOfType<TValue>(IProperty propToAdd, bool overwrite)
        {
            List<Property<TValue>> sharedProperties = GetSharedPropertiesOfType<TValue>();

            if (sharedProperties == null)
                return false;

            for (int i = 0; i < sharedProperties.Count; i++)
            {
                if (sharedProperties[i].GetKey() == propToAdd.GetKey())
                {
                    if (!overwrite) return false;
                    else return TryOverwriteValue(propToAdd, sharedProperties[i]);
                }
            }

            sharedProperties.Add(new Property<TValue>(propToAdd));
            return true;
        }

        private bool TryOverwriteValue(IProperty from, IProperty to)
        {
            if (from.GetValueType() != to.GetValueType())
            {
                Debug.LogWarning("Value-type mismatch: Properties with same keys have differnet value types. Duplicate keys should not exist.");
                return false;
            }

            try
            {
                to.SetValue(from.GetValue());
                return true;
            }
            catch (System.Exception e) 
            {
                throw new System.Exception($"Cannot overwrite value of (keys: {from?.GetKey()} |--->| {to?.GetKey()}) from " +
                    $"type<{from?.GetValueType()?.FullName}> to " +
                    $"type<{to?.GetValueType()?.FullName}> : " + e.Message);
            }
        }

        private List<Property<TValue>> GetSharedPropertiesOfType<TValue>() 
        {
            return
                typeof(TValue) == typeof(string) ? stringProperties as List<Property<TValue>> :
                typeof(TValue) == typeof(bool) ? boolProperties as List<Property<TValue>> :
                typeof(TValue) == typeof(int) ? intProperties as List<Property<TValue>> :
                typeof(TValue) == typeof(float) ? floatProperties as List<Property<TValue>> : 
                null;
        }
    }
}