using Arctic.Utilities;
using Arctic.Utilities.Editor;
using Arctic.Utilities.Serialization;
using Codice.Client.GameUI.Update;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Arctic.Gameplay.Items.Editor
{
    public sealed class ItemDataEditorWindow : EditorWindow
    {
        private const string WINDOW_TITLE = "ItemData Editor";
        private static ItemDataEditorWindow WindowInstance =null;
        private static string OutputText = string.Empty;
        private static int EditorTextFontSize = 12;

        private static readonly ISerializer<ItemData, string> ActiveItemDataSerializer = new ItemDataSerializer();
        private ItemData sourceItemData = null;

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
            if (WindowInstance == null)
                WindowInstance = GetWindow<ItemDataEditorWindow>(WINDOW_TITLE);

            WindowInstance.Focus();
            if(WindowInstance.sourceItemData == null)
            {
                WindowInstance.sourceItemData = target;
            }
        }

        private void OnDisable()
        {
            WindowInstance = null;
        }

        private void OnGUI()
        {
            string currentGuid = sourceItemData == null ? null : sourceItemData.GUID;
            GuiHelper.DrawObjectField("Target " + nameof(ItemData), ref sourceItemData);
            GuiHelper.DrawHorizontalLine(height: 1, spaceAbove: 3.5f);
            if ((currentGuid == null && sourceItemData != null) || (sourceItemData != null && currentGuid != sourceItemData.GUID)) 
                SerializeAndUpdateText();
            else if(sourceItemData != null) 
                UpdateTextEditor($"Item Editor<{sourceItemData.GUID}>", ref OutputText);
            else DrawWarning("Must asign a valid ItemData scriptable object.");
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

        private void DeserializeAndUpdateItemData() 
        {
            try
            {
                Deserialize(OutputText, ActiveItemDataSerializer);
                EditorUtility.SetDirty(sourceItemData);
                AssetDatabase.SaveAssets();
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(sourceItemData),ImportAssetOptions.ForceUpdate);
                AssetDatabase.Refresh();
            }
            catch (System.Exception e) 
            {
                Debug.LogException(e);
                return;
            }
        }

        private void Deserialize(string sourceString, ISerializer<ItemData, string> serializer)
        {
            var output = serializer.Deserialize(sourceString);
            if (output.Status == OutputStatus.Successful)
            {
                sourceItemData = output.Object;
                PrintConfirmation(false, output.Object.GUID);
                return;
            }
            else
                throw new System.InvalidOperationException($"Cannot deserialize <string> into <{nameof(ItemData)}> asset (status: {output.Status})");
        }

        private void SerializeAndUpdateText()
        {
            string serialized = Serialize(sourceItemData, ActiveItemDataSerializer);
            if(OutputText != serialized)
                SetOutputText(serialized);
        }

        public void SetOutputText(string text) 
        {
            GUI.FocusControl(null);
            OutputText = text;
            Repaint();
        }

        private string Serialize(ItemData source, ISerializer<ItemData, string> serializer)
        {
            var output = serializer.Serialize(source);
            if (output.Status == OutputStatus.Successful)
            {
                PrintConfirmation(true, source.GUID);
                return output.Object;
            }
            else
                throw new System.InvalidOperationException($"Cannot serialize <{nameof(ItemData)}> into <string>: (status: {output.Status})");
        }

        private void DrawWarning(string message) => GuiHelper.DrawText(message, UnityColorDatabase.PINK);


        #region Debug ##########################
        private void PrintConfirmation(bool serialized, string guid)
        {
            if (serialized)
                Debug.Log($"<color=cyan>SERIALIZED: </color> <{guid}>");
            else if (!serialized)
                Debug.Log($"<color=yellow>DESERIALIZED: </color> <{guid}>");
        }


        private static void PrintProperties(ItemData data, string title = "properties")
        {
            StringBuilder sb = new StringBuilder();
            foreach (var kv in data.GetUnifiedPropertyLookup(true))
            {
                if (kv.Value.GetValueType() == null)
                {
                    sb.Append($"########## key: {kv.Key}, value: {kv.Value.GetValue()}\n");
                    continue;
                }
                sb.Append($"key: {kv.Key}, value: {kv.Value.GetValue()}, type: {kv.Value.GetValueType()}");
            }
            Debug.Log($"<color=orange>==={title}===</color>");
            Debug.Log(sb.ToString());
        } 

        private static void PrintProperties(List<IProperty> properties, string title = "properties")
        {
            StringBuilder sb = new StringBuilder();
            foreach (var p in properties)
                sb.AppendLine($"key: {p.GetKey()}, value: {p.GetValue()}, type: {p.GetValueType()}");
            Debug.Log($"<color=orange>==={title}===</color>");
            Debug.Log(sb.ToString());
        }
        #endregion
    }
}  