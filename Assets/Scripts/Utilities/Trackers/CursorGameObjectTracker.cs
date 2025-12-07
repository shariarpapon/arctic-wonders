using UnityEngine;

namespace Arctic.Utilities.Trackers
{
    /// <summary>
    /// Uses Physics.Raycast thus, the gameobject must have collider in order to be detecting via its component.
    /// </summary>
    [System.Serializable]
    public class CursorGameObjectTracker : ITrackerBehaviour<GameObject>
    {
        [SerializeField] private LayerMask targetLayers = 1;
        [SerializeField] private float maxReach = 3.0f;
        [SerializeField] private Camera viewCamera = null;

        public float MaxReach => maxReach;
        public LayerMask TargetLayers => targetLayers;

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

        public bool HasTarget => CurrentTarget != null;
        public GameObject CurrentTarget => currentTarget;
        public RaycastHit LastRayHitInfo => hitInfo;
        public Camera ViewCamera => viewCamera;

        private GameObject currentTarget = default;
        private RaycastHit hitInfo;

        public void SetMaxReach(float maxReach) => this.maxReach = maxReach;

        public void SetTargetLayers(LayerMask layers) => targetLayers = layers;

        public void SetViewCamera(Camera camera) => viewCamera = camera;

        public void Update()
        {
            if (RaycastUtils.TryRaycastFromMousePoint(viewCamera, targetLayers, maxReach, out hitInfo))
                SetTarget(hitInfo.transform.gameObject);
            else 
                SetTarget(default);
        }

        public void SetTarget(GameObject newTarget)
        {
            if (currentTarget == newTarget) return;
            //if current target isnt null, invoke OnTargetLost with it before asigning the new target.
            if (currentTarget != null)
                OnTargetLost?.Invoke(currentTarget);
            //if new target isnt null, invoke OnNewTarget with it.
            if (newTarget != null)
                OnNewTargetFound?.Invoke(newTarget);
            currentTarget = newTarget;
        }
    }
}