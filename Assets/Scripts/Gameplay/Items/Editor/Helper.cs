using UnityEngine;
using UnityEditor;

namespace Arctic.Gameplay.Items.Editor
{
    public static class Helper
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
                AccumulatedHeight = 0f;
            }
        }

        /// <summary>
        /// For darker sections, this color matches unities default inspector very well.
        /// </summary>
        public static readonly Color INSPECTOR_DARK = new Color(0.10196f, 0.10196f, 0.10196f, 1);
        public const float DEFAULT_SECTION_SPACING = 2f;
        public const int DEFAULT_HEADER_FONTSIZE = 12;

        public const float U_DEFAULT_FIELD_HEIGHT = 18;

        public static void SetContentColor(Color color)
        {
            GUI.contentColor = color;
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
        public static float DrawHorizontalLine(int height = 1, float spaceAbove = DEFAULT_SECTION_SPACING, float spaceBelow = DEFAULT_SECTION_SPACING)
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
            return DrawHorizontalLine(spaceAbove: spaceAbove, spaceBelow: spaceBelow) + DEFAULT_SECTION_SPACING;
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

        public static float DrawTextEditorWindowArea(ref string text, float paddingX = 4f, float paddingY = 4f)
        {
            Rect fullArea = GUILayoutUtility.GetRect(
                GUIContent.none,
                GUIStyle.none,
                GUILayout.ExpandWidth(true),
                GUILayout.ExpandHeight(true)
            );

            Rect padded = new Rect(
                fullArea.x + paddingX,
                fullArea.y + paddingY,
                fullArea.width - paddingX * 2f,
                fullArea.height - paddingY * 2f
            );
            text = EditorGUI.TextArea(padded, text);
            return padded.height;
        }


        /// <summary>Switches to given content color -> invokes the draw action -> switches back to original color.</summary>
        public static void ContentColorSwitch(Color contentColor, System.Action draw) 
        {
            Color lastColor = GUI.contentColor;
            GUI.contentColor = contentColor;
            draw?.Invoke();
            GUI.contentColor = lastColor;
        }
    }
}