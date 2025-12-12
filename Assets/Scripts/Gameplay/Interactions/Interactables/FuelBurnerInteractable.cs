using Arctic.Gameplay.Survival;
using UnityEngine;

namespace Arctic.Gameplay.Interaction.Interactables
{
    [RequireComponent(typeof(FuelBurner))]
    public sealed class FuelBurnerInteractable : InteractableBehavior
    {
        public override string Prompt => "Fuel: " + Mathf.RoundToInt(fuelBurner.CurrentFuel);

        private FuelBurner fuelBurner;

        private void OnValidate()
        {
            ValidateFuelBurner();
        }

        private void Awake()
        {
            ValidateFuelBurner();
        }

        private void ValidateFuelBurner() 
        {
            if(fuelBurner == null)
                fuelBurner = GetComponent<FuelBurner>();            
        }
    }
}