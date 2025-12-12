using System.Collections;
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

        private Dictionary<System.Type, System.Type> propertyTypeInterpretation;
        private Dictionary<System.Type, object> propertyTypeListLookup;
        private Dictionary<string, object> propertyLookup;

        public Dictionary<System.Type, System.Type> PropertyTypeInterpretation 
        {
            get 
            {
                if (propertyTypeInterpretation == null)
                    propertyTypeInterpretation = CreateItemPropertyTypeInterpretation();
                return propertyTypeInterpretation;
            }
        }
        public Dictionary<System.Type, object> PropertyTypeListLookup
        {
            get
            {
                if (propertyTypeListLookup == null)
                    propertyTypeListLookup = CreatePropertyTypeListLookup();
                return propertyTypeListLookup;
            }
        }
        public Dictionary<string, object> PropertyLookup 
        {
            get 
            {
                if (propertyLookup == null)
                    propertyLookup = CreatePropertyLookup();
                return propertyLookup;
            }
        }


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
            var list = GetPropertyListByType<TValue>();
            if (list != null)
                return list.Find(c => c.Key == key);
            return null;
        }

        private List<ItemProperty<TValue>> GetPropertyListByType<TValue>()
        {
            var type = typeof(TValue);
            if (PropertyTypeListLookup.ContainsKey(type))
                return PropertyTypeListLookup[type] as List<ItemProperty<TValue>>;
            else 
            {
                Debug.LogError("Could not find a list for specified property type in ItemDefinition.");
                return null;
            }
        }

        private Dictionary<System.Type, object> CreatePropertyTypeListLookup()
        {
            return new()
            {
                { typeof(string), stringProperties },
                { typeof(bool), boolProperties },
                { typeof(int), intProperties},
                { typeof(float), floatProperties },
                { typeof(GameObject), gameObjectProperties },
                { typeof(UnityObject), unityObjectProperties }
            };
        }

        private Dictionary<System.Type, System.Type> CreateItemPropertyTypeInterpretation()
        {
            return new()
            {
                { typeof(string),  typeof(ItemProperty<string>) },
                { typeof(bool), typeof(ItemProperty<bool>) },
                { typeof(int), typeof(ItemProperty<int>)},
                { typeof(float), typeof(ItemProperty<float>)},
                { typeof(GameObject), typeof(ItemProperty<GameObject>) },
                { typeof(UnityObject), typeof(ItemProperty<UnityObject>) }
            };
        }


        private Dictionary<string, object> CreatePropertyLookup() 
        {
            try
            {
                Dictionary<string, object> lookup = new Dictionary<string, object>();
                foreach (var listKV in PropertyTypeListLookup) 
                {
                    IEnumerable listEnum = (IEnumerable)listKV.Value;
                    foreach (var propKv in listEnum)
                    {
                    }
                }

                return lookup;
            }
            catch(System.Exception e)
            {
                Debug.LogException(e);
                return null;
            }
        }

    }
}