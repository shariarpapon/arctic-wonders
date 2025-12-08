using Arctic.Gameplay.Survival;
using UnityEngine;

namespace Arctic.Gameplay.Interaction.Interactables
{
    [RequireComponent(typeof(FireSource))]
    public sealed class FireSourceInteractable : InteractableBase
    {
        public override string Prompt => "Fuel: " + Mathf.RoundToInt(fuelBurner.fuelBurner.CurrentFuel);

        private FireSource fuelBurner;

        private void Awake()
        {
            fuelBurner = GetComponent<FireSource>();
        }

        public override void Interact(GameObject source)
        {
            
        }
    }
}