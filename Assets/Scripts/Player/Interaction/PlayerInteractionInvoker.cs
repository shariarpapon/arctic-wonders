using Arctic.Gameplay.Interaction;
using Arctic.InputManagement;
using UnityEngine;

namespace Arctic.Player.Interaction
{
    public class PlayerInteractionInvoker : MonoBehaviour
    {
        [SerializeField] private GameObject interactionSource;

        public InteractableScanner scanner;

        private void Update()
        {
            scanner.Update();
            if (InputManager.Provider.MouseInteract)
                if (scanner.TryGetInteractable(out var interactable))
                    interactable.Interact(interactionSource);
        }
    }
}