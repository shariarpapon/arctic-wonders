using UnityEngine;

namespace Arctic.Utilities
{
    public static class RaycastUtils
    {
        /// <returns>True if the ray hits a collider or false otherwise.</returns>
        public static bool TryRaycastFromMousePoint(Camera cam, LayerMask mask, float range, out RaycastHit hit) 
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            return Physics.Raycast(ray, out hit, range, mask);
        }

        /// <returns>True if the ray hits a collider or false otherwise.</returns>
        public static bool TryRaycastFromMousePoint(Camera cam, LayerMask mask, out RaycastHit hit)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            return Physics.Raycast(ray, out hit, mask);
        }

        /// <summary>
        /// Casts a ray from the mouse point out into the scene and attemps to retrieve specified component type from the collider.
        /// </summary>
        /// <typeparam name="T">The component to retrieve from the hit game object.</typeparam>
        /// <returns>True if target component is found or false otherwise.</returns>
        public static bool TryRaycastFromMousePoint<T>(Camera cam, LayerMask layerMask, out T component) where T : Component
        {
            component = default;
            if (TryRaycastFromMousePoint(cam, layerMask, out RaycastHit hit)) 
                return hit.transform.TryGetComponent(out component);
            return false;
        }

        /// <summary>
        /// Casts a ray from the mouse point out into the scene and attemps to retrieve specified component type from the hit collider.
        /// </summary>
        /// <typeparam name="T">The component to retrieve from the hit game object.</typeparam>
        /// <returns>True if target component is found or false otherwise.</returns>
        public static bool TryRaycastFromMousePoint<T>(Camera cam, LayerMask layerMask, float range, out T component) where T : Component
        {
            component = default;
            if (TryRaycastFromMousePoint(cam, layerMask, range, out RaycastHit hit))
                return hit.transform.TryGetComponent(out component);
            return false;
        }
    }
}