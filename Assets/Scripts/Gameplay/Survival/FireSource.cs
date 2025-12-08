using UnityEngine;
using UnityEngine.Assertions;

namespace Arctic.Gameplay.Survival
{
    public sealed class FireSource : MonoBehaviour
    {
        public FuelBurner fuelBurner;
        [Space]
        [SerializeField] private float maxInterpolationFuel = 100.0f;
        [SerializeField] private bool overwriteToBurnerMax = false;

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

        private void Awake()
        {
            Assert.IsNotNull(fuelBurner);
            fuelBurner.OnUpdate += OnFuelUpdate;
        }

        private void OnDestroy()
        {
            fuelBurner.OnUpdate -= OnFuelUpdate;
        }

        private void Update()
        {
            fuelBurner.Burn(Time.deltaTime);
        }

        private void OnFuelUpdate(float currentFuel)
        {
            float normalizedFuel = currentFuel / maxInterpolationFuel;
            float clampedNormFuel = Mathf.Clamp01(normalizedFuel);
            UpdateLight(clampedNormFuel);
            UpdateParticle(clampedNormFuel);
            UpdateAudio(clampedNormFuel);
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

        private void OnValidate()
        {
            if (overwriteToBurnerMax)
                maxInterpolationFuel = Mathf.Max(0.01f, fuelBurner.MaxFuel);
            else
                maxInterpolationFuel = Mathf.Max(0.01f, maxInterpolationFuel);
        }
    }
}