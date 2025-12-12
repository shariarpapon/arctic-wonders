using UnityEngine;

namespace Arctic.Gameplay.Survival
{
    public sealed class FireSource : FuelBurner
    {
        [Space]
        [SerializeField] private float maxInterpolationFuel = 100.0f;
        [SerializeField] private bool overwriteToBurnerMax = true;

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


        protected override void OnFuelUpdated(float fuel)
        {
            float normalizedFuel = fuel / maxInterpolationFuel;
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
                maxInterpolationFuel = Mathf.Max(0.01f, MaxFuel);
            else
                maxInterpolationFuel = Mathf.Max(0.01f, maxInterpolationFuel);
        }
    }
}