using UnityEditor;

namespace Arctic.Gameplay.Items.Editor
{
    [CustomEditor(typeof(ItemDefinition))]
    public class ItemDefinitionEditor : UnityEditor.Editor
    {
        private ItemDefinition _target;

        private void OnEnable()
        {
            _target = (ItemDefinition)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}