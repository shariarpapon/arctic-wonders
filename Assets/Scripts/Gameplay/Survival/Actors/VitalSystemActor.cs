using UnityEngine;

namespace Arctic.Gameplay.Survival.Actors
{
    public class VitalSystemActor : MonoBehaviour
    {
        [SerializeField] protected bool _initOnStart = true;
        [SerializeField] protected VitalSystem _vitalSystem;

        public VitalSystem GetVitalSystem => _vitalSystem;

        protected virtual void Start()
        {
            if(_initOnStart)
                _vitalSystem.Init();
        }

        protected virtual void Update()
        {
            _vitalSystem.UpdateVitals(Time.deltaTime);
        }
    }
}