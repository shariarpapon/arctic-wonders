using UnityEditor;
using UnityEngine;

namespace Arctic.Gameplay.Items.Editor
{
    public class ItemJsonEditorWindow : EditorWindow
    {
        private const string MENU_PATH = "Tools/Item Editor";
        private static ItemJsonEditorWindow Instance;

        private ItemDefinition itemDefinition;
        private string text;

        [MenuItem(MENU_PATH)]
        public static void OpenWindow() 
        {
            if(Instance == null)
                Instance = GetWindow<ItemJsonEditorWindow>("Json Editor");

            Instance.Show();
        }

        private void OnGUI()
        {
            Helper.DrawObjectField(nameof(ItemDefinition), ref itemDefinition);
            if (itemDefinition != null)
                DrawMainSection($"Item Editor<{itemDefinition.GUID}>");
            else Warn("Must asign a valid ItemDefinition scriptable object.");
        }

        private void DrawMainSection(string title) 
        {
            Helper.DrawHorizontalLine(height: 1, spaceAbove: 3.5f);
            Helper.DrawHeaderLabel(title, fontSize: 11);
            Helper.DrawTextEditorWindowArea(ref text);
        }

        private void Warn(string message) => Helper.DrawText(message, Color.orange);

    }
}   