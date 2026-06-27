using UnityEngine;

namespace Arctic.World
{
    public class ChunkObject : MonoBehaviour
    {
        public bool IsRegistered { get; private set; } = false;
        private void Start()
        {
            if (!WorldChunkManager.Instance.RegisterChunkObject(this, true)) 
            {
                Debug.LogError($"Failed to register ChunkObject at position: {transform.position.ToString()}.");
                return;
            }
            IsRegistered = true;
        }

        private void OnDestroy()
        {
            if (!WorldChunkManager.Instance.UnregisterChunkObject(this, false))
            {
                Debug.LogWarning($"Failed to unregister ChunkObject at position: {transform.position.ToString()}.");
                return;
            }
            IsRegistered = false;
        }
    }
}