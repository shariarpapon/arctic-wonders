using Arctic.World;
using UnityEngine;

namespace Arctic.Gameplay.Survival.Actors
{
    public class FuelBurnerActor : MonoBehaviour
    {
        public FuelBurner burner;

        [Header("Light")]
        [SerializeField] protected bool updateLight = true;
        [SerializeField] protected Light lightSource;
        [SerializeField] protected AnimationCurve intensityOverFuel;
        [SerializeField] protected Gradient colorOverFuel;

        [Header("Particle")]
        [SerializeField] protected bool updateParticle = true;
        [SerializeField] protected GameObject particleInstance;
        [SerializeField] protected AnimationCurve scaleOverFuel;

        [Header("Audio")]
        [SerializeField] protected bool updateAudio = true;
        [SerializeField] protected AudioSource audioSource;
        [SerializeField] protected AnimationCurve volumeOverFuel;

        [Header("Thermal Zone")]
        [SerializeField] protected bool updateThermalZone = true;
        [SerializeField] protected ThermalZone thermalZone;
        [SerializeField] protected AnimationCurve tempInfluenceOverFuel;
        
        protected virtual void OnEnable()
        {
            burner.OnFuelUpdate += OnFuelUpdated;
        }

        protected virtual void OnDisable()
        {
            burner.OnFuelUpdate -= OnFuelUpdated;
        }

        protected virtual void Update()
        {
            burner.Update(Time.deltaTime);
        }

        private void OnFuelUpdated(float fuel)
        {
            float normalizedFuel = fuel / burner.MaxFuel;
            float clampedNormFuel = Mathf.Clamp01(normalizedFuel);
            UpdateLight(clampedNormFuel);
            UpdateParticle(clampedNormFuel);
            UpdateAudio(clampedNormFuel);
            UpdateThermalZone(normalizedFuel);
        }

        private void UpdateThermalZone(float normFuel) 
        {
            if (!updateThermalZone || thermalZone == null)
                return;
            float influence = tempInfluenceOverFuel.Evaluate(normFuel);
            thermalZone.SetInfluence(influence);
        }

        private void UpdateAudio(float normFuel)
        {
            if (!updateAudio || audioSource == null) return;
            audioSource.volume = volumeOverFuel.Evaluate(normFuel);
        }

        private void UpdateLight(float normFuel)
        {
            if (!updateLight || lightSource == null) return;
            lightSource.intensity = intensityOverFuel.Evaluate(normFuel);
            lightSource.color = colorOverFuel.Evaluate(normFuel);
        }

        private void UpdateParticle(float normFuel)
        {
            if (!updateParticle || particleInstance == null) return;
            particleInstance.transform.localScale = scaleOverFuel.Evaluate(normFuel) * Vector3.one;
        }
    }
}