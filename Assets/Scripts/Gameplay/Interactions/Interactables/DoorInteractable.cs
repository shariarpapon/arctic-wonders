using System.Collections;
using UnityEngine;

namespace Arctic.Gameplay.Interaction.Interactables
{
    public class DoorInteractable : InteractableBehavior
    {
        [System.Serializable]
        public enum DoorState { Locked, Closed, Open }

        [SerializeField] private DoorState state = DoorState.Closed;
        [SerializeField] private float swingAngle = 90f;
        [SerializeField] private float swingSpeed = 5.0f;

        private bool inTransition = false;
        private Collider doorCollider;
        private Quaternion openRotation;
        private Quaternion closedRotation;

        public override string Prompt 
        { 
            get 
            {
                switch (state) 
                {
                    default:
                        return base.Prompt;
                    case DoorState.Locked:
                        return "Unlock";
                    case DoorState.Open:
                        return "Close";
                    case DoorState.Closed:
                        return "Open";
                }
            }
        }

        private void Awake()
        {
            doorCollider = GetComponent<Collider>();
            closedRotation = transform.rotation;
            openRotation = closedRotation * Quaternion.Euler(swingAngle, 0f, 0f);
        }

        public override void Interact(InteractionInvoker invoker)
        {
            switch (state) 
            {
                case DoorState.Locked:
                    Unlock();
                    break;
                case DoorState.Closed:
                    MakeTransitionToTargetState(DoorState.Open);
                    break;
                case DoorState.Open:
                    MakeTransitionToTargetState(DoorState.Closed);
                    break;
            }
        }

        private void Unlock() 
        {
            state = DoorState.Closed;
        }

        private void MakeTransitionToTargetState(DoorState targetState) 
        {
            if (inTransition) return;
            inTransition = true;
            StartCoroutine(TransitionRoutine(targetState));
        }

        private IEnumerator TransitionRoutine(DoorState targetState) 
        {
            Quaternion currentRotation = transform.rotation;
            Quaternion targetRotation = targetState == DoorState.Open ? openRotation : closedRotation;
            doorCollider.isTrigger = true;
            float t = 0;
            while (t <= 1.0f) 
            {
                transform.rotation = Quaternion.Lerp(currentRotation, targetRotation, t);
                t += Time.deltaTime * swingSpeed;
                yield return null;
            }
            transform.rotation = targetRotation;
            doorCollider.isTrigger = false;
            state = targetState;
            inTransition = false;
        }

    }
}   