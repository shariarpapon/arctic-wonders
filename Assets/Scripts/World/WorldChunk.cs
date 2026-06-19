using UnityEngine;

namespace Arctic.World
{
    public class WorldChunk
    {
        public GameObject ChunkInstance { get; private set; } = null;
        public bool HasInstance => ChunkInstance != null;

        public readonly Vector3 position;
        public readonly Vector3 size;

        public WorldChunk(Vector3 position, Vector3 size, bool createInstance)
        {
            this.position = position;
            this.size = size;

            if(createInstance)
                CreateInstance();
        }

        public void CreateInstance() 
        {
            if (HasInstance) 
            {
                Debug.LogWarning($"Chunk instance already exists at position {position}. Destroying old instance and creating new one.");
                GameObject.Destroy(ChunkInstance);
            }

            ChunkInstance = new GameObject($"Chunk({position.x},{position.z})");
            ChunkInstance.transform.position = position;
            ChunkInstance.transform.localScale = size;
        }

        public void AddComponentsToInstance(params System.Type[] types) 
        {
            if (!HasInstance) 
            {
                Debug.LogWarning($"Cannot add components to chunk at position {position} because it does not have an instance.");
                return;
            }
            foreach (var type in types) 
            {
                if (!typeof(Component).IsAssignableFrom(type)) 
                {
                    Debug.LogError($"Type {type} is not a Component and cannot be added to the chunk instance.");
                    continue;
                }
                ChunkInstance.AddComponent(type);
            }
        }
    }
}
