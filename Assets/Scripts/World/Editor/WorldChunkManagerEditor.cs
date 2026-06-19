using UnityEngine;
using UnityEditor;
using Arctic.World;

namespace Arctic.Woirld.CustomEditors
{
    [CustomEditor(typeof(WorldChunkManager))]
    public class WorldChunkManagerEditor : Editor
    {
        private MeshFilter _worldMeshFilter;
        private WorldChunkManager _manager;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            _manager = (WorldChunkManager)target;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Helper Tools", EditorStyles.boldLabel);
            _worldMeshFilter = EditorGUILayout.ObjectField("World Terrain Mesh", _worldMeshFilter, typeof(MeshFilter), true) as MeshFilter;
            if (_worldMeshFilter == null)
                return;
            OnMeshFilterExists();
        }

        private void OnMeshFilterExists() 
        {
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
            _manager.worldSize = meshSizeXZ;
            _manager.worldCenter = _worldMeshFilter.transform.position;

            EditorUtility.SetDirty(_manager);
        }
    }
}