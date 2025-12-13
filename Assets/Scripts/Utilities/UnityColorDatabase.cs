using UnityEngine;

namespace Arctic.Utilities
{
    public static class UnityColorDatabase
    {
        public static readonly Color WHITE = new Color(1f, 1f, 1f, 1f);
        public static readonly Color BLACK = new Color(0f, 0f, 0f, 1f);
        public static readonly Color RED = new Color(1f, 0f, 0f, 1f);
        public static readonly Color GREEN = new Color(0f, 1f, 0f, 1f);
        public static readonly Color BLUE = new Color(0f, 0f, 1f, 1f);
        public static readonly Color YELLOW = new Color(1f, 1f, 0f, 1f);
        public static readonly Color CYAN = new Color(0f, 1f, 1f, 1f);
        public static readonly Color MAGENTA = new Color(1f, 0f, 1f, 1f);
        public static readonly Color GRAY = new Color(0.5f, 0.5f, 0.5f, 1f);
        public static readonly Color CLEAR = new Color(0f, 0f, 0f, 0f);

        // Custom / UI-friendly
        public static readonly Color ORANGE = new Color(1f, 0.5f, 0f, 1f);
        public static readonly Color PURPLE = new Color(0.5f, 0f, 0.5f, 1f);
        public static readonly Color TEAL = new Color(0f, 0.5f, 0.5f, 1f);
        public static readonly Color PINK = new Color(1f, 0.4f, 0.7f, 1f);
    }

}