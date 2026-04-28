using UnityEditor;
using UnityEngine;

namespace Deepwave.Core.Editor
{
    [CustomPropertyDrawer(typeof(DynamicValue))]
    public sealed class DynamicValueDrawer: PropertyDrawer
    {
        private const float ToggleWidth = 70f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var isDynamicProp = property.FindPropertyRelative("_isDynamic");
            float height = EditorGUIUtility.singleLineHeight; // Header height

            // Tính chiều cao động thay vì fix cứng số dòng
            if (isDynamicProp.boolValue)
            {
                var rangeProp = property.FindPropertyRelative("_range");
                height += EditorGUIUtility.standardVerticalSpacing + EditorGUI.GetPropertyHeight(rangeProp, true);
            }
            else
            {
                height += EditorGUIUtility.standardVerticalSpacing + EditorGUIUtility.singleLineHeight;
            }

            return height;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var isDynamicProp = property.FindPropertyRelative("_isDynamic");
            var valueProp = property.FindPropertyRelative("_value");
            var rangeProp = property.FindPropertyRelative("_range");
            var isIntegerProp = property.FindPropertyRelative("_isInteger");
            var minLimitProp = property.FindPropertyRelative("_minLimit");
            var maxLimitProp = property.FindPropertyRelative("_maxLimit");

            float minLimit = minLimitProp?.floatValue ?? 0f;
            float maxLimit = maxLimitProp?.floatValue ?? 100f;
            bool isFloat = isIntegerProp == null || !isIntegerProp.boolValue;
            DynamicRangeAttribute rangeAttr = null;

            // Ghi đè giới hạn nếu có Attribute
            if (fieldInfo != null)
            {
                var attributes = fieldInfo.GetCustomAttributes(typeof(DynamicRangeAttribute), true);
                if (attributes.Length > 0 && attributes [ 0 ] is DynamicRangeAttribute attr)
                {
                    rangeAttr = attr;
                    minLimit = attr.Min;
                    maxLimit = attr.Max;
                    isFloat = !attr.IsInteger;
                }
            }

            float currentMaxLimit = PropertyUtility.GetDynamicMax(property, maxLimit, rangeAttr);

            EditorGUI.BeginProperty(position, label, property);

            // 1. Draw Header
            Rect headerRect = new(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            Rect labelRect = new(headerRect.x, headerRect.y, headerRect.width - ToggleWidth, headerRect.height);
            Rect toggleRect = new(headerRect.xMax - ToggleWidth, headerRect.y, ToggleWidth, headerRect.height);

            EditorGUI.LabelField(labelRect, label, EditorStyles.boldLabel);
            isDynamicProp.boolValue = EditorGUI.ToggleLeft(toggleRect, "Dynamic", isDynamicProp.boolValue);

            // 2. Draw Content (tự động nối tiếp ngay dưới Header)
            Rect contentRect = new(
                position.x,
                headerRect.yMax + EditorGUIUtility.standardVerticalSpacing,
                position.width,
                EditorGUIUtility.singleLineHeight
            );

            if (isDynamicProp.boolValue)
            {
                contentRect.height = EditorGUI.GetPropertyHeight(rangeProp, true);
                Vector2RangeDrawer.DrawUI(EditorGUI.IndentedRect(contentRect), rangeProp, minLimit, currentMaxLimit, isFloat);
            }
            else
            {
                Rect indentedRect = EditorGUI.IndentedRect(contentRect);
                if (isFloat)
                {
                    valueProp.floatValue = EditorGUI.Slider(indentedRect, valueProp.floatValue, minLimit, currentMaxLimit);
                }
                else
                {
                    valueProp.floatValue = EditorGUI.IntSlider(indentedRect, Mathf.RoundToInt(valueProp.floatValue), (int)minLimit, (int)currentMaxLimit);
                }
            }

            EditorGUI.EndProperty();
        }
    }
}