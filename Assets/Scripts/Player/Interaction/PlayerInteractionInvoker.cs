using Arctic.Gameplay.Interaction;
using Arctic.InputManagement;
using Arctic.Utilities.Trackers;
using UnityEngine;

namespace Arctic.Player.Interaction
{
    public class PlayerInteractionInvoker : MonoBehaviour
    {
        [SerializeField] private GameObject interactionSource;
        [SerializeField] private CursorGameObjectTracker tracker;

        public CursorGameObjectTracker InteractableTracker => tracker;

        private void Update()
        {
            tracker.Update();
            if (InputManager.Provider.MouseInteract && tracker.HasTarget)
                if (tracker.CurrentTarget.TryGetComponent(out IInteractable interactable))
                    interactable.Interact(interactionSource);
        }
    }
}