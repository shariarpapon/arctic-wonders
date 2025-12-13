using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
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

        private Dictionary<System.Type, object> itemPropertyListLookup;
        private Dictionary<string, object> itemPropertyLookup;

        public Dictionary<System.Type, object> PropertyListLookup
        {
            get
            {
                if (itemPropertyListLookup == null)
                    itemPropertyListLookup = BuildPropretyListLookup();
                return itemPropertyListLookup;
            }
        }
        public Dictionary<string, object> ItemPropertyLookup 
        {
            get 
            {
                if (itemPropertyLookup == null)
                    itemPropertyLookup = BuildItemPropertyLookup();
                return itemPropertyLookup;
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
        public bool TryGetPropertyValue<TValue>(string key, out TValue propertyValue) 
        {
            propertyValue = GetValue<TValue>(key);
            return propertyValue != null;
        }

        public TValue GetValue<TValue>(string key)
        {
            try 
            {
                if (ItemPropertyLookup.TryGetValue(key, out var prop))
                    return (TValue)prop;
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

        private Dictionary<string, object> BuildItemPropertyLookup() 
        {
            Dictionary<string, object> lookup = new();
            AddPropretyListToLookup<string>(ref lookup);
            AddPropretyListToLookup<bool>(ref lookup);
            AddPropretyListToLookup<int>(ref lookup);
            AddPropretyListToLookup<float>(ref lookup);
            AddPropretyListToLookup<GameObject>(ref lookup);
            AddPropretyListToLookup<UnityObject>(ref lookup);
            return lookup;
        }

        private void AddPropretyListToLookup<TValue>(ref Dictionary<string, object> lookup) 
        {
            if(lookup == null)
                lookup = new();
            var typeList = PropertyListLookup[typeof(TValue)];
            var propertyList = typeList as List<ItemProperty<TValue>>;
            foreach (var prop in propertyList)
            {
                if (lookup.ContainsKey(prop.Key)) 
                {
                    Debug.LogError($"Duplicate item property key found: (key: {prop.Key})  (guid: {guid}). Ignoring all except for the first property.");
                    continue;
                }
                lookup.Add(prop.Key, prop.Value);
            }
        }   
    }
}