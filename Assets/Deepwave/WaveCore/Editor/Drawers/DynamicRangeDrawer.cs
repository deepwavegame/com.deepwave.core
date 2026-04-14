using UnityEditor;
using UnityEngine;

namespace Deepwave.Core.Editor
{
    /// <summary>
    /// Custom PropertyDrawer for fields marked with <see cref="DynamicRangeAttribute"/>.
    /// Provides a slider interface with support for dynamic maximum limits based on other collection properties.
    /// </summary>
    [CustomPropertyDrawer(typeof(DynamicRangeAttribute))]
    public sealed class DynamicRangeDrawer : PropertyDrawer
    {
        // ── PropertyDrawer Overrides ──────────────────────────────────────
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.Float || property.propertyType == SerializedPropertyType.Integer)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (attribute is not DynamicRangeAttribute rangeAttr)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            float minLimit = rangeAttr.Min;
            float maxLimit = GetDynamicMax(property, rangeAttr.Max, rangeAttr);

            switch (property.propertyType)
            {
                case SerializedPropertyType.Float:
                    property.floatValue = EditorGUI.Slider(position, label, property.floatValue, minLimit, maxLimit);
                    break;
                case SerializedPropertyType.Integer:
                    property.intValue = EditorGUI.IntSlider(position, label, property.intValue, Mathf.RoundToInt(minLimit), Mathf.RoundToInt(maxLimit));
                    break;
                default:
                    EditorGUI.PropertyField(position, property, label, true);
                    break;
            }
        }

        // ── Private Helpers ───────────────────────────────────────────────
        private float GetDynamicMax(SerializedProperty property, float defaultMax, DynamicRangeAttribute attr)
        {
            if (attr == null || string.IsNullOrEmpty(attr.DynamicMaxList))
                return defaultMax;

            int lastDotIndex = property.propertyPath.LastIndexOf('.');
            string parentPath = lastDotIndex == -1 ? "" : property.propertyPath[..lastDotIndex] + ".";
            string listPath = $"{parentPath}{attr.DynamicMaxList}";

            var listProp = property.serializedObject.FindProperty(listPath);
            if (listProp != null && listProp.isArray)
            {
                // Subtract 1 to treat as index count (standard for core logic) or use arraySize as is?
                // The current DynamicValueDrawer uses arraySize - 1.
                return Mathf.Max(attr.Min, listProp.arraySize - 1);
            }

            return defaultMax;
        }
    }
}
