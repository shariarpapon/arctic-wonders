using Arctic.Gameplay.Survival;
using Arctic.Gameplay.Interaction.Core;
using UnityEngine;

namespace Arctic.Gameplay.Interaction
{
    [RequireComponent(typeof(FireBurnerSource))]
    public sealed class FuelBurnerInteractable : InteractableBehavior
    {
        public override string HoverPrompt => "Fuel: " + Mathf.RoundToInt(_burningActor.burner.CurrentFuel);

        [SerializeField]
        private FireBurnerSource _burningActor = null;

        private void OnValidate()
        {
            ValidateProperties();
        }

        private void Awake()
        {
            ValidateProperties();
        }

        private void ValidateProperties() 
        {
            if(_burningActor == null)
                _burningActor = GetComponent<FireBurnerSource>();            
        }
    }
}