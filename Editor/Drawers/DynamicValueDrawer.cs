using UnityEditor;
using UnityEngine;

namespace Deepwave.Core.Editor
{
    /// <summary>
    /// Custom PropertyDrawer for DynamicValue type.
    /// Manages the layout for switching between a single value slider and a min/max range slider.
    /// </summary>
    [CustomPropertyDrawer(typeof(DynamicValue))]
    public sealed class DynamicValueDrawer : PropertyDrawer
    {
        // ── Constants ─────────────────────────────────────────────────────
        private const float ToggleWidth = 82.5f;

        // ── PropertyDrawer Overrides ──────────────────────────────────────
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (EditorGUIUtility.singleLineHeight * 2) + EditorGUIUtility.standardVerticalSpacing;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var randomizeProp = property.FindPropertyRelative("_randomize");
            var valueProp = property.FindPropertyRelative("_value");
            var rangeProp = property.FindPropertyRelative("_range");
            var isIntegerProp = property.FindPropertyRelative("_isInteger");
            var minLimitProp = property.FindPropertyRelative("_minLimit");
            var maxLimitProp = property.FindPropertyRelative("_maxLimit");

            // Base limits from the struct itself
            float minLimit = minLimitProp != null ? minLimitProp.floatValue : 0f;
            float maxLimit = maxLimitProp != null ? maxLimitProp.floatValue : 100f;
            DynamicRangeAttribute rangeAttr = null;

            if (fieldInfo != null)
            {
                var attributes = fieldInfo.GetCustomAttributes(typeof(DynamicRangeAttribute), true);
                if (attributes.Length > 0 && attributes[0] is DynamicRangeAttribute attr)
                {
                    rangeAttr = attr;
                    // Attribute explicitly overrides internal struct limits
                    minLimit = attr.Min;
                    maxLimit = attr.Max;
                }
            }

            EditorGUI.BeginProperty(position, label, property);

            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;

            // Layout calculation
            Rect headerRect = new(position.x, position.y, position.width, lineHeight);
            Rect contentRect = new(position.x, position.y + lineHeight + spacing, position.width, lineHeight);

            Rect labelRect = new(headerRect.x, headerRect.y, headerRect.width - ToggleWidth, headerRect.height);
            Rect toggleRect = new(headerRect.xMax - ToggleWidth, headerRect.y, ToggleWidth, headerRect.height);

            // Draw Header
            EditorGUI.LabelField(labelRect, label, EditorStyles.boldLabel);
            randomizeProp.boolValue = EditorGUI.ToggleLeft(toggleRect, "Randomize", randomizeProp.boolValue);

            // Dynamic Max Limit evaluation
            float currentMaxLimit = GetDynamicMax(property, maxLimit, minLimit, rangeAttr);

            // Draw Content
            Rect indentedContentRect = EditorGUI.IndentedRect(contentRect);

            bool isFloat = isIntegerProp == null || !isIntegerProp.boolValue;
            if (rangeAttr != null)
            {
                isFloat = !rangeAttr.IsInteger;
            }

            if (randomizeProp.boolValue)
            {
                Vector2RangeDrawer.DrawUI(indentedContentRect, rangeProp, minLimit, currentMaxLimit, isFloat);
            }
            else
            {
                if (isFloat)
                {
                    valueProp.floatValue = EditorGUI.Slider(indentedContentRect, valueProp.floatValue, minLimit, currentMaxLimit);
                }
                else
                {
                    valueProp.floatValue = EditorGUI.IntSlider(indentedContentRect, Mathf.RoundToInt(valueProp.floatValue), (int)minLimit, (int)currentMaxLimit);
                }
            }

            EditorGUI.EndProperty();
        }

        // ── Private Helpers ───────────────────────────────────────────────
        private static float GetDynamicMax(SerializedProperty property, float defaultMax, float minLimit, DynamicRangeAttribute attr)
        {
            if (attr == null || string.IsNullOrEmpty(attr.DynamicMaxList))
                return defaultMax;

            int lastDotIndex = property.propertyPath.LastIndexOf('.');
            string parentPath = lastDotIndex == -1 ? "" : property.propertyPath[..lastDotIndex] + ".";
            string listPath = $"{parentPath}{attr.DynamicMaxList}";

            var listProp = property.serializedObject.FindProperty(listPath);
            if (listProp != null && listProp.isArray)
            {
                return Mathf.Max(minLimit, listProp.arraySize - 1);
            }

            return defaultMax;
        }
    }
}
