using Arctic.Utilities;
using UnityEngine;

namespace Arctic.Core
{
    public class TimeManager : PersistentSingletonMonobehaviour<TimeManager>
    {
        [SerializeField]
        private float timeScale = 1.0f;
        [SerializeField]
        private float dayLengthSec = 420f;
        [SerializeField]
        private float currentDaySeconds = 0f;
        [SerializeField]
        private int currentDays = 0;
        [SerializeField]
        private float timeOfDayNormalized = 0.0f;

        /// <summary>
        /// The duration of an in-game day in seconds.
        /// </summary>
        public float DayLengthSeconds => dayLengthSec;

        /// <summary>
        /// The current number of completed days since the start of the run.
        /// </summary>
        public int CurrentDay => currentDays;

        /// <summary>
        /// Float in the range [0f, 420f]
        /// <br>There are 420 seconds in an in-game day.</br>
        /// </summary>
        public float CurrentDaySeconds => currentDaySeconds;

        /// <summary>
        /// A value in range [0, 1] representing the normalized time of day. 
        /// </summary>
        public float TimeOfDayNormalized => timeOfDayNormalized;

        ///<summary>
        /// The int parameter represents the current day count (starting from 0).
        /// <br>Subscribe to this event to trigger actions that should occur at the start of each day.</br>
        /// </summary>
        public event System.Action<int> OnNewDay;

        private void Update()
        {
            UpdateTime();    
        }

        private void UpdateTime() 
        {
            float scaledDeltaTime = Time.deltaTime * timeScale;
            currentDaySeconds += scaledDeltaTime;
            if (currentDaySeconds >= dayLengthSec)
                StartNewDay();
            timeOfDayNormalized = Mathf.Clamp01(currentDaySeconds / dayLengthSec);
        }

        private void StartNewDay() 
        {
            currentDaySeconds %= dayLengthSec;
            currentDays++;
            OnNewDay?.Invoke(currentDays);
        }
   
    }
}