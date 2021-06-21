using UnityEditor;
using UnityEngine;

namespace Unity.PixelText
{
    [CustomPropertyDrawer(typeof(TextProperties), true)]
    public class TextPropertiesDrawer : PropertyDrawer
    {
        private SerializedProperty _font;
        private SerializedProperty _scale;
        private SerializedProperty _horizontalAlign;
        private SerializedProperty _verticalAlign;

        private float _fontHeight;
        private float _scaleHeight;
        private float _horizontalAlignHeight;
        private float _verticalAlignHeight;

        protected void Init(SerializedProperty property)
        {
            _font = property.FindPropertyRelative("_font");
            _scale = property.FindPropertyRelative("_scale");
            _horizontalAlign = property.FindPropertyRelative("_horizontalAlign");
            _verticalAlign = property.FindPropertyRelative("_verticalAlign");
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            Init(property);

            _fontHeight = EditorGUI.GetPropertyHeight(_font);
            _scaleHeight = EditorGUI.GetPropertyHeight(_scale);
            _horizontalAlignHeight = EditorGUI.GetPropertyHeight(_horizontalAlign);
            _verticalAlignHeight = EditorGUI.GetPropertyHeight(_verticalAlign);

            return _fontHeight + _scaleHeight + _horizontalAlignHeight + _verticalAlignHeight +
                EditorGUIUtility.standardVerticalSpacing * 3f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Init(property);

            var rect = position;

            rect.height = _fontHeight;
            EditorGUI.PropertyField(rect, _font);
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;

            rect.height = _scaleHeight;
            EditorGUI.PropertyField(rect, _scale);
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;

            rect.height = _horizontalAlignHeight;
            EditorGUI.PropertyField(rect, _horizontalAlign);
            rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;

            rect.height = _verticalAlignHeight;
            EditorGUI.PropertyField(rect, _verticalAlign);
        }
    }
}
