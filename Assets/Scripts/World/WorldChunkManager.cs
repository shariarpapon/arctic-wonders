using Arctic.DebugTools;
using Arctic.Utilities.Generics;
using System.Collections.Generic;
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
        public Vector3 worldOffset = Vector3.zero;
        public float chunkSize = 64f;

        private Dictionary<Vector3Int, WorldChunk> chunkGrid = new Dictionary<Vector3Int, WorldChunk>();

        private void OnValidate()
        {
            const float minChunkSize = 0.01f;
            if (chunkSize < minChunkSize)
                chunkSize = minChunkSize;
        }

        public void GenerateChunks() 
        {
            if(chunkGrid != null) 
            {
                Debugc.LogEmphasis("chunk grid is alreeady initialized, clearing existing chunks before generation...");
                chunkGrid.Clear();
            }
            chunkGrid = GenerateChunkGrid();
        }

        private Dictionary<Vector3Int, WorldChunk> GenerateChunkGrid()
        {
            int chunkCountX = Mathf.CeilToInt(worldSize.x / chunkSize);
            int chunkCountZ = Mathf.CeilToInt(worldSize.y / chunkSize);
            Dictionary<Vector3Int, WorldChunk> chunks = new Dictionary<Vector3Int, WorldChunk>(chunkCountX * chunkCountZ);
            Vector3 worldPosition = Vector3.zero;
            Vector3Int gridLocation = Vector3Int.zero;
            for (int x = 0; x < chunkCountX; x++)
                for (int z = 0; z < chunkCountZ; z++)
                {
                    gridLocation = new(x, 0, z);
                    worldPosition = GetWorldPositionFromGridLocation(gridLocation);
                    chunks.Add(gridLocation, new WorldChunk(worldPosition, chunkSize));
                }   
            return chunks;
        }

        public WorldChunk GetChunkByWorldPosition(Vector3 worldPosition)
        {
            Vector3Int gridLocation = GetGridLocationFromWorldPosition(worldPosition);
            if (chunkGrid.TryGetValue(gridLocation, out WorldChunk chunk))
                return chunk;
            return null;
        }

        public Vector3Int GetGridLocationFromWorldPosition(Vector3 worldPosition)
        {
            int x = Mathf.FloorToInt((worldPosition.x - worldOffset.x) / chunkSize);
            int z = Mathf.FloorToInt((worldPosition.z - worldOffset.z) / chunkSize);
            return new Vector3Int(x, 0, z);
        }

        public Vector3 GetWorldPositionFromGridLocation(Vector3Int gridLocation)
        {
            return (Vector3)gridLocation * chunkSize + worldOffset;
        }

    }
}

