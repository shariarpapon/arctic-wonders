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

            //TEST
            JsonPropertySerializer srz = new JsonPropertySerializer();
            JsonProperty prop = new JsonProperty("test_id", 3.141f, typeof(float));
            var serialized = srz.Serialize(prop);
            string json = serialized.Object;
            EditorGUILayout.SelectableLabel(json);
            EditorGUILayout.Separator();

            json = json.Replace("test_id", "bid").Replace("3.141", "true");
            var deserialized = srz.Deserialize(json);
            JsonProperty property = deserialized.Object;
            EditorGUILayout.SelectableLabel(property.id+"--"+property.ValueAs<bool>().ToString()+"--"+prop.type.FullName);
            //TEST^


            if (target != null)
                DrawTextEditor($"Item Editor<{target.GUID}>");
            else GuiWarn("Must asign a valid ItemDefinition scriptable object.");
        }

        private void DrawTextEditor(string title) 
        {
            GuiHelper.DrawHeaderLabel(title, fontSize: 11);

            SerializableItemDefinition serItemDef = new SerializableItemDefinition(target);
            text = serItemDef.ToJsonString();

            GuiHelper.DrawTextEditorWindowArea(ref text);
        }

        private void GuiWarn(string message) => GuiHelper.DrawText(message, Color.orange);

    }
}   