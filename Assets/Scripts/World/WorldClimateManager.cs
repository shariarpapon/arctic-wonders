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
        private float globalBaseTempF = 5f;
        [Header("Seasonal Temp Modifiers (F)")]
        [SerializeField]
        private float summerModF = 20f;
        [SerializeField]
        private float winterModF = -20f;
        [SerializeField]
        private float autumnModF = 0f;
        [SerializeField]
        private float springModF = 0f;

        public float GlobalTempF => globalBaseTempF + GetSeasonTempModifier(_currentSeason);
    
    
        private float GetSeasonTempModifier(Season season)
        {
            return season switch
            {
                Season.Summer => summerModF,
                Season.Winter => winterModF,
                Season.Autumn => autumnModF,
                Season.Spring => springModF,
                _ => 0f
            };
        }
    }
}