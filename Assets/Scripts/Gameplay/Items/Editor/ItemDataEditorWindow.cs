using Arctic.Utilities;
using Arctic.Utilities.Editor;
using Arctic.Utilities.Editor.WindowTabs;
using Codice.CM.Common;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Arctic.Gameplay.Items.Editor
{
    public sealed class ItemDataEditorWindow : EditorWindow
    {
        private const int DEFAULT_BUTTON_HEIGHT = 25;

        public ItemDataEditorController controller;
        private WindowTabOperator tabOperator;

        private string editorText = string.Empty;
        private string sourceGuidInput;
        private int fontSize = 14;

        private string loadPath;

        private void OnEnable()
        {
            loadPath = string.Empty;

            InitializeController();
            IntializeTabs();
        }

        private void InitializeController() 
        {
            controller = new ItemDataEditorController(
               new ItemDataSerializer(),
               OnSerialized,
               OnDeserialized
           );
        }

        private void IntializeTabs() 
        {
            Tab[] tabs = new Tab[]
            {
                new Tab("Text Editor", DrawTextEditorTab),
                new Tab("Load Text File", DrawLoadTextFileTab),
            };

            tabOperator = WindowTabOperator
                .Builder
                .Init()
                .RegisterTabs(tabs)
                .SetDefaultSelection(0)
                .Build();
        }

        private void OnGUI()
        {
            tabOperator.Operate();
        }

        private void DrawTextEditorTab() 
        {
            DrawSourceField();
            DrawTextEditorInputSection();
            if(controller.HasValidSource)
                DrawSerializeButtons();
        }

        private void DrawLoadTextFileTab() 
        {
            GuiHelper.HorizontalLine();
            loadPath = EditorGUILayout.TextField("Load From: ", loadPath);
            GuiHelper.HorizontalLine();
            bool selected = false;
            if(GUILayout.Button("Browse"))
                selected = Helper.BrowseFilesystem(ref loadPath, Helper.BrowseFilter.All);
            if (selected) 
            {
                controller.SetSource(null);
                tabOperator.TrySetSelection("Text Editor");
                try 
                {
                    string loaded = File.ReadAllText(loadPath);
                    SetEdtiorText(loaded);
                }
                catch(System.Exception e) 
                {
                    Debug.LogError("Could not load file content: " + e.Message);
                    return; 
                }
            }
            Repaint();
        }

        private void DrawSerializeButtons() 
        {
            GUILayout.BeginHorizontal();
            GuiHelper.DrawButton("Serialize", controller.Serialize, height: DEFAULT_BUTTON_HEIGHT);
            GuiHelper.DrawButton("Deserialize", () => controller.Deserialize(editorText), height: DEFAULT_BUTTON_HEIGHT);
            GUILayout.EndHorizontal();
            GuiHelper.HorizontalLine();
        }

        private void DrawTextEditorInputSection()
        {
            fontSize = EditorGUILayout.IntSlider("Font Size", fontSize, 1, 100);
            GuiHelper.HorizontalLine();
            GuiHelper.DrawTextEditorWindowArea(ref editorText, fontSize : fontSize, paddingX : 3f, paddingY : 3f);
            GuiHelper.HorizontalLine();
        }

        private void DrawSourceField()
        {
            GuiHelper.HorizontalLine(spaceAbove: 2f);
            var source = controller.Source;
            GuiHelper.DrawObjectField("Target ItemData", ref source);
            controller.SetSource(source);
            GuiHelper.HorizontalLine();
        }

        //GUID prompt not needed atm but its functional
        //private void DrawInvalidSourceGuid()
        //{
        //    DrawWarning(
        //        "Source ItemData does not have a valid GUID.\nGenerate random GUID?"
        //    );

        //    if (GUILayout.Button("Generate Random GUID", GUILayout.Height(DEFAULT_BUTTON_HEIGHT)))
        //    {
        //        controller.Source.SetRandomGUID();
        //        Repaint();
        //        return;
        //    }

        //    GUILayout.Space(5);

        //    DrawWarning("Alternatively, set one manually:");

        //    EditorGUILayout.BeginHorizontal();
        //    sourceGuidInput = EditorGUILayout.TextField(sourceGuidInput);

        //    if (GUILayout.Button("Set") && !string.IsNullOrEmpty(sourceGuidInput))
        //    {
        //        controller.Source.SetGUID(sourceGuidInput);
        //        sourceGuidInput = null;
        //        Repaint();
        //    }

        //    EditorGUILayout.EndHorizontal();
        //}

        private void SetEdtiorText(string text) 
        {
            GUI.FocusControl(null);
            editorText = text;
            Repaint();
        }

        private void OnSerialized(string text)
        {
            SetEdtiorText(text);
        }

        private void OnDeserialized(ItemData item)
        {
            Repaint();
        }

        private void DrawWarning(string message)
        {
            GuiHelper.DrawText(message, UnityColorDatabase.gentle_yellow);
        }
    }
}

