using UnityEditor;
using UnityEditor.UI;

namespace Unity.PixelText
{
    [CustomEditor(typeof(PixelText))]
    [CanEditMultipleObjects]
    public class PixelTextEditor : GraphicEditor
    {
        SerializedProperty _text;
        SerializedProperty _props;

        protected override void OnEnable()
        {
            base.OnEnable();
            _text = serializedObject.FindProperty("_text");
            _props = serializedObject.FindProperty("_props");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_text);
            EditorGUILayout.PropertyField(_props);
            AppearanceControlsGUI();
            RaycastControlsGUI();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
