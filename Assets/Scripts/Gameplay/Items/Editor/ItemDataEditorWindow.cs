using Arctic.Utilities;
using Arctic.Utilities.Editor;
using Arctic.Utilities.Serialization;
using Arctic.Utilities.Serialization.Json;
using UnityEditor;
using UnityEngine;

namespace Arctic.Gameplay.Items.Editor
{
    public sealed class ItemDataEditorWindow : EditorWindow
    {
        private const string WINDOW_TITLE = "ItemData Editor";
        private static ItemDataEditorWindow WindowInstance =null;
        private static string text = string.Empty;
        private static int EditorTextFontSize = 12;

        private static readonly ISerializer<ItemDataWrapper, string> ActiveItemDataSerializer = new ItemDataSerializer(new JsonPropertySerializer());
        private ItemData targetData = null;

        [MenuItem("Tools/" + WINDOW_TITLE)]
        public static void OpenWindow() 
        {
            Initialize(null);
        }

        [MenuItem("Assets/" + WINDOW_TITLE)]
        public static void AssetMenuItem()
        {
            if (Selection.activeObject is ItemData) 
            {
                ItemData selected = (ItemData)Selection.activeObject;
                Initialize(selected);
            }
        }

        public static void Initialize(ItemData target) 
        {
            if (WindowInstance.targetData == null && WindowInstance != null)
                WindowInstance.targetData = target;
            if (WindowInstance == null)
                WindowInstance = GetWindow<ItemDataEditorWindow>(WINDOW_TITLE);
            WindowInstance.Focus();
        }

        private void OnDisable()
        {
            WindowInstance = null;
        }

        private void OnGUI()
        {
            string currentGuid = targetData == null ? null : targetData.GUID;
            GuiHelper.DrawObjectField("Target " + nameof(ItemData), ref targetData);
            GuiHelper.DrawHorizontalLine(height: 1, spaceAbove: 3.5f);
            if ((currentGuid == null && targetData != null) || (targetData != null && currentGuid != targetData.GUID)) 
                SerializeAndUpdateText();
            else if(targetData != null) 
                UpdateTextEditor($"Item Editor<{targetData.GUID}>", ref text);
            else GuiWarn("Must asign a valid ItemData scriptable object.");
        }

        private void UpdateTextEditor(string title, ref string textRef) 
        {
            GuiHelper.DrawHeaderLabel(title, fontSize: 11);
            GUILayout.BeginHorizontal();
            GuiHelper.DrawButton("Serialize", UnityColorDatabase.CYAN,UnityColorDatabase.WHITE, SerializeAndUpdateText);
            GuiHelper.DrawButton("Deserialize", UnityColorDatabase.YELLOW, UnityColorDatabase.WHITE, DeserializeAndUpdateItemData);
            GUILayout.EndHorizontal();
            EditorTextFontSize = EditorGUILayout.IntSlider("Font Size", EditorTextFontSize, 1, 100);
            GuiHelper.DrawTextEditorWindowArea(ref textRef, fontSize: EditorTextFontSize);
        }

        private void SerializeAndUpdateText() 
        {
            string serialized = SerializeItemDataWrapper(targetData, ActiveItemDataSerializer);
            text = serialized;
        }

        private void DeserializeAndUpdateItemData() 
        {
            try
            {
                ItemDataWrapper deserialized = DeserializeItemDataWrapper(text, ActiveItemDataSerializer);
                Undo.RecordObject(targetData, "deserialize_" + nameof(targetData) + "_" + targetData.GUID);
                if (!deserialized.ApplyChangesToSource(ref targetData)) 
                    Debug.LogError("Could not apply changes to source ItemDaata.");
                EditorUtility.SetDirty(targetData);                
            }
            catch (System.Exception e) 
            {
                Debug.LogException(e);
                return;
            }
        }

        private string SerializeItemDataWrapper(ItemData source, ISerializer<ItemDataWrapper, string> serializer)
        {
            ItemDataWrapper deserializedSource = new ItemDataWrapper(source);
            var serialized = serializer.Serialize(deserializedSource);
            if (serialized.Status == OutputStatus.Successful)
            {
                PrintConfirmation(true, deserializedSource);
                return serialized.Object;
            }

            GuiHelper.ContentColorSwitch(UnityColorDatabase.ORANGE,
              () => { EditorGUILayout.SelectableLabel($"Cannot serialize item defintion. (status: {serialized.Status})"); });
            return null;
        }

        private ItemDataWrapper DeserializeItemDataWrapper(string source, ISerializer<ItemDataWrapper, string> serializer)
        {
            var deserialized = serializer.Deserialize(source);
            if (deserialized.Status == OutputStatus.Successful)
            {
                PrintConfirmation(false, deserialized.Object);
                return deserialized.Object;
            }
            throw new System.InvalidOperationException($"Cannot deserialize into {nameof(ItemDataWrapper)}. (status: {deserialized.Status})");
        }

        private void PrintConfirmation(bool serialized, ItemDataWrapper deserializedItemDef) 
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