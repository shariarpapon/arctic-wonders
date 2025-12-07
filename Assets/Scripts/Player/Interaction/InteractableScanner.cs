using Arctic.Gameplay.Interaction;
using Arctic.Utilities;
using UnityEngine;

namespace Arctic.Player.Interaction
{
    [System.Serializable]
    public class InteractableScanner
    {
        [SerializeField] private float maxReach = 3.0f;
        [SerializeField] private LayerMask interactableLayers = 1;
        [SerializeField] private Camera viewCamera = null;

        public float MaxReach => maxReach;
        public LayerMask InteractableLayers => interactableLayers;
        public Camera Camera => viewCamera;

        /// <summary>
        /// Invoked when cursor is over a new gameobject.
        /// Parameter represents the new target gameobject.
        /// </summary>
        public event System.Action<GameObject> OnNewTargetFound;

        /// <summary>
        /// Invoked when the current target is loses focus. 
        /// Parameter represents the gameobject that lost focus.
        /// </summary>
        public event System.Action<GameObject> OnTargetLost;

        public event System.Action<IInteractable> OnInteractableTargetFound;

        public bool HasTarget { get; private set; }
        public GameObject CurrentTarget => target;
        public RaycastHit LastRayHitInfo => hitInfo;
        public Camera ViewCamera => viewCamera;

        private GameObject target = null;
        private RaycastHit hitInfo;

        public void SetMaxReach(float maxReach) => this.maxReach = maxReach;

        public void SetInteractableLayers(LayerMask layers) => interactableLayers = layers;

        public void SetViewCamera(Camera camera) => viewCamera = camera;

        public void Update()
        {
            DetectAndUpdateTarget();
        }

        private void DetectAndUpdateTarget()
        {
            if (RaycastUtils.TryRaycastFromMousePoint(viewCamera, interactableLayers, maxReach, out hitInfo))
            {
                HasTarget = true;
                SetTarget(hitInfo.transform.gameObject);
            }
            else SetTarget(null);
        }

        private void SetTarget(GameObject newTarget)
        {
            if (target == newTarget) return;

            //if current target isnt null, invoke OnTargetLost with it before asigning the new target.
            if (target != null)
                OnTargetLost?.Invoke(target);

            //if new target isnt null, invoke OnNewTarget with it.
            if (newTarget != null)
                NewTargetFound(newTarget);

            target = newTarget;
        }

        private void NewTargetFound(GameObject newTarget)
        {
            OnNewTargetFound?.Invoke(newTarget);
            if (newTarget.TryGetComponent<IInteractable>(out var interactable))
                OnInteractableTargetFound?.Invoke(interactable);
        }

        /// <returns>True if an IInteractable was retrieved from the active target or false otherwise.</returns>
        public bool TryGetInteractable(out IInteractable interactable)
        {
            interactable = null;
            if (target == null)
                return false;
            return target.TryGetComponent<IInteractable>(out interactable);
        }
    }
}