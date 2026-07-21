using Arctic.Gameplay.Survival.Actors;
using UnityEngine;
using Arctic.Foundation.Interaction;

namespace Arctic.Gameplay.Interactables
{
    [RequireComponent(typeof(FuelBurnerActor))]
    public sealed class FuelBurnerInteractable : InteractableBehavior
    {
        public override string HoverPrompt => "Fuel: " + Mathf.RoundToInt(_burningActor.burner.CurrentFuel);

        [SerializeField]
        private FuelBurnerActor _burningActor = null;

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
                _burningActor = GetComponent<FuelBurnerActor>();            
        }
    }
}