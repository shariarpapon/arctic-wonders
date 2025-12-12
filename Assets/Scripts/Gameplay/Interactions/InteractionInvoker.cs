using Arctic.Utilities.Trackers;
using UnityEngine;

namespace Arctic.Gameplay.Interaction
{
    public class InteractionInvoker : MonoBehaviour
    {

        [SerializeField] private KeyCode invokeKey = KeyCode.Mouse0;
        [SerializeField] private CursorComponentTracker<IInteractable> interactableTracker;
        public CursorComponentTracker<IInteractable> InteractableTracker => interactableTracker;

        public event System.Action OnAttempted;
        public event System.Action OnEnabled;
        public event System.Action OnDisabled;
        public bool IsEnabled { get; private set; } = true;

        private void Update()
        {
            if (!IsEnabled) 
                return;

            interactableTracker.Tick();
            if (Input.GetKeyDown(invokeKey))
                if (interactableTracker.TargetComponent != null)
                {
                    interactableTracker.TargetComponent?.Interact(this);
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