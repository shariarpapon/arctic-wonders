using UnityEngine;

namespace Arctic.Utilities
{
    /// <summary>
    /// Uses Physics.Raycast thus, the gameobject must have collider in order to be detecting via its component.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GameObjectScanner<T> where T : Component
    {
        [SerializeField] private float maxReach = 3.0f;
        [SerializeField] private LayerMask targetLayers = 1;
        [SerializeField] private Camera viewCamera = null;

        public float MaxReach => maxReach;
        public LayerMask InteractableLayers => targetLayers;
        public Camera Camera => viewCamera;

        /// <summary>
        /// Invoked when cursor is over a new gameobject.
        /// Parameter represents the new target gameobject.
        /// </summary>
        public event System.Action<T> OnNewTargetFound;

        /// <summary>
        /// Invoked when the current target is loses focus. 
        /// Parameter represents the gameobject that lost focus.
        /// </summary>
        public event System.Action<T> OnTargetLost;

        public bool HasTarget { get; private set; }
        public T CurrentTarget => currentTarget;
        public RaycastHit LastRayHitInfo => hitInfo;
        public Camera ViewCamera => viewCamera;

        private T currentTarget = null;
        private RaycastHit hitInfo;

        public void SetMaxReach(float maxReach) => this.maxReach = maxReach;

        public void SetTargetLayers(LayerMask layers) => targetLayers = layers;

        public void SetViewCamera(Camera camera) => viewCamera = camera;

        public void Update()
        {
            DetectAndUpdateTarget();
        }

        private void DetectAndUpdateTarget()
        {
            if (RaycastUtils.TryRaycastFromMousePoint(viewCamera, targetLayers, maxReach, out hitInfo))
            {
                if (hitInfo.transform.TryGetComponent(out T target))
                {
                    HasTarget = true;
                    SetTarget(target);
                }
            }
            else SetTarget(null);
        }

        private void SetTarget(T newTarget)
        {
            if (currentTarget == newTarget) return;

            //if current target isnt null, invoke OnTargetLost with it before asigning the new target.
            if (currentTarget != null)
                OnTargetLost?.Invoke(currentTarget);

            //if new target isnt null, invoke OnNewTarget with it.
            if (newTarget != null)
                NewTargetFound(newTarget);

            currentTarget = newTarget;
        }

        private void NewTargetFound(T newTarget)
        {
            OnNewTargetFound?.Invoke(newTarget);
        }

        /// <returns>True if an target component was retrieved from the active target or false otherwise.</returns>
        public bool TryGetTargetsAttachedComponent(out T targetComp)
        {
            targetComp = null;
            if (currentTarget == null)
                return false;
            return currentTarget.TryGetComponent<T>(out targetComp);
        }

    }
}