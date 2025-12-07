using UnityEngine;

namespace Arctic.InputManagement
{
    /// <summary>
    /// Contains runtime input capture utility and the active input provider that is feeding input to the whole game.
    /// </summary>
    public static class InputManager
    {
        static InputManager() 
        {
            Initialize();
        }

        /// <summary>
        /// If true, all non-UI user input will be disabled.
        /// </summary>
        public static bool IsInputDisabled { get; private set; } = false;
        private static InputProvider _DefaultInputProvider;

        /// <summary>
        /// This is the active input source. 
        /// <br>This will be null if input is disabled.</br>
        /// </summary>
        public static InputProvider Provider
        { 
            get 
            {
                if (IsInputDisabled)
                    return _DisabledInputProvider;
                return _InputProvider;
            }
            private set 
            {
                _InputProvider = value;
            }
        }
        private static InputProvider _InputProvider;
        private static InputProvider _DisabledInputProvider;

        /// <summary>
        /// Initializes the input manager.
        /// </summary>
        public static void Initialize() 
        {
            _DefaultInputProvider = new InputProvider();
            _DisabledInputProvider = new DisabledInputProvider();
            SetInputProvider(_DefaultInputProvider);
            Enable();
        }

        /// <summary>
        /// Sets the given input provider as the active one.
        /// </summary>
        public static void SetInputProvider(InputProvider provider)
        {
            if (provider != null)  
                Provider = provider; 
            else 
                Debug.LogWarning("Cannot assign a null input provider."); 
        }

        /// <summary>
        /// Enables the input provider.
        /// </summary>
        public static void Enable() 
        {
            IsInputDisabled = false;
        }

        /// <summary>
        /// Disables the input provider.
        /// </summary>
        public static void Disable() 
        {
            IsInputDisabled = true;
        }
    }
}
