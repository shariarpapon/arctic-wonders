using UnityEngine;
using UnityEditor;
using Arctic.World;

namespace Arctic.Woirld.CustomEditors
{
    [CustomEditor(typeof(WorldChunkManager))]
    public class WorldChunkManagerEditor : Editor
    {
        private MeshFilter _worldMeshFilter;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Helper Tools", EditorStyles.boldLabel);
            _worldMeshFilter = EditorGUILayout.ObjectField("World Mesh", _worldMeshFilter, typeof(MeshFilter), true) as MeshFilter;
            if (_worldMeshFilter == null)
                return;
            if (GUILayout.Button("Extract Data From Mesh"))
                ExtractDataFromMesh();
        }

        private void ExtractDataFromMesh() 
        {
            if(_worldMeshFilter == null)
            {
                Debug.LogError("World Mesh is not assigned.");
                return;
            }

            Mesh mesh = _worldMeshFilter.sharedMesh;
            if (mesh == null)
            {
                Debug.LogError("The assigned MeshFilter does not have a mesh.");
                return;
            }

            Vector3 meshSizeXYZ = mesh.bounds.size;
            Vector2 meshSizeXZ = new Vector2(meshSizeXYZ.x, meshSizeXYZ.z);
            WorldChunkManager manager = (WorldChunkManager)target;
            manager.worldSize = meshSizeXZ;
            manager.worldOffset = _worldMeshFilter.transform.position;

            EditorUtility.SetDirty(manager);
        }
    }
}