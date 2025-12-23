using Arctic.Utilities;
using Arctic.Utilities.Editor;
using Arctic.Utilities.Editor.Tabs;
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
        private int fontSize = 14;


        private bool autoCreateIfNotFound = true;
        private string loadPath = null;
        private string autoCreatePath = null;

        private static readonly Color SelectedTabButtonColor = new Color(0.32f, 0.32f, 0.55f, .5f);
        private Color defBackgroundColor;

        private void OnEnable()
        {
            defBackgroundColor = GUI.backgroundColor;
            InitializeController();
            IntializeTabs();
        }

        private void OnDisable()
        {
            DisposeTabOperator();    
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
                new Tab("Options", DrawOptionsTab),
            };

            tabOperator = WindowTabOperator
                .Builder
                .Init()
                .RegisterTabs(tabs)
                .SetDefaultSelection(0)
                .Build();


            Color defaultColor = GUI.backgroundColor;
            tabOperator.OnBeforeSelectedTabButtonRendered += SetSelectedTabButtonColor;
            tabOperator.OnAfterSelectedTabButtonRendered += SetDefaultTabButtonColor;
        }

        private void DisposeTabOperator() 
        {
            tabOperator.OnBeforeSelectedTabButtonRendered -= SetSelectedTabButtonColor;
            tabOperator.OnAfterSelectedTabButtonRendered -= SetDefaultTabButtonColor;
        }

        private void OnGUI()
        {
            tabOperator.Operate();
        }

        private void SetSelectedTabButtonColor(Tab tab) 
        {
            GUI.backgroundColor = SelectedTabButtonColor; 
        }

        private void SetDefaultTabButtonColor(Tab tab) 
        {
            GUI.backgroundColor = defBackgroundColor;
        }

        private void DrawTextEditorTab() 
        {
            GuiHelper.HorizontalLine(spaceAbove: 2f);
            DrawSourceField();
            DrawTextEditorInputSection();
            DrawSerializeButtons();
        }
        private void DrawOptionsTab() 
        {
            GuiHelper.HorizontalLine(spaceAbove: 2f);
            DrawAutoCreationPathSection();
            GuiHelper.HorizontalLine();
            DrawLoadTextFileSection();
        }
        private void DrawAutoCreationPathSection() 
        {
            autoCreateIfNotFound = EditorGUILayout.Toggle("Auto Create If Not Found", autoCreateIfNotFound, GUILayout.ExpandWidth(false));
            if (autoCreateIfNotFound)
            {
                autoCreatePath = EditorGUILayout.TextField("Auto Create Path", autoCreatePath);
                controller.SetAutoCreationPath(autoCreatePath);
            }
        }

        private void DrawLoadTextFileSection()
        {
            bool fileSelected = false;
            if (GUILayout.Button("Load Text File"))
                fileSelected = Helper.BrowseFilesystem(ref loadPath, Helper.BrowseFilter.All);
            
            if(fileSelected)
                LoadFileContentIntoEditor(loadPath);
        }

        private void LoadFileContentIntoEditor(string path) 
        {
            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("Invalid load path.");
                return;
            }

            try
            {
                string loaded = File.ReadAllText(loadPath);
                controller.SetSource(null);
                tabOperator.TrySetSelection("Text Editor");
                SetEdtiorText(loaded);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Could not load file content: " + e.Message);
                return;
            }
            Repaint();   
        }

        private void DrawSerializeButtons() 
        {
            if(controller.HasValidSource)
                GuiHelper.DrawButton("Serialize", controller.Serialize, height: DEFAULT_BUTTON_HEIGHT);
            GuiHelper.DrawButton("Deserialize", () => controller.Deserialize(editorText, autoCreateIfNotFound), height: DEFAULT_BUTTON_HEIGHT);
            GuiHelper.HorizontalLine();
        }

        private void DrawTextEditorInputSection()
        {
            fontSize = EditorGUILayout.IntSlider("Font Size", fontSize, 1, 100);
            GuiHelper.HorizontalLine();
            GuiHelper.DrawTextEditorWindowArea(ref editorText, fontSize : fontSize, paddingX : 3f, paddingY : 3f);
        }

        private void DrawSourceField()
        {
            var source = controller.Source;
            GuiHelper.DrawObjectField("Target ItemData", ref source);
            controller.SetSource(source);
            GuiHelper.HorizontalLine();
        }

        //GUID prompt not needed atm but its functional
        //privat estring sourceGuidInput = null;
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
    }
}

