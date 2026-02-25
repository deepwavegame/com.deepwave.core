using UnityEditor;
using UnityEngine;

namespace Deepwave.Core.Editor
{
    [CustomPropertyDrawer(typeof(Vector2RangeAttribute))]
    public class Vector2RangeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Lấy Vector2RangeAttribute
            Vector2RangeAttribute range = attribute as Vector2RangeAttribute;

            // Lấy giá trị Vector2 từ VolumeParameter
            SerializedProperty valueProp = property.FindPropertyRelative("m_Value");
            if (valueProp == null)
            {
                Debug.LogError("Vector2RangeDrawer: Could not find m_Value in Vector2Parameter");
                return;
            }

            Vector2 currentValue = valueProp.vector2Value;

            float labelWidth = EditorGUIUtility.labelWidth;
            float fieldHeight = EditorGUIUtility.singleLineHeight;
            float spacing = 2f;

            // Vẽ nhãn
            Rect labelRect = new Rect(position.x, position.y, labelWidth, fieldHeight);
            EditorGUI.LabelField(labelRect, label);

            // Vẽ slider cho X
            Rect xLabelRect = new Rect(position.x + labelWidth, position.y, 20, fieldHeight);
            EditorGUI.LabelField(xLabelRect, "X");
            Rect xSliderRect = new Rect(position.x + labelWidth + 20, position.y, position.width - labelWidth - 20, fieldHeight);
            currentValue.x = EditorGUI.Slider(xSliderRect, currentValue.x, range.min, range.max);

            // Vẽ slider cho Y
            Rect yLabelRect = new Rect(position.x + labelWidth, position.y + fieldHeight + spacing, 20, fieldHeight);
            EditorGUI.LabelField(yLabelRect, "Y");
            Rect ySliderRect = new Rect(position.x + labelWidth + 20, position.y + fieldHeight + spacing, position.width - labelWidth - 20, fieldHeight);
            currentValue.y = EditorGUI.Slider(ySliderRect, currentValue.y, range.min, range.max);

            // Cập nhật giá trị
            valueProp.vector2Value = currentValue;

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2 + 2; // 2 dòng cho X và Y, cộng khoảng cách
        }
    }
}