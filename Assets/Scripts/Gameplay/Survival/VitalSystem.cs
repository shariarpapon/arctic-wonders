using JetBrains.Annotations;
using UnityEngine;

namespace Arctic.Gameplay.Survival
{
    [System.Serializable]
    public class VitalSystem
    {
        public event System.Action<float> OnHealthChanged;
        public event System.Action<float> OnMaxHealthChanged;
        
        public event System.Action OnDeath;
        public event System.Action<float> OnRevive;

        public event System.Action<float> OnHungerChanged;
        public event System.Action<float> OnThirstChanged;
        public event System.Action<float> OnBodyTempFChanged;

        public float Health => _health;
        public float Hunger => _hunger;
        public float Thirst => _thirst;
        public float BodyTempF => _bodyTempF;
        public float MaxHealth => _maxHealth;
        public float MaxHunger => _maxHunger;
        public float MaxThirst => _maxThirst;
        public bool IsDead => _isDead;

        public float StarvingHealthDecayRate => _starvingHealthDecayRate;
        public float DehydrationHealthDecayRate => _dehydrationHealthDecayRate;
        public float ColdHealthDecayRate => _coldHealthDecayRate;
        public float HungerIncreaseRate => _hungerIncreaseRate;
        public float ThirstIncreaseRate => _thirstIncreaseRate;

        [SerializeField] private float _health;
        [SerializeField] private float _hunger;
        [SerializeField] private float _thirst;
        [SerializeField] private float _bodyTempF;
        private bool _isDead;

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

        public void Init() 
        {
            SetHealth(_maxHealth);
            SetHunger(0);
            SetThirst(0);
            SetBodyTempF(97.6f);
        }

        public VitalSystem SetHealth(float health)
        {
            if (Mathf.Approximately(_health, health))
                return this;

            _health = Mathf.Clamp(health, 0.0f, _maxHealth);
            OnHealthChanged?.Invoke(this._health);

            if (_health <= 0)
            {
                if (!_isDead) Die();
            }
            else if(_isDead)
            {
                Revive();
            }

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
            OnBodyTempFChanged?.Invoke(this._bodyTempF);
            return this;
        }

        public VitalSystem SetMaxHealth(float maxHealth) 
        {
            if (Mathf.Approximately(maxHealth, _maxHealth))
                return this;

            _maxHealth = Mathf.Max(0.001f, maxHealth);
            OnMaxHealthChanged?.Invoke(_maxHealth);
            return this;
        }

        public VitalSystem SetStarvingHealthDecayRate(float rate) 
        {
            _starvingHealthDecayRate = Mathf.Max(0.0f, rate);
            return this;
        }

        public VitalSystem SetDehydrationHealthDecayRate(float rate) 
        {
            _dehydrationHealthDecayRate = Mathf.Max(0.0f, rate);
            return this;
        }

        public VitalSystem SetColdHealthDecayRate(float rate) 
        {
            _coldHealthDecayRate = Mathf.Max(0.0f, rate);
            return this;
        }

        public VitalSystem SetHungerIncreaseRate(float rate) 
        {
            _hungerIncreaseRate = Mathf.Max(0.0f, rate);
            return this;
        }

        public VitalSystem SetThirstIncreaseRate(float rate) 
        {
            _thirstIncreaseRate = Mathf.Max(0.0f, rate);
            return this;
        }

        public void UpdateVitals(float deltaTime) 
        {
            SetHealth(_health - GetCumulativeHealthDecayRate() * deltaTime);
            SetHunger(_hunger + _hungerIncreaseRate * deltaTime);
            SetThirst(_thirst + _thirstIncreaseRate * deltaTime);
        }

        private void Die() 
        {
            _isDead = true;
            OnDeath?.Invoke();
        }

        private void Revive() 
        {
            _isDead = false;
            OnRevive?.Invoke(_health);
        }

        private float GetCumulativeHealthDecayRate() 
        {
            return (_hunger >= _maxHunger ? _starvingHealthDecayRate : 0.0f) + 
                   (_thirst >= _maxThirst ? _dehydrationHealthDecayRate : 0.0f) + 
                   (_bodyTempF < _minBodyTempF ? _coldHealthDecayRate : 0.0f);
        }
    }
}