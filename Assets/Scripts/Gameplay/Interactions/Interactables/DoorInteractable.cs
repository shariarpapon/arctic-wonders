using System.Collections;
using UnityEngine;
using Arctic.Gameplay.Interaction.Core;

namespace Arctic.Gameplay.Interaction
{
    public class DoorInteractable : InteractableBehavior
    {
        [System.Serializable]
        public enum DoorState { Locked, Closed, Open }
        [System.Serializable]
        public enum SwingAxis { X, Y, Z }


        [SerializeField] private DoorState state = DoorState.Closed;
        [SerializeField] private float swingAngle = 90f;
        [SerializeField] private float swingSpeed = 5.0f;
        [SerializeField] private SwingAxis swingAxis = SwingAxis.Y;

        private bool inTransition = false;
        private Collider doorCollider;
        private Quaternion openRotation;
        private Quaternion closedRotation;

        public override string HoverPrompt 
        { 
            get 
            {
                switch (state) 
                {
                    default:
                        return base.HoverPrompt;
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
            if (doorCollider is MeshCollider meshCollider)
                meshCollider.convex = true;

            closedRotation = transform.rotation;

            Quaternion axisRot = swingAxis switch
            {
                SwingAxis.X => Quaternion.Euler(swingAngle, 0f, 0f),
                SwingAxis.Y => Quaternion.Euler(0f, swingAngle, 0f),
                SwingAxis.Z => Quaternion.Euler(0f, 0f, swingAngle),
                _ => Quaternion.identity
            };

            openRotation = closedRotation * axisRot;
        }

        public override bool Interact(InteractionInvoker invoker)
        {
            if (inTransition)
                return false;

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
            return true;
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