using Arctic.Utilities.Generics;
using Unity.AI.Navigation;
using UnityEngine;

namespace Arctic.World
{
    public class WorldChunkManager : PersistentSingletonMonobehaviour<WorldChunkManager>
    {
        public Vector2 worldSize = new Vector2(128f, 128f);
        public Vector3 worldCenter = Vector3.zero;
        public Vector3 chunkSize = new Vector3(256, 16, 256);

        [Space]
        public bool createInstances = true;

        [HideInInspector]
        public bool enableChunkCulling = true;
        
        [HideInInspector]
        public GameObject viewer = null;

        private ChunkGrid _chunkGrid;

        
        protected override void OnSingletonEvaluated()
        {
            base.OnSingletonEvaluated();
            CreateChunkGrid();
        }

        private void CreateChunkGrid() 
        {
            _chunkGrid = new ChunkGrid(worldSize, chunkSize, worldCenter);
            _chunkGrid.GenerateChunks(createInstances); 
            _chunkGrid.SetInstanceParent(transform);
            _chunkGrid.AddComponentsToChunkInstances(typeof(NavMeshSurface));
            InitChunkNavMeshSurface();
        }

        private void InitChunkNavMeshSurface() 
        {
            foreach (Chunk chunk in _chunkGrid.GetAllChunks())
            {
                if (!chunk.HasInstance)
                {
                    Debug.LogWarning($"Chunk at position {chunk.position} does not have an instance.");
                    continue;
                }

                NavMeshSurface nms = chunk.ChunkInstance.GetComponent<NavMeshSurface>();
                if (!nms)
                {
                    Debug.LogWarning($"Chunk at position {chunk.position} does not have a NavMeshSurface component.");
                    continue;
                }

                nms.collectObjects = CollectObjects.Volume;
                nms.center = new Vector3(chunk.size.x / 2f, 0, chunk.size.z / 2f);
                nms.size = chunk.size;
                nms.BuildNavMesh();
            }
        }

        public bool RegisterChunkObject(ChunkObject chunkObject, bool setParent) 
        {
            Chunk chunk = _chunkGrid.GetChunkByWorldPosition(chunkObject.transform.position);
            if (chunk != null)
            {
                chunk.RegisterChunkObject(chunkObject, setParent);
                return true;
            }
            return false;
        }

        public bool UnregisterChunkObject(ChunkObject chunkObject, bool clearParent) 
        {
            Chunk chunk = _chunkGrid.GetChunkByWorldPosition(chunkObject.transform.position);
            if (chunk != null)
            {
                chunk.UnregisterChunkObject(chunkObject, clearParent);
                return true;
            }
            return false;
        }
    }
}

