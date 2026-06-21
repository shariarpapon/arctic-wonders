using Arctic.Utilities.Generics;
using UnityEngine;

namespace Arctic.World
{
    public class WorldClimateManager : PersistentSingletonMonobehaviour<WorldClimateManager>
    {
        [System.Serializable]
        public enum Season
        {
            Summer,
            Autumn,
            Winter,
            Spring
        }

        [SerializeField]
        private Season _currentSeason = Season.Summer;

        [Space]
        [SerializeField]
        private float baseAmbientTempF = 5f;
        [Header("Seasonal Temp Modifiers (F)")]
        [SerializeField]
        private float summerTempOffsetF = 20f;
        [SerializeField]
        private float winterTempOffsetF = -20f;
        [SerializeField]
        private float autumnTempOffsetF = 0f;
        [SerializeField]
        private float springTempOffsetF = 0f;

        public float CurrentAmbientTempF => baseAmbientTempF + GetSeasonalTempOffset(_currentSeason);
    
    
        private float GetSeasonalTempOffset(Season season)
        {
            return season switch
            {
                Season.Summer => summerTempOffsetF,
                Season.Winter => winterTempOffsetF,
                Season.Autumn => autumnTempOffsetF,
                Season.Spring => springTempOffsetF,
                _ => 0f
            };
        }
    }
}