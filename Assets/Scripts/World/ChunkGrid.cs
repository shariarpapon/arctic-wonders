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

        private Dictionary<Vector3Int, Chunk> _chunks;

        public ChunkGrid(Vector2 gridSize, Vector3 chunkSize, Vector3 worldCenter)
        {
            this.worldSize = gridSize;
            this.chunkSize = chunkSize;
            this.worldCenter = worldCenter;
            this.worldMin = new Vector3(worldCenter.x - gridSize.x / 2.0f, worldCenter.y, worldCenter.z - gridSize.y / 2.0f);
            this.chunkCountX = Mathf.CeilToInt(worldSize.x / chunkSize.x);
            this.chunkCountZ = Mathf.CeilToInt(worldSize.y / chunkSize.z);
            _chunks = new Dictionary<Vector3Int, Chunk>();
        }

        public void GenerateChunks(bool createInstance) 
        {
            _chunks.Clear();
            Vector3Int coord;
            Vector3 worldPos = new Vector3(0, worldCenter.y, 0);
            for (int x = 0; x < chunkCountX; x++)
                for (int z = 0; z < chunkCountZ; z++) 
                {
                    coord = new Vector3Int(x, 0, z);
                    worldPos.x = worldMin.x + chunkSize.x * x;
                    worldPos.z = worldMin.z + chunkSize.z * z;
                    Chunk chunk = new Chunk(worldPos, chunkSize);
                    if (createInstance)
                        chunk.CreateInstance();
                    _chunks.Add(coord, chunk);
                }
        }

        public IEnumerable<Chunk> GetAllChunks() 
        {
            return _chunks.Values;
        }

        public Chunk GetWorldChunkByCoord(Vector3Int coord) 
        {
            if (_chunks.TryGetValue(coord, out Chunk chunk))
                return chunk;
            return null;
        }

        public Chunk GetChunkByWorldPosition(Vector3 worldPosition)
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