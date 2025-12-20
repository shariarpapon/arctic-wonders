using UnityEditor;
using UnityEngine;

namespace Arctic.Utilities.Editor
{
    public static class Helper
    {
        public enum BrowseFilter { All=0, Text, PNG, JSON, None }

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