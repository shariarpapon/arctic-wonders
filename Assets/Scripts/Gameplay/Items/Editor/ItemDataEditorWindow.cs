using Arctic.Utilities;
using Arctic.Utilities.Editor;
using Arctic.Utilities.Serialization;
using Arctic.Utilities.Serialization.Json;
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

        private static readonly ISerializer<ItemDataWrapper, string> ActiveItemDataSerializer = new ItemDataSerializer(new JsonPropertySerializer());
        private ItemData srcItemData = null;

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
            if(WindowInstance.srcItemData == null)
            {
                WindowInstance.srcItemData = target;
            }
        }

        private void OnDisable()
        {
            WindowInstance = null;
        }

        private void OnGUI()
        {
            string currentGuid = srcItemData == null ? null : srcItemData.GUID;
            GuiHelper.DrawObjectField("Target " + nameof(ItemData), ref srcItemData);
            GuiHelper.DrawHorizontalLine(height: 1, spaceAbove: 3.5f);
            if ((currentGuid == null && srcItemData != null) || (srcItemData != null && currentGuid != srcItemData.GUID)) 
                SerializeAndUpdateText();
            else if(srcItemData != null) 
                UpdateTextEditor($"Item Editor<{srcItemData.GUID}>", ref OutputText);
            else DrawWarning("Must asign a valid ItemData scriptable object.");
        }

        private T ReloadAsset<T>(T asset) where T : ScriptableObject
        {
            string assetPath = AssetDatabase.GetAssetPath(asset);
            if (string.IsNullOrEmpty(assetPath)) 
            {
                Debug.LogError($"Could nto find asset path of <{asset.ToString()}>");
                return null;
            }
            T loaded = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (loaded == null)
                Debug.LogError($"Could not load asset from path. (path: {assetPath})");
            return loaded;
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
                ItemDataWrapper deserialized = DeserializeToItemDataWrapper(OutputText, ActiveItemDataSerializer);
                if (!deserialized.ApplyChangesToSource(srcItemData)) 
                    Debug.LogError("Could not apply changes to source ItemDaata.");

                EditorUtility.SetDirty(srcItemData);
                AssetDatabase.SaveAssets();
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(srcItemData),ImportAssetOptions.ForceUpdate);
                AssetDatabase.Refresh();
            }
            catch (System.Exception e) 
            {
                Debug.LogException(e);
                return;
            }
        }

        private ItemDataWrapper DeserializeToItemDataWrapper(string source, ISerializer<ItemDataWrapper, string> serializer)
        {
            var deserialized = serializer.Deserialize(source);
            if (deserialized.Status == OutputStatus.Successful)
            {
                PrintConfirmation(false, deserialized.Object);
                return deserialized.Object;
            }
            throw new System.InvalidOperationException($"Cannot deserialize into {nameof(ItemDataWrapper)}. (status: {deserialized.Status})");
        }

        private void SerializeAndUpdateText()
        {
            string serialized = SerializeItemDataWrapper(srcItemData, ActiveItemDataSerializer);
            if(OutputText != serialized)
                SetOutputText(serialized);
        }

        public void SetOutputText(string text) 
        {
            GUI.FocusControl(null);
            OutputText = text;
            Repaint();
        }

        private void ReloadItemDataSource()
        {
            AssetDatabase.SaveAssets();
            ItemData itemDataOnDisk = ReloadAsset(srcItemData);
            srcItemData = itemDataOnDisk;
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

        private void DrawWarning(string message) => GuiHelper.DrawText(message, UnityColorDatabase.PINK);


        #region Debug ##########################
        private void PrintConfirmation(bool serialized, ItemDataWrapper deserializedWrapper)
        {
            string guid = deserializedWrapper.guid;
            if (serialized)
                Debug.Log($"<color=cyan>SERIALIZED: </color> <{guid}>");
            else if (!serialized)
                Debug.Log($"<color=yellow>DESERIALIZED: </color> <{guid}>");
        }


        private static void PrintProperties(ItemData data, string title = "properties")
        {
            StringBuilder sb = new StringBuilder();
            foreach (var kv in data.GetUnifiedPropertyDataLookup(true))
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