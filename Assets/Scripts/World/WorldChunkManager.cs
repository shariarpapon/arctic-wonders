using Arctic.Utilities.Generics;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;

// Runtime Navmesh Generation Snippet
/*
            using Unity.AI.Navigation;

            Debugc.LogInfo("Runtime NavMesh testing...");
            GameObject g = GameObject.Find("test_rt_navmesh");
            NavMeshSurface nms = g.AddComponent<NavMeshSurface>();
            nms.collectObjects = CollectObjects.Volume; //needed for volume collection
            nms.center = Vector3.zero; /relative to attached object
            nms.size = new Vector3(10, 1, 10);
            nms.BuildNavMesh();
            Debugc.LogConfirm("navmesh built.");
 */


namespace Arctic.World
{
    public class WorldChunkManager : PersistentSingletonMonobehaviour<WorldChunkManager>
    {
        public Vector2 worldSize = new Vector2(128f, 128f);
        public Vector3 worldCenter = Vector3.zero;
        public Vector3 chunkSize = new Vector3(256, 16, 256);
        
        [SerializeField]
        private bool _createInstances = true;

        private ChunkGrid _chunkGrid;

        
        protected override void OnSingletonEvaluated()
        {
            base.OnSingletonEvaluated();
            CreateChunkGrid();

            WorldChunk[] chunks = _chunkGrid.GetAllChunks().ToArray();
        }

        private void CreateChunkGrid() 
        {
            _chunkGrid = new ChunkGrid(worldSize, chunkSize, worldCenter, _createInstances);
            _chunkGrid.SetInstanceParent(transform);
            _chunkGrid.AddComponentsToChunkInstances(typeof(NavMeshSurface));
            InitChunkNavMeshSurface();
        }

        private void InitChunkNavMeshSurface() 
        {
            foreach (WorldChunk chunk in _chunkGrid.GetAllChunks())
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
    }
}

