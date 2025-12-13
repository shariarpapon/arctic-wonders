using Arctic.Utilities;
using Arctic.Utilities.Editor;
using Arctic.Utilities.Serialization;
using UnityEditor;
using UnityEngine;

namespace Arctic.Gameplay.Items.Editor
{
    public sealed class ItemJsonEditorWindow : EditorWindow
    {
        private const string MENU_PATH = "Tools/Item Editor";
        private static readonly ISerializer<DeserializableItemDefintion, string> itemDefSerializer = new JsonItemDefinitionSerializer();

        private ItemDefinition itemDefinition = null;
        private string text = string.Empty;

        [MenuItem(MENU_PATH)]
        public static void OpenWindow() 
        {
            GetWindow<ItemJsonEditorWindow>("Json Editor");
        }

        private void OnGUI()
        {
            string currentGuid = itemDefinition == null ? null : itemDefinition.GUID;
            GuiHelper.DrawObjectField("Target " + nameof(ItemDefinition), ref itemDefinition);
            GuiHelper.DrawHorizontalLine(height: 1, spaceAbove: 3.5f);
            if ((currentGuid == null && itemDefinition != null) || (itemDefinition != null && currentGuid != itemDefinition.GUID)) 
                SerializeAndUpdateText();
            else if(itemDefinition != null) 
                UpdateTextEditor($"Item Editor<{itemDefinition.GUID}>", ref text);
            else GuiWarn("Must asign a valid ItemDefinition scriptable object.");
        }

        private void UpdateTextEditor(string title, ref string textRef) 
        {
            GuiHelper.DrawHeaderLabel(title, fontSize: 11);
            GUILayout.BeginHorizontal();
            GuiHelper.DrawButton("Serialize", UnityColorDatabase.CYAN,UnityColorDatabase.WHITE, SerializeAndUpdateText);
            GuiHelper.DrawButton("Deserialize", UnityColorDatabase.YELLOW, UnityColorDatabase.WHITE, DeserializeAndUpdateItemDefinition);
            GUILayout.EndHorizontal();
            GuiHelper.DrawTextEditorWindowArea(ref textRef);
        }

        private void SerializeAndUpdateText() 
        {
            string serialized = Serialize(itemDefinition, itemDefSerializer);
            if (text != serialized) 
                text = serialized;
        }

        private void DeserializeAndUpdateItemDefinition() 
        {
            try
            {
                DeserializableItemDefintion deserialized = Deserialize(text, itemDefSerializer);
                Undo.RecordObject(itemDefinition, "deserialize_" + nameof(itemDefinition) + "_" + itemDefinition.GUID);
                deserialized.TryParseIntoSource(ref itemDefinition);
                EditorUtility.SetDirty(itemDefinition);                
            }
            catch (System.Exception e) 
            {
                Debug.LogException(e);
                return;
            }
        }

        private string Serialize(ItemDefinition source, ISerializer<DeserializableItemDefintion, string> serializer)
        {
            DeserializableItemDefintion deserializedSource = new DeserializableItemDefintion(source);
            var serialized = serializer.Serialize(deserializedSource);
            if (serialized.Status == SerializerStatus.Successful)
            {
                PrintConfirmation(true, deserializedSource);
                return serialized.Object;
            }

            GuiHelper.ContentColorSwitch(UnityColorDatabase.ORANGE,
              () => { EditorGUILayout.SelectableLabel($"Cannot serialize item defintion. (status: {serialized.Status})"); });
            return null;
        }

        private DeserializableItemDefintion Deserialize(string source, ISerializer<DeserializableItemDefintion, string> serializer)
        {
            var deserialized = serializer.Deserialize(source);
            if (deserialized.Status == SerializerStatus.Successful)
            {
                PrintConfirmation(false, deserialized.Object);
                return deserialized.Object;
            }
            throw new System.InvalidOperationException($"Cannot deserialize into item defintion. (status: {deserialized.Status})");
        }

        private void PrintConfirmation(bool serialized, DeserializableItemDefintion deserializedItemDef) 
        {
            string guid = deserializedItemDef.guid;
            if (serialized)
                Debug.Log($"<color=cyan>SERIALIZED: </color> <{guid}>");
            else if (!serialized)
                Debug.Log($"<color=yellow>DESERIALIZED: </color> <{guid}>");
        }

        private void GuiWarn(string message) => GuiHelper.DrawText(message, UnityColorDatabase.ORANGE);

    }
}  