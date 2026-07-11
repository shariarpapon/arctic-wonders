using Arctic.World;
using UnityEngine;
using Arctic.Gameplay.Survival.Core;

namespace Arctic.Gameplay.Survival
{
    public sealed class FireBurnerSource : MonoBehaviour
    {
        public FuelBurner burner;

        [Header("Light")]
        [SerializeField] private bool updateLight = true;
        [SerializeField] private Light lightSource;
        [SerializeField] private AnimationCurve intensityOverFuel;
        [SerializeField] private Gradient colorOverFuel;

        [Header("Particle")]
        [SerializeField] private bool updateParticle = true;
        [SerializeField] private GameObject particleInstance;
        [SerializeField] private AnimationCurve scaleOverFuel;

        [Header("Audio")]
        [SerializeField] private bool updateAudio = true;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AnimationCurve volumeOverFuel;

        [Header("Thermal Zone")]
        [SerializeField] bool updateThermalZone = true;
        [SerializeField] ThermalZone thermalZone;
        [SerializeField] AnimationCurve tempInfluenceOverFuel;
        
        private void OnEnable()
        {
            burner.OnFuelUpdate += OnFuelUpdated;
        }

        private void OnDisable()
        {
            burner.OnFuelUpdate -= OnFuelUpdated;
        }

        private void Update()
        {
            burner.Burn(Time.deltaTime);
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