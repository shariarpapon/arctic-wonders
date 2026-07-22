using UnityEngine;

namespace Arctic.Gameplay.Survival.Actors
{
    public class VitalSystemView
    {
        private VitalSystem _vitalSystem;
        public void Init(VitalSystem vitalSystem) 
        { 
            _vitalSystem = vitalSystem;
        }
    }
}