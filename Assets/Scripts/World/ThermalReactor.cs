using System.Collections.Generic;
using UnityEngine;

namespace Arctic.World
{
    public class ThermalReactor : MonoBehaviour
    {
        [SerializeField]
        private float queryRadius = 1.0f;
        [SerializeField]
        private LayerMask queryLayers = 0;


        public float GetEffectiveTempF() 
        {
            float effectiveTemp = WorldClimateManager.Instance.CurrentAmbientTempF;
            ThermalZone[] thermalZones = QueryThermalZones();
            for(int i = 0; i < thermalZones.Length; i++)
                effectiveTemp += thermalZones[i].GetTemperatureOffset(transform.position);
            return effectiveTemp;
        }


        private ThermalZone[] QueryThermalZones()
        {
            Collider[] candidates = Physics.OverlapSphere(transform.position, queryRadius, queryLayers);
            List<ThermalZone> thermalZones = new List<ThermalZone>();
            foreach (Collider c in candidates) 
            {
                if (!c.TryGetComponent<ThermalZone>(out ThermalZone zone))
                    continue;
                thermalZones.Add(zone);
            }
            return thermalZones.ToArray();
        } 


#if UNITY_EDITOR
        [Space]
        public bool drawGizmos = true;
        private void OnDrawGizmos()
        {
            if (!drawGizmos) return;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, queryRadius);
        }
#endif

    }
}