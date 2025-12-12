using UnityEngine;

namespace Arctic.Gameplay.Survival
{
    public class FuelBurner : MonoBehaviour
    {
        [SerializeField] private float fuel = 100.0f;
        [SerializeField] private float maxFuel = 100.0f;
        [SerializeField] private float burnRate = 0.25f;

        public float CurrentFuel => fuel;
        public float MaxFuel => maxFuel;
        public bool HasFuel => CurrentFuel > 0;

        public event System.Action OnEmpty;
        public event System.Action<float> OnFuelUpdate;
        public event System.Action OnFull;

        public void Add(float amt) => SetFuel(fuel + amt);

        public void Remove(float amt) => SetFuel(fuel - amt);

        protected virtual void Update()
        {
            Burn(Time.deltaTime);
        }

        protected virtual void OnFuelUpdated(float fuelValue)
        {
            OnFuelUpdate?.Invoke(fuelValue);
        }

        protected void Burn(float deltaTime)
        {
            if (HasFuel)
            {
                float burnAmount = deltaTime * burnRate;
                Remove(burnAmount);
            }
        }

        protected void SetFuel(float newAmount)
        {
            float clampedAmount = Mathf.Clamp(newAmount, 0, maxFuel);
            if (Mathf.Approximately(fuel, clampedAmount))
                return;
            fuel = clampedAmount;
            OnFuelUpdated(clampedAmount);
            if (fuel <= 0)
                OnEmpty?.Invoke();
            else if (fuel >= maxFuel)
                OnFull?.Invoke();
        }

    } 
}