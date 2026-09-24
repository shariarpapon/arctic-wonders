using Arctic.Foundation.Actor;
using UnityEngine;

namespace Arctic.Gameplay.Survival.Actors
{
    public class VitalSystemActor : MonoActor<VitalSystem, VitalSystemView>
    {
        [SerializeField] protected bool _initOnStart = true;
        [SerializeField] protected VitalSystem _vitalSystem;

        public VitalSystem GetVitalSystem => _vitalSystem;

        protected virtual void Start()
        {
            if (_initOnStart)
                _vitalSystem.Init();
        }

        public override VitalSystem GetViewContext()
        {
            return _vitalSystem;
        }

        protected override void Simulate(float deltaTime)
        {
            _vitalSystem.UpdateVitals(deltaTime);
        }

       
    }
}