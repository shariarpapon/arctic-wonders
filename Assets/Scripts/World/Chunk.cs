using System.Collections.Generic;
using UnityEngine;

namespace Arctic.World
{
    public class Chunk
    {
        public GameObject ChunkInstance { get; private set; } = null;
        public bool HasInstance => ChunkInstance != null;

        public readonly Vector3 position;
        public readonly Vector3 size;

        private readonly HashSet<ChunkObject> chunkObjects;

        public event System.Action<ChunkObject> OnChunkObjectRegistered;
        public event System.Action<ChunkObject> OnChunkObjectUnregistered;


        public Chunk(Vector3 position, Vector3 size)
        {
            this.position = position;
            this.size = size;
            chunkObjects = new HashSet<ChunkObject>();
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

        public void SetActive(bool active) 
        {
            if (!HasInstance)
                return;
            ChunkInstance.SetActive(active);
        }

        public void RegisterChunkObject(ChunkObject chunkObject, bool setParent = true)
        {
            if (chunkObject == null) 
            {
                Debug.LogError("Cannot register null ChunkObject.");
                return;
            }

            if (!chunkObjects.Add(chunkObject))
            {
                Debug.LogWarning($"Chunk object <{chunkObject.transform.name}> is already registered in chunk$<{position}>.");
                return;
            }

            if (setParent && HasInstance)
                chunkObject.transform.SetParent(ChunkInstance.transform);
            OnChunkObjectRegistered?.Invoke(chunkObject);
        }

        public void UnregisterChunkObject(ChunkObject chunkObject, bool clearParent = true) 
        {
            if (chunkObject == null)
            {
                Debug.LogError("Cannot unregister null ChunkObject.");
                return;
            }

            if (!chunkObjects.Remove(chunkObject))
                return;
            
            if (clearParent)
                chunkObject.transform.SetParent(null);
            
            OnChunkObjectUnregistered?.Invoke(chunkObject);
        }
    }
}
 