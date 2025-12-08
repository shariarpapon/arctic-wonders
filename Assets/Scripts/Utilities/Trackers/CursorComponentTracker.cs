using UnityEngine;

namespace Arctic.Utilities.Trackers
{
    /// <summary>
    /// Uses Physics.Raycast thus, the gameobject must have collider in order to be detecting via its component.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [System.Serializable]
    public class CursorComponentTracker<T> : CursorGameObjectTracker where T : class
    {
        private T target;
        public T  TargetComponent => target;
        public new bool HasTarget => target != null;

        public override void SetTarget(GameObject newTarget)
        {
            base.SetTarget(newTarget);

            if(!base.CurrentTarget || !base.CurrentTarget.TryGetComponent(out target))
                target = null;
        }
    }
}