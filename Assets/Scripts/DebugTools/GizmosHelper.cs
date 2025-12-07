using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Arctic.DebugTools
{
    public class GizmosHelper : MonoBehaviour
    {
#if UNITY_EDITOR
        private static GizmosHelper _Instance;
        public static GizmosHelper Instance
        {
            get
            {
                if (_Instance == null)
                {
                    GameObject g = new GameObject("~GizmosRenderer");
                    _Instance = g.AddComponent<GizmosHelper>();
                }
                return _Instance;
            }
        }

        private static readonly Dictionary<string, object> _KeyValues = new Dictionary<string, object>();
        private static readonly Dictionary<string, Action> _DrawCalls = new Dictionary<string, Action>();

        /// <returns>A style with the given font style and text color</returns>
        public static GUIStyle GetStyle(FontStyle fontStyle, Color textColor, int fontSize) 
        {
            GUIStyle style = new GUIStyle();
            style.fontStyle = fontStyle;
            style.normal.textColor = textColor;
            style.fontSize = fontSize;
            return style;
        }

        /// <returns>A style with the given font style</returns>
        public static GUIStyle GetStyle(FontStyle fontStyle)
            => GetStyle(fontStyle, Handles.color, 14);

        /// <returns>A style with the given font color.</returns>
        public static GUIStyle GetStyle(Color fontColor)
            => GetStyle(FontStyle.Normal, fontColor, 14);

        /// <summary>
        /// Creates or updated the value with the given key.
        /// </summary>
        /// <remarks>
        /// The value can later be retrived by the key from elsewhere. 
        /// <br>This allows data to be shared between multiple objects using GizmosHelper.</br>
        /// </remarks>
        public static void SetKeyValue(string key, object value) 
        {
            if (_KeyValues.ContainsKey(key))
                _KeyValues[key] = value;
            else
                _KeyValues.Add(key, value);
        }

        /// <returns>True, if a value was found with the key.</returns>
        public static bool TryGetValue<T>(string key, out T value) 
        {
            value = default;
            if (_KeyValues.TryGetValue(key, out var obj)) 
            {
                value = (T)obj;
                return true;
            }
            return false;
        }

        /// <returns>True, if a value was found with the key.</returns>
        public static bool TryGetValueRaw(string key, out object value)
        {
            return _KeyValues.TryGetValue(key, out value);
        }

        /// <summary>
        /// Prints the key value pairs to the console.
        /// </summary>
        public static void PrintKeyValues() 
        {
            StringBuilder buffer = new StringBuilder();
            buffer.AppendLine("<color=yellow>GIZMOS HELPER KEY-VALUES</color>");
            foreach (var kvp in _KeyValues)
                buffer.AppendLine($"<color=orange>{kvp.Key}</color> | <color=cyan>{kvp.Value}</color>");
            Debug.Log(buffer.ToString());
        }

        /// <summary>
        /// <br>Registers the draw call to be invoked within OnDrawGizmos.
        /// <br/>If one already exists with the same id, it will be overwritten.</br>
        /// </summary>
        public void QueueDrawCall(string id, Action drawCall)
        {
            if (!_DrawCalls.ContainsKey(id))
                _DrawCalls.Add(id, drawCall);
            else
                _DrawCalls[id] = drawCall;
        }

        /// <summary>
        /// Registers the draw call with a randomly generated ID.
        /// </summary>
        /// <returns>
        /// The generated ID for the call registration.
        /// </returns>
        public string QueueDrawCall(Action drawCall)
        {
            string randomId = System.IO.Path.GetRandomFileName() + System.DateTime.Now.Millisecond;
            QueueDrawCall(randomId, drawCall);
            return randomId;
        }

        /// <returns>Random color generated with the given seed.</returns>
        public static Color RandomColor(string seed) 
        {
            System.Random prng = new System.Random(seed.GetHashCode());
            float r = (float)prng.NextDouble(), 
                  g = (float)prng.NextDouble(),
                  b = (float)prng.NextDouble();
            return new Color(r, g, b, 1);
        }

        /// <summary>
        /// Unregisters the draw action with the givens id.
        /// </summary>
        public void RemoveDrawCall(string id)
        {
            _DrawCalls.Remove(id);
        }

        /// <summary>
        /// Clears all gizmos draw actions.
        /// </summary>
        public void ClearDrawCalls()
        {
            _DrawCalls.Clear();
        }

        private void OnDrawGizmos()
        {
            foreach (var kvp in _DrawCalls)
                kvp.Value?.Invoke();
        }

        private void OnDestroy()
        {
            ClearDrawCalls();
        }
#endif
    }
}