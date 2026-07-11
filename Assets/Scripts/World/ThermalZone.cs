using UnityEngine;

namespace Arctic.World
{
    [RequireComponent(typeof(SphereCollider))]
    public class ThermalZone : MonoBehaviour
    {
        [SerializeField]
        private float _tempOffsetF = 0f;
        [SerializeField]
        private float _effectRadius = 5.0f;
        [SerializeField, Range(0, 1)]
        private float _influence = 1.0f;

        [SerializeField, Tooltip("0 = closest, 1 = farthest")]
        private AnimationCurve tempOffsetByDist = AnimationCurve.Linear(0f, 1f, 1f, 0f);

        private void Awake()
        {
            SphereCollider collider = GetComponent<SphereCollider>();
            if (!collider)
                collider = gameObject.AddComponent<SphereCollider>();
            collider.isTrigger = true;
            collider.radius = _effectRadius;
        }

        public float GetTemperatureOffset(Vector3 position)
        {
            float distance = Vector3.Distance(transform.position, position);
            if (distance > _effectRadius)
                return 0f;
            float normalizedDistance = distance / _effectRadius;
            float curveValue = tempOffsetByDist.Evaluate(normalizedDistance);
            return _tempOffsetF * curveValue * _influence;
        }

        public void SetInfluence(float influence) 
        {
            _influence = Mathf.Clamp01(influence);
        }

        public bool InEffectRadius(Vector3 position) 
        {
            return (position - transform.position).sqrMagnitude <= _effectRadius * _effectRadius;
        }


#if UNITY_EDITOR
        [Space]
        public bool drawGizmos = true;
        private void OnDrawGizmos()
        {
            if (!drawGizmos) return;
            Gizmos.color = Color.orange;
            Gizmos.DrawWireSphere(transform.position, _effectRadius);
        }
#endif
    }
} 