using UnityEngine;

namespace Arctic.World
{
    public class ThermalReactor : MonoBehaviour
    {
        [SerializeField]
        private float size;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, size);
        }
    }
}