using UnityEngine;

namespace Arctic.Gameplay.Survival.Core
{
    [System.Serializable]
    public class FuelBurner
    {
        [SerializeField] private float _fuel = 100.0f;
        [SerializeField] private float _maxFuel = 100.0f;
        [SerializeField] private float _burnRate = 0.25f;

        public float CurrentFuel => _fuel;
        public float MaxFuel => _maxFuel;
        public bool HasFuel => CurrentFuel > 0;

        public event System.Action OnEmpty;
        public event System.Action<float> OnFuelUpdate;
        public event System.Action OnFull;

        public void Add(float amt) => SetFuel(_fuel + amt);

        public void Remove(float amt) => SetFuel(_fuel - amt);

        public void Burn(float deltaTime)
        {
            if (HasFuel)
            {
                float burnAmount = deltaTime * _burnRate;
                Remove(burnAmount);
            }
        }

        private void OnFuelUpdated(float fuelValue)
        {
            OnFuelUpdate?.Invoke(fuelValue);
        }

        private void SetFuel(float newAmount)
        {
            float clampedAmount = Mathf.Clamp(newAmount, 0, _maxFuel);
            if (Mathf.Approximately(_fuel, clampedAmount))
                return;
            _fuel = clampedAmount;
            OnFuelUpdated(clampedAmount);
            if (_fuel <= 0)
                OnEmpty?.Invoke();
            else if (_fuel >= _maxFuel)
                OnFull?.Invoke();
        }

    } 
}