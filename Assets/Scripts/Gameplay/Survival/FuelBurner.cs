using UnityEngine;

namespace Arctic.Gameplay.Survival
{
    [System.Serializable]
    public class FuelBurner
    {
        [SerializeField] private float fuel = 100.0f;
        [SerializeField] private float maxFuel = 9999999f;
        [SerializeField] private float burnRate = 0.25f;

        public FuelBurner(float fuel, float burnRate) 
        {
            this.fuel = fuel;
            this.burnRate = burnRate;
        }

        public float CurrentFuel => fuel;
        public float MaxFuel => maxFuel;
        public bool HasFuel => CurrentFuel > 0;

        public event System.Action OnEmpty;
        public event System.Action<float> OnUpdate;
        public event System.Action OnFull;
  
        public void Burn(float deltaTime)
        {
            if (HasFuel)
            {
                float burnAmount = deltaTime * burnRate;
                Remove(burnAmount);
            }
        }

        public void Add(float amt) => SetFuel(this.fuel + amt);

        public void Remove(float amt) => SetFuel(this.fuel - amt);

        protected void SetFuel(float newAmount)
        {
            float clampedAmount = Mathf.Clamp(newAmount, 0, maxFuel);
            if (Mathf.Approximately(fuel, clampedAmount))
                return;
            fuel = clampedAmount;
            OnUpdate?.Invoke(fuel);
            if (fuel <= 0)
                OnEmpty?.Invoke();
            else if (fuel >= maxFuel)
                OnFull?.Invoke();
            
        }
    }
}