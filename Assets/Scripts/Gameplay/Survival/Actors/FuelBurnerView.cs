using Arctic.Foundation.Actor;
using UnityEngine;

namespace Arctic.Gameplay.Survival.Actors
{
    [System.Serializable]
    public class FuelBurnerView : ActorView
    {
        [Header("Light")]
        [SerializeField] protected Light lightSource;
        [SerializeField] protected AnimationCurve intensityOverFuel;
        [SerializeField] protected Gradient colorOverFuel;

        [Header("Particle")]
        [SerializeField] protected GameObject particleInstance;
        [SerializeField] protected AnimationCurve scaleOverFuel;

        [Header("Audio")]
        [SerializeField] protected AudioSource audioSource;
        [SerializeField] protected AnimationCurve volumeOverFuel;
        [SerializeField] protected float _maxVolume = 0.2f;

        private FuelBurner _burner;

        public void Init(FuelBurner burner)
        {
            _burner = burner;
        }

        public override void Enable()
        {
            throw new System.NotImplementedException();
        }

        public override void Disable()
        {
            throw new System.NotImplementedException();
        }

        public override void Update(float deltaTime)
        {
            float normalizedFuel = _burner.CurrentFuel / _burner.MaxFuel;
            float clampedNormFuel = Mathf.Clamp01(normalizedFuel);
            UpdateLight(clampedNormFuel);
            UpdateParticle(clampedNormFuel);
            UpdateAudio(clampedNormFuel);
        }

        private void UpdateAudio(float normFuel)
        {
            if (audioSource == null) 
                return;
            audioSource.volume = _maxVolume * volumeOverFuel.Evaluate(normFuel);
        }

        private void UpdateLight(float normFuel)
        {
            if (lightSource == null) 
                return;
            lightSource.intensity = intensityOverFuel.Evaluate(normFuel);
            lightSource.color = colorOverFuel.Evaluate(normFuel);
        }

        private void UpdateParticle(float normFuel)
        {
            if (particleInstance == null) 
                return;
            particleInstance.transform.localScale = scaleOverFuel.Evaluate(normFuel) * Vector3.one;
        }
    }
}