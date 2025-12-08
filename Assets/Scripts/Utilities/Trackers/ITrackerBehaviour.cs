using UnityEngine;

namespace Arctic.Utilities.Trackers
{
    public interface ITrackerBehaviour<T> where T : class
    {
        public abstract event System.Action<T> OnNewTargetFound;
        public abstract event System.Action<T> OnTargetLost;
        public abstract T CurrentTarget { get; }
        public abstract bool HasTarget { get; }
        public abstract float MaxReach { get; }
        public abstract LayerMask TargetLayers { get; }
        public abstract Camera ViewCamera { get; }
        public abstract RaycastHit LastRayHitInfo { get; }

        public abstract void Update();
        public abstract void SetMaxReach(float maxReach);
        public abstract void SetTargetLayers(LayerMask layers);
        public abstract void SetViewCamera(Camera camera);
        public abstract void SetTarget(T newTarget);
    }
}