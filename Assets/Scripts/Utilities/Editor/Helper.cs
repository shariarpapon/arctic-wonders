using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Arctic.Utilities.Editor
{
    //Resources
    public static partial class Helper
    {
        public static TObject LoadResource<TObject>(string path) where TObject : Object
        {
            TObject resource = Resources.Load<TObject>(path);
            if (resource == null)
            {
                Debug.LogError($"Failed to load resource at path: {path}");
            }
            return resource;
        }

        public static TObject[] LoadAllResources<TObject>(string path) where TObject : Object
        {
            TObject[] resources = Resources.LoadAll<TObject>(path);
            if (resources == null || resources.Length == 0)
            {
                Debug.LogError($"Failed to load resources at path: {path}");
            }
            return resources;
        }

        public static bool TryLoadAssetsOfType<T>(out List<T> assetList) where T : UnityEngine.Object
        {
            assetList = new List<T>();
            string[] assetGuids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            if (assetGuids == null || assetGuids.Length <= 0)
            {
                throw new System.Exception($"No assets with sepcified type<{typeof(T).Name}> found.");
            }

            foreach (string guid in assetGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                T asset = AssetDatabase.LoadAssetAtPath<T>(path);
                if (asset != null)
                    assetList.Add(asset);
            }
            return true;
        }

        public static bool TryFindAssetOfType<T>(out T asset, System.Func<T, bool> predicate = null) where T : UnityEngine.Object
        {
            asset = null;
            string[] assetGuids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
            if (assetGuids == null || assetGuids.Length <= 0)
                return false;

            foreach (string guid in assetGuids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                asset = AssetDatabase.LoadAssetAtPath<T>(path);
                if (predicate != null)
                    return predicate(asset);
                return true;
            }
            return true;
        }
        
        public static void CreateAssetAtPath<T>(T asset, string pathLocation) where T : UnityEngine.Object
        {
            AssetDatabase.CreateAsset(asset, pathLocation);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void CommitAssetChanges(Object asset)
        {
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(
                AssetDatabase.GetAssetPath(asset),
                ImportAssetOptions.ForceUpdate);
            AssetDatabase.Refresh();
        }

        public enum BrowseFilter { All = 0, Text, PNG, JSON, None }

        public static bool BrowseFilesystem(ref string path, BrowseFilter browseFilter = BrowseFilter.All)
        {
            string extension = GetExtension(browseFilter);
            string selected = EditorUtility.OpenFilePanel("Select", Application.dataPath, extension);

            if (!string.IsNullOrEmpty(selected))
            {
                path = selected;
                return true;
            }

            Debug.LogWarning("No valid paths selected");
            return false;
        }


        private static string GetExtension(BrowseFilter browserFilter)
        {
            switch (browserFilter)
            {
                default:
                case BrowseFilter.All:
                    return "*";
                case BrowseFilter.Text:
                    return "txt";
                case BrowseFilter.PNG:
                    return "png";
                case BrowseFilter.JSON:
                    return "json";
                case BrowseFilter.None:
                    return "";
            }
        }
    }
}