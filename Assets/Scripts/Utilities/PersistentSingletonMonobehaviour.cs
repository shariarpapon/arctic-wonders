using UnityEngine;

namespace Arctic.Utilities
{
    public class PersistentSingletonMonobehaviour<T> : MonoBehaviour where T : Component
    {
        public static T Instance { get; private set; }

        /// <summary>
        /// Called before the singleton evaluation. Will be called once on Awake regardless of singleton status.
        /// </summary>
        protected virtual void OnPreSingletonEvaluation() { }

        protected virtual void Awake()
        {
            OnPreSingletonEvaluation();
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
            OnSingletonEvaluated();
        }

        /// <summary>
        /// Called once the singleton instance finishes succesfully evaluates .
        /// <br>It's only called once in the lifetime of the singleton.</br>
        /// </summary>
        protected virtual void OnSingletonEvaluated() { }

    }
}