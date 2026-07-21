using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Arctic.Foundation.Editor
{
    //Resources
    public static partial class ReasourceHelper
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
        
        public static void CreateAssetAtPath<T>(T asset, string assetFileName, string directory, bool createDirIfMissing = true) where T : UnityEngine.Object
        {
            if (!AssetDatabase.IsValidFolder(directory))
            {
                if (createDirIfMissing)
                    CreateDirectoryInAssets(directory);
                else 
                {
                    Debug.LogError("Could not create asset at path: "  + directory);
                    return;
                }
            }

            string finalPath = directory + "/" + assetFileName;
            AssetDatabase.CreateAsset(asset, finalPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void CreateDirectoryInAssets(string path) 
        {
            path = SanitizeUnityPath(path);
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogWarning("Invalid asset path provided.");
                return;
            }

            const string ROOT = "Assets/";
            if (path.StartsWith(ROOT))
                path = path.Remove(0, ROOT.Length);

            string[] folders = path.Split('/');
            path = "Assets";
            foreach (string folder in folders) 
            {
                if (string.IsNullOrEmpty(folder))
                {
                    Debug.LogError("Invalid folder name");
                    return;
                }
                string combinedPath = Path.Combine(path, folder);
                if (!AssetDatabase.IsValidFolder(combinedPath)) 
                { 
                    AssetDatabase.CreateFolder(path, folder);
                    path = combinedPath;
                }
            }
            
        }

        public static string SanitizeUnityPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return string.Empty;
            path = path.Replace("\\", "/");
            path = Regex.Replace(path, "/+", "/");
            if (path.Length > 1 && path.EndsWith("/"))
                path = path.TrimEnd('/');
            return path;
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