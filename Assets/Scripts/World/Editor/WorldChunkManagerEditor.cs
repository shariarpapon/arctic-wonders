using UnityEngine;
using UnityEditor;
using Arctic.World;
using UnityEngine.Rendering;

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

            DrawChunkCullingVariables();
            DrawHelperTools();
        }

        private void DrawChunkCullingVariables() 
        {
            if (!_manager.createInstances)
                return;

            _manager.enableChunkCulling = EditorGUILayout.Toggle("Enable Chunk Culling", _manager.enableChunkCulling);
            if (!_manager.enableChunkCulling)
                return;

            _manager.viewer = EditorGUILayout.ObjectField("Viewer", _manager.viewer, typeof(GameObject), true) as GameObject;
        }

        private void DrawHelperTools() 
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Helper Tools", EditorStyles.boldLabel);
            _worldMeshFilter = EditorGUILayout.ObjectField("World Terrain Mesh", _worldMeshFilter, typeof(MeshFilter), true) as MeshFilter;
            if (_worldMeshFilter)
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