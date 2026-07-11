using Arctic.Utilities.Trackers;
using UnityEngine;

namespace Arctic.Gameplay.Interaction.Core
{
    public class InteractionInvoker : MonoBehaviour
    {

        [SerializeField] private KeyCode invokeKey = KeyCode.Mouse0;
        [SerializeField] private CursorComponentTracker<IInteractable> interactableTracker;
        public CursorComponentTracker<IInteractable> InteractableTracker => interactableTracker;

        /// <summary>
        /// Invoked everytime interaction is attempted.
        /// </summary>
        public event System.Action<InteractionInvoker, IInteractable> OnAttempted;
        public event System.Action<InteractionInvoker, IInteractable> OnInteract;
        public event System.Action OnEnabled;
        public event System.Action OnDisabled;
        public bool IsEnabled { get; private set; } = true;

        private void Update()
        {
            if (!IsEnabled) 
                return;

            interactableTracker.Tick();
            if (Input.GetKeyDown(invokeKey))
                if (interactableTracker.HasTarget)
                {
                    OnAttempted?.Invoke(this, interactableTracker.TargetComponent);
                    if (interactableTracker.TargetComponent.Interact(this))
                        OnInteract?.Invoke(this, interactableTracker.TargetComponent);
                }
        }

        public void SetEnabled(bool enable) 
        {
            if (IsEnabled == enable)
                return;

            IsEnabled = enable;
            if (enable) OnEnabled?.Invoke();
            else OnDisabled?.Invoke();
        }
    }
}