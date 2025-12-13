using UnityEditor;
using UnityEngine;
using Arctic.Utilities.Editor;
using Arctic.Utilities.Serialization.Json;
using System.Text;

namespace Arctic.Gameplay.Items.Editor
{
    public class ItemJsonEditorWindow : EditorWindow
    {
        private const string MENU_PATH = "Tools/Item Editor";
        private static ItemJsonEditorWindow Instance;

        private ItemDefinition target;
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
            GuiHelper.DrawObjectField("Target " + nameof(ItemDefinition), ref target);
            GuiHelper.DrawHorizontalLine(height: 1, spaceAbove: 3.5f);

            if (target != null)
                DrawTextEditor($"Item Editor<{target.GUID}>");
            else GuiWarn("Must asign a valid ItemDefinition scriptable object.");
        }

        private void DrawTextEditor(string title) 
        {
            GuiHelper.DrawHeaderLabel(title, fontSize: 11);

            DeserializableItemDefintion deserialized = new DeserializableItemDefintion(target);
            JsonItemDefinitionSerializer serializer = new JsonItemDefinitionSerializer();
            text = serializer.Serialize(deserialized).Object;

            GuiHelper.DrawTextEditorWindowArea(ref text);
        }

        private void GuiWarn(string message) => GuiHelper.DrawText(message, Color.orange);

    }
}   