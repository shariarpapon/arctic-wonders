using UnityEngine;

namespace Arctic.World
{
    public class WorldChunk
    {
        public readonly Vector3 position;
        public readonly float size;

        public WorldChunk(Vector3 position, float size)
        {
            this.position = position;
            this.size = size;
        }
    }
}
