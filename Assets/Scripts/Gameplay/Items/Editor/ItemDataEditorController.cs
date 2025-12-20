using Arctic.Utilities.Serialization;
using UnityEditor;
using UnityEngine;

namespace Arctic.Gameplay.Items.Editor
{
    public sealed class ItemDataEditorController
    {
        public ItemData Source { get; private set; }
        public bool HasValidSource => Source != null && !string.IsNullOrEmpty(Source.GUID);

        private readonly ISerializer<ItemData, string> serializer;
        private readonly System.Action<string> onSerialized;
        private readonly System.Action<ItemData> onDeserialized;

        public ItemDataEditorController(
            ISerializer<ItemData, string> serializer,
            System.Action<string> onSerialized,
            System.Action<ItemData> onDeserialized)
        {
            this.serializer = serializer;
            this.onSerialized = onSerialized;
            this.onDeserialized = onDeserialized;
        }

        public void SetSource(ItemData item)
        {
            Source = item;
        }

        public void Serialize()
        {
            var result = serializer.Serialize(Source);
            if (result.Status != OutputStatus.Successful)
            {
                Debug.LogError("Serialization failed");
                return;
            }

            onSerialized.Invoke(result.Object);
            Debug.Log($"__serialized (guid: {Source.GUID})__");
        }

        public void Deserialize(string text)
        {
            var result = serializer.Deserialize(text);
            if (result.Status != OutputStatus.Successful)
            {
                Debug.LogError("Deserialization failed");
                return;
            }

            Source = result.Object;
            Commit(Source);

            onDeserialized.Invoke(Source);
            Debug.Log($"__deserialized (guid: {Source.GUID})__");
        }

        private void Commit(Object asset)
        {
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
            AssetDatabase.ImportAsset(
                AssetDatabase.GetAssetPath(asset),
                ImportAssetOptions.ForceUpdate);
            AssetDatabase.Refresh();
        }
    }

}