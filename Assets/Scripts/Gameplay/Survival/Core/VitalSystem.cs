using UnityEngine;

namespace Arctic.Gameplay.Survival.Core
{
    [System.Serializable]
    public class VitalSystem
    {
        public event System.Action<float> OnHealthChanged;
        public event System.Action<float> OnHungerChanged;
        public event System.Action<float> OnThirstChanged;
        public event System.Action<float> OnBodyTempChanged;

        public float Health => _health;
        public float Hunger => _hunger;
        public float Thirst => _thirst;
        public float BodyTempF => _bodyTempF;

        [SerializeField] private float _health;
        [SerializeField] private float _hunger;
        [SerializeField] private float _thirst;
        [SerializeField] private float _bodyTempF;

        [Space]
        [SerializeField] private float _maxHealth = 100.0f;
        [SerializeField] private float _maxHunger = 100.0f;
        [SerializeField] private float _maxThirst = 100.0f;
        [SerializeField] private float _minBodyTempF = 95.0f;

        [Space]
        [SerializeField] private float _starvingHealthDecayRate = 0.1f;
        [SerializeField] private float _dehydrationHealthDecayRate = 0.1f;
        [SerializeField] private float _coldHealthDecayRate = 0.1f;

        [SerializeField] private float _hungerIncreaseRate = 0.02f;
        [SerializeField] private float _thirstIncreaseRate = 0.02f;

        public void InitDefault() 
        {
            SetHealth(_maxHealth);
            SetHunger(0);
            SetThirst(0);
            SetBodyTempF(98.0f);
        }

        public VitalSystem SetHealth(float health)
        {
            if (Mathf.Approximately(Health, health)) 
                return this;
            this._health = Mathf.Clamp(health, 0.0f, _maxHealth);
            OnHealthChanged?.Invoke(this._health);
            return this;
        }
        public VitalSystem SetHunger(float hunger)
        {
            if (Mathf.Approximately(Hunger, hunger)) 
                return this;
            this._hunger = Mathf.Clamp(hunger, 0.0f, _maxHunger);
            OnHungerChanged?.Invoke(this._hunger);
            return this;
        }
        
        public VitalSystem SetThirst(float thirst)
        {
            if (Mathf.Approximately(Thirst, thirst)) 
                return this;
            this._thirst = Mathf.Clamp(thirst, 0.0f, _maxThirst);
            OnThirstChanged?.Invoke(this._thirst);
            return this;
        }
        public VitalSystem SetBodyTempF(float bodyTempF)
        {
            if (Mathf.Approximately(BodyTempF, bodyTempF))
                return this;
            this._bodyTempF = bodyTempF;
            OnBodyTempChanged?.Invoke(this._bodyTempF);
            return this;
        }

        public void UpdateVitals(float deltaTime) 
        {
            SetHealth(_health - GetCumulativeHealthDecayRate() * deltaTime);
            SetHunger(_hunger + _hungerIncreaseRate * deltaTime);
            SetThirst(_thirst + _thirstIncreaseRate * deltaTime);
        }

        private float GetCumulativeHealthDecayRate() 
        {
            return (_hunger >= _maxHunger ? _starvingHealthDecayRate : 0.0f) + 
                   (_thirst >= _maxThirst ? _dehydrationHealthDecayRate : 0.0f) + 
                   (_bodyTempF < _minBodyTempF ? _coldHealthDecayRate : 0.0f);
        }
    }
}