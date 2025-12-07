using UnityEngine;
using Arctic.Utilities;

namespace Arctic.DebugTools
{
    /// <summary>
    /// Some quick functions to easily log different colors on the unity console.
    /// </summary>
    public static class Debugc
    {
        public const string CORAL_RED = "#FF6161";
        public const string AMBER = "#FFF07B";
        public const string OLIVE_GREEN = "#99BB11";
        public const string BRIGHT_GREEN = "#12FFBB";
        public const string BLUE = "#46D7FF";
        public const string NEON_MAGENTA = "#FD92FF";

        public static bool IsEnabled { get; private set; } = true;

        public static void SetEnabled(bool enable) => IsEnabled = enable;

        public static void Log(object msg, string colorHex)
            => Debug.Log($"<color={colorHex}>{msg}</color>");

        public static void Log(object msg, Color color) => Log(msg, color.ToHex());

        public static void LogError(object msg) => Log(msg, CORAL_RED);
        public static void LogWarning(object msg) => Log(msg, AMBER);
        public static void LogConfirm(object msg) => Log(msg, OLIVE_GREEN);
        public static void LogInfo(object msg) => Log(msg, BLUE);
        public static void LogEmphasis(object msg) => Log(msg, NEON_MAGENTA);
    }
}