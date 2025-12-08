using Arctic.Utilities.Trackers;
using UnityEngine;

namespace Arctic.Gameplay.Interaction
{
    public class InteractionInvoker : MonoBehaviour
    {
        [SerializeField] private KeyCode invokeKey = KeyCode.Mouse0;
        [SerializeField] private GameObject invoker;
        [SerializeField] private CursorComponentTracker<IInteractable> interactableTracker;

        public CursorComponentTracker<IInteractable> InteractableTracker => interactableTracker;

        private void Update()
        {
            interactableTracker.Update();
            if (Input.GetKeyDown(invokeKey))
                if (interactableTracker.TargetComponent != null)
                    interactableTracker.TargetComponent?.Interact(invoker);
        }
    }
}