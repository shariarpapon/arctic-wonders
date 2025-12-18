using UnityEditor;

namespace Arctic.Gameplay.Items.Editor
{
    public static class ItemDataEditorWindowHook
    {
        private const string WINDOW_TITLE = nameof(ItemData) + " Editor";
        private static ItemDataEditorWindow WindowInstance = null;
        public static bool IsWindowOpen =>  WindowInstance != null;

        [MenuItem("Tools/" + WINDOW_TITLE)]
        public static void ToolMenuItem()
        {
            Initialize(null);
        }

        [MenuItem("Assets/" + WINDOW_TITLE)]
        public static void AssetMenuItem()
        {
            if (Selection.activeObject is ItemData selected)
                Initialize(selected);
        }

        private static void Initialize(ItemData target)
        {
            if (!IsWindowOpen)
                WindowInstance = EditorWindow.GetWindow<ItemDataEditorWindow>(WINDOW_TITLE);

            WindowInstance.Focus();
            if (!WindowInstance.HasDataSource)
                WindowInstance.SetDataSource(target);
        }

    }
}