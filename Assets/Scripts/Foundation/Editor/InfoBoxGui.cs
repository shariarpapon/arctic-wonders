using UnityEditor;
using UnityEngine;

namespace Arctic.Foundation.Editor
{
    public struct InfoBoxGui
    {
        private const int DEFAULT_FONT_SIZE = 11;
        public static readonly Vector2 DEFAULT_TEXT_BORDER = new Vector2(8, 8);
        public static readonly Color DEFAULT_BG_COLOR = new Color(0.32f, 0.30f, 0.26f, 1f);
        public static readonly Color DEFAULT_FONT_COLOR = Color.white;

        public string Text { get; private set; }
        
        public Color BgColor { get; private set; }
        public Color FontColor { get; private set; }

        public Vector2 TextBorder { get; private set; }
        public GUIStyle GuiStyle { get; private set; }
        public bool ExpandWidth { get; private set; }
        public InfoBoxGui(string text = "")
            : this(text, DEFAULT_BG_COLOR, DEFAULT_FONT_COLOR) { }

        public InfoBoxGui(string text, Color bgColor, Color fontColor)
        {
            Text = text;
            BgColor = DEFAULT_BG_COLOR;
            FontColor = DEFAULT_FONT_COLOR;
            TextBorder = DEFAULT_TEXT_BORDER; 
            ExpandWidth = true;
            GuiStyle = GuiHelper.GetLabelStyleWithSize(DEFAULT_FONT_SIZE);
            Draw();
        }

        public void Draw()
        {
            Color bgColor = BgColor;
            string text = Text;
            GUIStyle style = GuiStyle;
            Rect bgRect = GetBgRect();
            Rect textRect = GetTextRect(bgRect);
            GuiHelper.ColorSwitch(FontColor, GuiHelper.INSPECTOR_TEXT_BG, () =>
            {
                EditorGUI.DrawRect(bgRect, bgColor);
                EditorGUI.LabelField(textRect, text, style);
            });
        }

        private Rect GetBgRect() 
        {
            EditorGUILayout.Space(5);
            Rect lastRect = GUILayoutUtility.GetLastRect();
            Rect bgRect = GUILayoutUtility.GetRect(new GUIContent(Text), GuiStyle, GUILayout.ExpandWidth(true));
            bgRect.height += TextBorder.y;
            bgRect.position = lastRect.position + new Vector2(0, lastRect.size.y);
            EditorGUILayout.Space(10);

            return bgRect;
        }

        private Rect GetTextRect(Rect bgRect) 
        {
            Rect textRect = new Rect(bgRect);
            textRect.width -= TextBorder.x;
            textRect.height -= TextBorder.y;
            textRect.x += TextBorder.x / 2;
            textRect.y += TextBorder.y / 2;
            return textRect;
        }

        public void SetTextBorder(Vector2 border) 
        {
            TextBorder = border;
        }

        public InfoBoxGui SetExpandWidth(bool expandWidth)
        {
            ExpandWidth = expandWidth;
            return this;
        }

        public InfoBoxGui SetText(string text)
        {
            Text = text;
            return this;
        }
        public InfoBoxGui SetBgColor(Color color)
        {
            BgColor = color;
            return this;
        }
        public InfoBoxGui SetFontColor(Color color)
        {
            FontColor = color;
            return this;
        }

        public InfoBoxGui SetFontSize(int fontSize)
        {
            GuiStyle.fontSize = fontSize;
            return this;
        }

        public InfoBoxGui SetFontAlignment(TextAnchor alignment)
        {
            GuiStyle.alignment = alignment;
            return this;
        }

        public InfoBoxGui SetGUIStyle(GUIStyle guiStyle)
        {
            GuiStyle = guiStyle;
            return this;
        }
    }
}