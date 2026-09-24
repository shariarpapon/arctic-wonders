using Arctic.Gameplay.Survival.Actors;
using UnityEngine;
using Arctic.Foundation.Interaction;

namespace Arctic.Gameplay.Interactables
{
    [RequireComponent(typeof(FuelBurnerActor))]
    public sealed class FuelBurnerInteractable : InteractableBehavior
    {
        public override string HoverPrompt => "Fuel: " + Mathf.RoundToInt(_burnerActor.Burner.CurrentFuel);

        [SerializeField]
        private FuelBurnerActor _burnerActor = null;

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
            if(_burnerActor == null)
                _burnerActor = GetComponent<FuelBurnerActor>();            
        }
    }
}