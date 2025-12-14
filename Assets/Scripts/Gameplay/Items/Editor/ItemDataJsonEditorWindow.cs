using Arctic.Utilities;
using Arctic.Utilities.Editor;
using Arctic.Utilities.Serialization;
using UnityEditor;
using UnityEngine;

namespace Arctic.Gameplay.Items.Editor
{
    public sealed class ItemDataJsonEditorWindow : EditorWindow
    {
        private static ItemDataJsonEditorWindow Instance =null;
        private static readonly ISerializer<ItemDataWrapper, string> itemDefSerializer = new JsonItemDataSerializer();
        private static ItemData TargetItemData = null;
        private static string text = string.Empty;
        private static int FontSize = 18;

        [MenuItem("Tools/ItemData Editor")]
        public static void OpenWindow() 
        {
            Initialize(TargetItemData);
        }

        [MenuItem("Assets/ItemData Editor")]
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
            TargetItemData = target;
            if (Instance == null)
                Instance = GetWindow<ItemDataJsonEditorWindow>("Json Editor");
        }

        private void OnDisable()
        {
            Instance = null;
        }

        private void OnGUI()
        {
            string currentGuid = TargetItemData == null ? null : TargetItemData.GUID;
            GuiHelper.DrawObjectField("Target " + nameof(ItemData), ref TargetItemData);
            GuiHelper.DrawHorizontalLine(height: 1, spaceAbove: 3.5f);
            if ((currentGuid == null && TargetItemData != null) || (TargetItemData != null && currentGuid != TargetItemData.GUID)) 
                SerializeAndUpdateText();
            else if(TargetItemData != null) 
                UpdateTextEditor($"Item Editor<{TargetItemData.GUID}>", ref text);
            else GuiWarn("Must asign a valid ItemData scriptable object.");
        }

        private void UpdateTextEditor(string title, ref string textRef) 
        {
            GuiHelper.DrawHeaderLabel(title, fontSize: 11);
            GUILayout.BeginHorizontal();
            GuiHelper.DrawButton("Serialize", UnityColorDatabase.CYAN,UnityColorDatabase.WHITE, SerializeAndUpdateText);
            GuiHelper.DrawButton("Deserialize", UnityColorDatabase.YELLOW, UnityColorDatabase.WHITE, DeserializeAndUpdateItemData);
            GUILayout.EndHorizontal();
            FontSize = EditorGUILayout.IntSlider("Font Size", FontSize, 1, 100);
            //GuiHelper.DrawTextEditorWindowArea(ref textRef, fontSize:FontSize);
            GuiHelper.DrawTextEditorWindowArea(ref textRef, fontSize: FontSize);
        }

        private void SerializeAndUpdateText() 
        {
            string serialized = Serialize(TargetItemData, itemDefSerializer);
            if (text != serialized) 
                text = serialized;
        }

        private void DeserializeAndUpdateItemData() 
        {
            try
            {
                ItemDataWrapper deserialized = Deserialize(text, itemDefSerializer);
                Undo.RecordObject(TargetItemData, "deserialize_" + nameof(TargetItemData) + "_" + TargetItemData.GUID);
                if (!deserialized.TryParseIntoSource(ref TargetItemData)) 
                    Debug.LogError("Could not parse into source item definition.");
                EditorUtility.SetDirty(TargetItemData);                
            }
            catch (System.Exception e) 
            {
                Debug.LogException(e);
                return;
            }
        }

        private string Serialize(ItemData source, ISerializer<ItemDataWrapper, string> serializer)
        {
            ItemDataWrapper deserializedSource = new ItemDataWrapper(source);
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

        private ItemDataWrapper Deserialize(string source, ISerializer<ItemDataWrapper, string> serializer)
        {
            var deserialized = serializer.Deserialize(source);
            if (deserialized.Status == SerializerStatus.Successful)
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