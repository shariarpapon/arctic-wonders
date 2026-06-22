using UnityEngine;

namespace Arctic.World
{
    [RequireComponent(typeof(SphereCollider))]
    public class ThermalZone : MonoBehaviour
    {
        [SerializeField]
        private float tempOffsetF = 0f;

        [SerializeField]
        private float effectRadius = 10f;

        [SerializeField, Tooltip("0 = closest, 1 = farthest")]
        private AnimationCurve tempOffsetByDist = AnimationCurve.Linear(0f, 1f, 1f, 0f);

        private void Awake()
        {
            SphereCollider collider = GetComponent<SphereCollider>();
            if (!collider)
                collider = gameObject.AddComponent<SphereCollider>();
            collider.isTrigger = true;
            collider.radius = effectRadius;
        }

        public float GetTemperatureOffset(Vector3 position)
        {
            float distance = Vector3.Distance(transform.position, position);
            if (distance > effectRadius)
                return 0f;
            float normalizedDistance = distance / effectRadius;
            float curveValue = tempOffsetByDist.Evaluate(normalizedDistance);
            return tempOffsetF * curveValue;
        }

        public bool InEffectRadius(Vector3 position) 
        {
            return (position - transform.position).sqrMagnitude <= effectRadius * effectRadius;
        }
    }
} 