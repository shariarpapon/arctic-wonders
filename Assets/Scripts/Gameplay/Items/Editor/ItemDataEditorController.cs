using Arctic.Utilities.Editor;
using Arctic.Utilities.Serialization;
using UnityEngine;

namespace Arctic.Gameplay.Items.Editor
{
    public sealed class ItemDataEditorController
    {
        public ItemData Source { get; private set; }
        public bool HasValidSource => Source != null && !string.IsNullOrEmpty(Source.GUID);

        private readonly ItemDataSerializer itemDataSerializer;
        private readonly System.Action<string> onSerialized;
        private readonly System.Action<ItemData> onDeserialized;

        public string AutoCreationPath { get; private set; } = "Assets";

        public ItemDataEditorController(
            ItemDataSerializer itemDataSerializer,
            System.Action<string> onSerialized,
            System.Action<ItemData> onDeserialized)
        {
            this.itemDataSerializer = itemDataSerializer;
            this.onSerialized = onSerialized;
            this.onDeserialized = onDeserialized;
        }

        public void SetAutoCreationPath(string path)
        {
            AutoCreationPath = path;
        }

        public void SetSource(ItemData item)
        {
            Source = item;
        }

        public void Serialize()
        {
            var result = itemDataSerializer.Serialize(Source);
            if (result.Status != OutputStatus.Successful)
            {
                Debug.LogError("Serialization failed");
                return;
            }

            onSerialized.Invoke(result.Object);
            Debug.Log($"<color=orange>__serialized (guid: {Source.GUID})__</color>");
        }

        public void Deserialize(string text, bool createIfNotFound)
        {
            var details = itemDataSerializer.DeserializeDetailed(text);
            switch (details.status)
            {
                case ItemDataSerializer.DeserializeStatus.Successful:
                    OnDeserializeSuccess(details.deserializedItemData);
                    break;
                case ItemDataSerializer.DeserializeStatus.CouldNotFindDeserializeTarget:
                    if (!createIfNotFound)
                    {
                        Debug.LogError("Could not find deserialize target with guid: " + details.guid);
                        return;
                    }
                    CreateFromDeserializedData(details);
                    break;
                default:
                    Debug.LogError($"Deserialization failed: {details.message}");
                    return;
            }
            Source = details.deserializedItemData;
            Debug.Log($"<color=orange>__deserialization complete (guid: {Source?.GUID})__</color>");
        }

        private void CreateFromDeserializedData(ItemDataSerializer.DeserializeDetails details)
        {
            Debug.Log("Creating new ItemData asset from deserialized data...");

            string path = AutoCreationPath;
            if (string.IsNullOrEmpty(AutoCreationPath))
            {
                path = "Assets/";
                Debug.LogWarning("Invalid auto creation path. ItemData asset will be created in the Assets folder.");
            }

            ItemData newItemData = ScriptableObject.CreateInstance<ItemData>();
            newItemData.SetData(details.guid, details.deserializedProperties);
            string assetName = newItemData.GUID;
            const string DISPLAY_NAME_PROP_KEY = "display_name";
            if (newItemData.TryGetPropertyValue(DISPLAY_NAME_PROP_KEY, out string displayName))
                assetName = displayName;
            try
            {
                Helper.CreateAssetAtPath(newItemData, $"{path}/{assetName}.asset");
                OnDeserializeSuccess(newItemData);
                Debug.Log($"<color=green>Successfully created new ItemData asset at path: {path}/{assetName}.asset</color>");
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
            onDeserialized.Invoke(newItemData);
        }

        private void OnDeserializeSuccess(ItemData itemData)
        {
            Source = itemData;
            Helper.CommitAssetChanges(Source);
            onDeserialized.Invoke(Source);
        }
    }

}
