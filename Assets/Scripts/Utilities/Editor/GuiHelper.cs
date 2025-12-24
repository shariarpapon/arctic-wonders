using UnityEditor;
using UnityEngine;

namespace Arctic.Utilities.Editor
{
    public static class GuiHelper
    {
        public struct GuiAccumulatedSpacer
        {
            public float AccumulatedHeight { get; private set; }

            public void Add(float spaceHeight)
            {
                GUILayout.Space(spaceHeight);
                AccumulatedHeight += spaceHeight;
            }

            public void Reset()
            {
                AccumulatedHeight = 0.0f;
            }
        }

        /// <summary>
        /// For darker sections, this color matches unities default inspector very well.
        /// </summary>
        public static readonly Color INSPECTOR_DARK = new Color(0.10196f, 0.10196f, 0.10196f, 1);
        public static readonly Color INSPECTOR_TEXT_BG = new Color(0.12f, 0.12f, 0.12f);
        public const float DEFAULT_SECTION_SPACING = 2f;
        public const float DEFAULT_SECTION_PADDING = 4f;
        public const int DEFAULT_HEADER_FONTSIZE = 12;
        public const float U_DEFAULT_FIELD_HEIGHT = 18;
        public const float U_DEFAULT_BUTTON_HEIGHT = 20;

        public static InfoBoxGui DrawInfoBox(string text, float heightPadding) 
        {
            InfoBoxGui infoBox = new InfoBoxGui(text);
            infoBox.Draw(heightPadding);
            return infoBox;
        }

        public static InfoBoxGui DrawInfoBox(string text, Color bgColor, Color fontColor) 
        {
            InfoBoxGui infoBox = new InfoBoxGui(text, bgColor, fontColor);
            infoBox.Draw(10);
            return infoBox;
        }

        public static void SetContentColor(Color color) => GUI.contentColor = color;
        public static void SetBgColor(Color color) => GUI.backgroundColor = color;

        public static GUIStyle GetLabelStyleWithSize(int fontSize) 
        {
            var style = new GUIStyle(EditorStyles.label);
            style.fontSize = fontSize;
            style.wordWrap = true;
            style.alignment = TextAnchor.UpperLeft;
            return style;
        }

        public static void DrawButton(string label, System.Action action, float height = U_DEFAULT_BUTTON_HEIGHT)
        {
            if (GUILayout.Button(label, GUILayout.Height(height)))
                action?.Invoke();
        }

        public static void DrawButton(string label, Color contentColor, Color bgColor, System.Action action, float height = U_DEFAULT_BUTTON_HEIGHT) 
        {
            ColorSwitch(contentColor, bgColor, 
                () => 
                { 
                    DrawButton(label, action, height); 
                });
        }

        /// <returns>Accumulated vertical height of the drawn elements.</returns>
        public static float DrawObjectField<TObject>(string label, ref TObject obj, bool allowSceneObjects = false) where TObject : Object
        {
            obj = (TObject)EditorGUILayout.ObjectField(label, obj, typeof(TObject), allowSceneObjects);
            return U_DEFAULT_FIELD_HEIGHT;
        }

        /// <returns>Accumulated vertical height of the drawn elements.</returns>
        public static float DrawText(string text, Color color)
        {
            ContentColorSwitch(color, () => EditorGUILayout.SelectableLabel(text) );
            return U_DEFAULT_FIELD_HEIGHT;
        }

        /// <returns>Accumulated vertical height of the drawn elements.</returns>
        public static float HorizontalLine(int height = 1, float spaceAbove = DEFAULT_SECTION_SPACING, float spaceBelow = DEFAULT_SECTION_SPACING)
        {
            return DrawHorizontalLine(INSPECTOR_DARK, height, spaceAbove, spaceBelow);
        }

