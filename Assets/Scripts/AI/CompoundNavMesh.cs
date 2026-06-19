using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

namespace Arctic.AI
{
    public sealed class CompoundNavMesh
    {
        public Vector2 size = new Vector2(100, 100);
        public int xPartitions = 0;
        public int yPartitions = 0;
    }
}