using Arctic.Utilities;
using Arctic.Utilities.Editor;
using Arctic.Utilities.Serialization;
using UnityEditor;
using UnityEngine;

// TODO:
/* __ Add system to dynamically create item data asset if GUID doesnt exist during deserialization
 * __ Add system to load external files via editor window
 * __ Add "Options" tab for settings such as: font-size, asset-create-path, allow-dynamic-creation,  
 * __ Modularize GUI contents (as much as possible)
 * __ Implement parsing system so a serializer can have different parsers injected.
 * 
 * 
 */

namespace Arctic.Gameplay.Items.Editor
{
    public sealed class ItemDataEditorWindow : EditorWindow
    {
        private static int EditorTextFontSize = 12;
        private const int DEFAULT_BUTTON_HEIGHT = 25;

        private static readonly ISerializer<ItemData, string> CurrentSerializer = new ItemDataSerializer();

        private ItemData sourceItemData = null;
        private string sourceGuidInput = null;
        private string sourceEditText = string.Empty;
        public bool HasDataSource => sourceItemData != null;

        public void SetDataSource(ItemData target) 
        {
            sourceItemData = target;
            SerializeButtonEvent();
        }

        private void OnGUI()
        {
            DrawSourceEditor(ref sourceItemData);
        }

        private void DrawSourceEditor(ref ItemData sourceData) 
        {
            GuiHelper.DrawObjectField("Target " + nameof(ItemData), ref sourceData);
            GuiHelper.DrawHorizontalLine(height: 1, spaceAbove: 3.5f);

            if (!HasDataSource) 
            {
                DrawWarning("Must asign a source ItemData scriptable object");
                return;
            }

            string guid = sourceData.GUID;
            if (string.IsNullOrEmpty(guid))
            {
                HandleInvalidSourceGuid(ref sourceData);
                return;
            }

            DrawTextEditor(ref sourceEditText);
        }

        private void HandleInvalidSourceGuid(ref ItemData sourceData) 
        {
            DrawWarning("Source data does not have a valid GUID.\nGenerate random guid?");
            GuiHelper.DrawButton("Generate Random GUID", sourceData.SetRandomGUID, height : DEFAULT_BUTTON_HEIGHT);
            DrawWarning("Alternatively, set one manually: ");
            EditorGUILayout.BeginHorizontal();
            sourceGuidInput = EditorGUILayout.TextField(sourceGuidInput);
            if (GUILayout.Button("Set") && !string.IsNullOrEmpty(sourceGuidInput))
                sourceData.SetGUID(sourceGuidInput);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawTextEditor(ref string textRef) 
        {
            GUILayout.BeginHorizontal();
            GuiHelper.DrawButton("Serialize", SerializeButtonEvent, height : DEFAULT_BUTTON_HEIGHT);
            GuiHelper.DrawButton("Deserialize", DeserializeButtonEvent, height : DEFAULT_BUTTON_HEIGHT);
            GUILayout.EndHorizontal();
            EditorTextFontSize = EditorGUILayout.IntSlider("Font Size", EditorTextFontSize, 1, 100);
            GuiHelper.DrawTextEditorWindowArea(ref textRef, fontSize: EditorTextFontSize);
        }


        private void SerializeButtonEvent()
        {
            Serialize(sourceItemData, CurrentSerializer);
        }

        private void DeserializeButtonEvent() 
        {
            Deserialize(sourceEditText, CurrentSerializer);
        }

        private void Serialize(ItemData source, ISerializer<ItemData, string> serializer)
        {
            var output = serializer.Serialize(source);
            if (output.Status == OutputStatus.Successful)
            {
                string serialized = output.Object;
                SetSourceEditorText(serialized);
                PrintConfirmation(true, source.GUID);
            }
            else Debug.LogError($"Cannot serialize <{nameof(ItemData)} : {source?.GUID}> into <string>: (status: {output.Status})");
        }

        private void Deserialize(string sourceString, ISerializer<ItemData, string> deserializer)
        {
            var output = deserializer.Deserialize(sourceString);
            if (output.Status == OutputStatus.Successful)
            {
                sourceItemData = output.Object;
                PrintConfirmation(false, output.Object.GUID);
                CommitChanges(sourceItemData);
            }
            else Debug.LogError($"Cannot deserialize <string> into <{nameof(ItemData)}> asset (status: {output.Status})");
        }

        private void CommitChanges(Object asset) 
        {
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(sourceItemData), ImportAssetOptions.ForceUpdate);
            AssetDatabase.Refresh();
        }

        private void SetSourceEditorText(string text) 
        {
            GUI.FocusControl(null);
            sourceEditText = text;
            Repaint();
        }

        private void PrintConfirmation(bool serialized, string guid)
        {
            if (serialized)
                Debug.Log($"__serialized (guid: {guid})__");
            else if (!serialized)
                Debug.Log($"__deserialized (guid: {guid})__");
        }

        private void DrawWarning(string message) => GuiHelper.DrawText(message, UnityColorDatabase.soft_blue);
    }
}  