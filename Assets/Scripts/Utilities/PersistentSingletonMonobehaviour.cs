using UnityEngine;

namespace Arctic.Utilities
{
    public class PersistentSingletonMonobehaviour<T> : MonoBehaviour where T : Component
    {
        public static T Instance { get; private set; }

        protected virtual void OnPreAwake() { }

        protected virtual void Awake()
        {
            OnPreAwake();
            if (Instance == null)
            {
                Instance = this as T;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            OnPostAwake();
        }

        /// <summary>
        /// Called once the singleton instance is successfully awakened.
        /// <br>It's only called once in the lifetime of the singleton.</br>
        /// </summary>
        protected virtual void OnPostAwake() { }

    }
}