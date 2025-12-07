using Arctic.Gameplay.Survival;
using UnityEngine;

namespace Arctic.Gameplay.Interaction.Interactables
{
    [RequireComponent(typeof(FireSource))]
    public sealed class CampfireInteractable : InteractableBase
    {
        public override string Prompt => "Fuel: " + Mathf.RoundToInt(campfire.fuelBurner.CurrentFuel);

        private FireSource campfire;

        private void Awake()
        {
            campfire = GetComponent<FireSource>();
        }

        public override void Interact(GameObject source)
        {
            
        }
    }
}