using System.Collections.Generic;
using UnityEngine;

namespace Arctic.World
{
    public sealed class ChunkGrid
    {
        public readonly Vector2 worldSize;
        public readonly Vector3 worldCenter;
        public readonly Vector3 worldMin;
        public readonly Vector3 chunkSize;
        public readonly int chunkCountX;
        public readonly int chunkCountZ;

        private Dictionary<Vector3Int, WorldChunk> _chunks;

        public ChunkGrid(Vector2 gridSize, Vector3 chunkSize, Vector3 worldCenter, bool createInstance = false)
        {
            this.worldSize = gridSize;
            this.chunkSize = chunkSize;
            this.worldCenter = worldCenter;
            this.worldMin = new Vector3(worldCenter.x - gridSize.x / 2.0f, worldCenter.y, worldCenter.z - gridSize.y / 2.0f);
            this.chunkCountX = Mathf.CeilToInt(worldSize.x / chunkSize.x);
            this.chunkCountZ = Mathf.CeilToInt(worldSize.y / chunkSize.z);
            _chunks = new Dictionary<Vector3Int, WorldChunk>();
            GenerateChunks(createInstance);
        }

        private void GenerateChunks(bool createInstance) 
        {
            Vector3Int coord;
            Vector3 worldPos = new Vector3(0, worldCenter.y, 0);
            for (int x = 0; x < chunkCountX; x++)
                for (int z = 0; z < chunkCountZ; z++) 
                {
                    coord = new Vector3Int(x, 0, z);
                    worldPos.x = worldMin.x + chunkSize.x * x;
                    worldPos.z = worldMin.z + chunkSize.z * z;
                    WorldChunk chunk = new WorldChunk(worldPos, chunkSize, createInstance);
                    _chunks.Add(coord, chunk);
                }
        }

        public IEnumerable<WorldChunk> GetAllChunks() 
        {
            return _chunks.Values;
        }

        public WorldChunk GetWorldChunkByCoord(Vector3Int coord) 
        {
            if (_chunks.TryGetValue(coord, out WorldChunk chunk))
                return chunk;
            return null;
        }

        public WorldChunk GetChunkByWorldPosition(Vector3 worldPosition)
        {
            Vector3Int coord = GridCoordFromWorldPos(worldPosition);
            return GetWorldChunkByCoord(coord);
        }

        public Vector3Int GridCoordFromWorldPos(Vector3 worldPos)
        {
            float localX = worldPos.x - worldMin.x;
            float localZ = worldPos.z - worldMin.z;
            Vector3Int coord = Vector3Int.zero;
            coord.x = Mathf.FloorToInt(localX / chunkSize.x);
            coord.z = Mathf.FloorToInt(localZ / chunkSize.z);
            return coord;
        }

        public void SetInstanceParent(Transform parent) 
        {
            if (_chunks == null) return;
            foreach (var c in _chunks.Values) 
            {
                if (!c.HasInstance) continue;
                c.ChunkInstance.transform.SetParent(parent);
            }
        }

        public void AddComponentsToChunkInstances(params System.Type[] types)
        {
            foreach (var c in _chunks.Values)
                c.AddComponentsToInstance(types);
        }
    }
}