        /// <returns>Accumulated vertical height of the drawn elements.</returns>
        public static float DrawHorizontalLine(Color color, int height = 1, float spaceAbove = DEFAULT_SECTION_SPACING, float spaceBelow = DEFAULT_SECTION_SPACING)
        {
            GuiAccumulatedSpacer spacer = new GuiAccumulatedSpacer();

            spacer.Add(spaceAbove);
            Rect rect = GUILayoutUtility.GetRect(10, height, GUILayout.ExpandWidth(true));
            rect.height = height;
            rect.xMin = 0;
            rect.xMax = EditorGUIUtility.currentViewWidth;

            Color lineColor = color;
            EditorGUI.DrawRect(rect, lineColor);
            spacer.Add(spaceBelow);

            return spacer.AccumulatedHeight + height;
        }

        /// <returns>Accumulated vertical height of the drawn elements.</returns>
        public static float DrawHeaderLabel(string label, int fontSize = DEFAULT_HEADER_FONTSIZE, float spaceAbove = DEFAULT_SECTION_SPACING, float spaceBelow = DEFAULT_SECTION_SPACING) 
        {
            var style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = fontSize };
            EditorGUILayout.LabelField(label, style, GUILayout.ExpandWidth(true));
            return HorizontalLine(spaceAbove: spaceAbove, spaceBelow: spaceBelow) + DEFAULT_SECTION_SPACING;
        }

        /// <returns>Accumulated vertical height of the drawn elements.</returns>
        public static float DrawHeaderLabel(string label, Color color, int fontSize = DEFAULT_HEADER_FONTSIZE)
        {
            float height = 0;
            ContentColorSwitch(color, () => 
            {
                height = DrawHeaderLabel(label, fontSize);
            });
            return height;
        }

        /// <returns>Accumulated vertical height of the drawn elements.</returns>
        public static float DrawTextEditorWindowArea(ref string text, int fontSize = 14, float paddingX = DEFAULT_SECTION_PADDING, float paddingY = DEFAULT_SECTION_PADDING)
        {
            return DrawTextEditorWindowArea(ref text, INSPECTOR_TEXT_BG, fontSize, paddingX, paddingY);
        }

        /// <returns>Accumulated vertical height of the drawn elements.</returns>
        public static float DrawTextEditorWindowArea(ref string text, Color bgColor, int fontSize = 14, float paddingX = DEFAULT_SECTION_PADDING, float paddingY = DEFAULT_SECTION_PADDING)
        {
            GUIStyle style = GetLabelStyleWithSize(fontSize);

            Rect fullArea = GUILayoutUtility.GetRect(
                GUIContent.none,
                style,
                GUILayout.ExpandWidth(true),
                GUILayout.ExpandHeight(true)
            );

            Rect paddedBgArea = new Rect(
                fullArea.x + paddingX,
                fullArea.y + paddingY,
                fullArea.width - paddingX * 2f,
                fullArea.height - paddingY * 2f
            );

            const float inputAreaPadding = 3f;
            Rect paddedInputArea = new Rect(
                paddedBgArea.x + inputAreaPadding,
                paddedBgArea.y + inputAreaPadding,
                paddedBgArea.width - inputAreaPadding * 2,
                paddedBgArea.height - inputAreaPadding * 2
            );
            

            EditorGUI.DrawRect(paddedBgArea, bgColor);
            text = EditorGUI.TextArea(paddedInputArea, text, style);

            return paddedInputArea.height;
        }

        /// <summary>Switches to given content color -> invokes the draw action -> switches back to original color.</summary>
        public static void ContentColorSwitch(Color contentColor, System.Action draw) 
        {
            Color lastColor = GUI.contentColor;
            GUI.contentColor = contentColor;
            draw?.Invoke();
            GUI.contentColor = lastColor;
        }

        /// <summary>Switches to given content color -> invokes the draw action -> switches back to original color.</summary>
        public static void BgColorSwitch(Color bgColor, System.Action draw)
        {
            Color lastColor = GUI.backgroundColor;
            GUI.backgroundColor = bgColor;
            draw?.Invoke();
            GUI.backgroundColor = lastColor;
        }

        public static void ColorSwitch(Color contColor, Color bgColor, System.Action draw)
        {
            Color lastCont = GUI.contentColor;
            Color lastBg = GUI.backgroundColor;
            GUI.contentColor = contColor;
            GUI.backgroundColor = bgColor;
            draw?.Invoke();
            GUI.contentColor = lastCont;
            GUI.backgroundColor = lastBg;
        }

    }

}