using UnityEngine;
using UnityEngine.UI;
using System.IO;

namespace Arctic.Utilities
{

    public static class UnityExtensions
    {
        #region General
        /// <returns>The color RGB as a hexadecimal string.</returns>
        public static string ToHex(this Color color)
        {
            int r = Mathf.RoundToInt(color.r * 255f);
            int g = Mathf.RoundToInt(color.g * 255f);
            int b = Mathf.RoundToInt(color.b * 255f);
            return $"#{r:X2}{g:X2}{b:X2}";
        }

        /// <returns>The RGBA color converted from the hexcode.</returns>
        public static Color HexToColor(string hex)
        {
            if (hex.StartsWith("#"))
                hex = hex.Substring(1);

            byte r = 255, g = 255, b = 255, a = 255;

            if (hex.Length == 6)
            {
                r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            }
            else if (hex.Length == 8)
            {
                r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
            }
            else
            {
                Debug.LogWarning($"Invalid hex color string: {hex}");
            }

            return new Color(r / 255f, g / 255f, b / 255f, a / 255f);
        }

        /// <summary>
        /// Plays the particle system if the play condition is true and stops it otherwise.
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="playCondition"></param>
        public static void PlayToggle(this ParticleSystem instance, bool playCondition)
        {
            if (playCondition)
                instance.PlayIfStopped();
            else
                instance.Stop();
        }

        /// <summary>
        /// Plays the particle system only if it is currently stopped.
        /// </summary>
        public static void PlayIfStopped(this ParticleSystem instance)
        {
            if (instance.isStopped)
                instance.Play();
        }
        #endregion

        #region Generic
        /// <summary>
        /// Attempts to look for the object in the active scene. 
        /// <br>If not found, the object is loaded in from specified resource path.</br>
        /// </summary>
        public static T GetResourceInstance<T>(this object obj, string resourceDir, string resourceName) where T : Object
        {
            var instance = Object.FindObjectOfType<T>(true);
            if (instance == null)
            {
                GameObject prefab = Resources.Load<GameObject>(Path.Combine(resourceDir, resourceName));
                instance = Object.Instantiate(prefab).GetComponent<T>();
            }
            return instance;
        }
        #endregion

        #region UI
        /// <summary>
        /// Sets the alpha value to zero.
        /// </summary>
        public static void MakeTransparent(this Image image)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
        }

        /// <summary>
        /// Sets the alpha value to one.
        /// </summary>
        public static void MakeOpaque(this Image image)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
        }

        /// <summary>s
        /// Sets the slider value.
        /// </summary>
        public static void SetValue(this Slider slider, float value)
        {
            slider.value = value;
        }

        /// <summary>
        /// Sets the toggle on state.
        /// </summary>
        public static void SetValue(this Toggle toggle, bool on)
        {
            toggle.isOn = on;
        }

        /// <summary>
        /// Fixes a very annoying bug with unities nested LayoutGroup and ContetSizeFitter components.
        /// </summary>
        public static void FixAnnoyingUnityUIBug(this Transform root)
        {
            //Essentially deactivate -> reactivate -> set to original state, for all gameobjects under the root.
            Canvas.ForceUpdateCanvases();
            var children = root.GetComponentsInChildren<Transform>();
            foreach (Transform child in children)
            {
                bool originalState = child.gameObject.activeSelf;
                child.gameObject.SetActive(false);
                child.gameObject.SetActive(true);
                child.gameObject.SetActive(originalState);
            }
        }
        #endregion
    }
}