using UnityEngine;

namespace Arctic.Utilities
{
    public static class CursorManager
    {
        private static bool _IsCursorModificationRestricted = false;

        public static void Initialize() 
        {
            Release();
            SetCursor(CursorLockMode.Locked, true, false);
        }

        /// <summary>
        /// Sets the cursor lock mode.
        /// </summary>
        /// <param name="mode">Cursor lock mode to change to.</param>
        /// <param name="force">If true, it will force release any restrictions and change to the given mode.</param>
        /// <param name="restrict">
        /// If true and cursor was successfully modified, 
        /// <br>it will retrict further modifications until restriction is manually released or force changed.</br>
        /// </param>
        public static void SetCursor(CursorLockMode mode, bool force, bool restrict) 
        {
            if (force)
                Release();

            if (!_IsCursorModificationRestricted)
            {
                Cursor.lockState = mode;
                if (restrict)
                    Restrict();
            }
        }

        /// <summary>
        /// Restricts cursor modification.
        /// </summary>
        public static void Restrict() 
        {
            _IsCursorModificationRestricted = true;
        }

        /// <summary>
        /// Releases cursor modification restrictions.
        /// </summary>
        public static void Release() 
        {
            _IsCursorModificationRestricted = false;
        }
    }
}
