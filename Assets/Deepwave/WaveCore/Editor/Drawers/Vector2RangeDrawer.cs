using System;
using UnityEditor;
using UnityEngine;

namespace Deepwave.Core.Editor
{
    /// <summary>
    /// Custom PropertyDrawer for Vector2Range.
    /// Provides a min/max slider interface for editing range values.
    /// </summary>
    [CustomPropertyDrawer(typeof(Vector2Range))]
    public sealed class Vector2RangeDrawer : PropertyDrawer
    {
        // ── PropertyDrawer Overrides ──────────────────────────────────────
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            SerializedProperty isIntegerProp = property.FindPropertyRelative("_isInteger");
            SerializedProperty minLimitProp = property.FindPropertyRelative("_minLimit");
            SerializedProperty maxLimitProp = property.FindPropertyRelative("_maxLimit");

            // Base limits from the struct itself
            float minLimit = minLimitProp != null ? minLimitProp.floatValue : 0f;
            float maxLimit = maxLimitProp != null ? maxLimitProp.floatValue : 100f;
            bool isFloat = isIntegerProp == null || !isIntegerProp.boolValue;

            if (fieldInfo != null)
            {
                var attrs = fieldInfo.GetCustomAttributes(typeof(DynamicRangeAttribute), true);
                if (attrs.Length > 0 && attrs[0] is DynamicRangeAttribute rangeAttr)
                {
                    // Attribute explicitly overrides internal struct limits
                    minLimit = rangeAttr.Min;
                    maxLimit = GetDynamicMax(property, rangeAttr.Max, rangeAttr);
                    isFloat = !rangeAttr.IsInteger;
                }
            }

            // Note: DynamicValueDrawer uses this to draw the slider for the unified DynamicValue type.
            DrawUI(position, property, minLimit, maxLimit, isFloat);

            EditorGUI.EndProperty();
        }

        // ── Public API ────────────────────────────────────────────────────
        /// <summary>
        /// Shared static method to draw the range UI without requiring a drawer instance.
        /// Useful for embedding range sliders within other customized property drawers.
        /// </summary>
        public static void DrawUI(Rect rect, SerializedProperty property, float minLimit, float maxLimit, bool isFloat)
        {
            SerializedProperty minProp = property.FindPropertyRelative("min");
            SerializedProperty maxProp = property.FindPropertyRelative("max");

            float fieldWidth = 40f;
            float spacing = 5f;
            float sliderWidth = rect.width - (fieldWidth * 2) - (spacing * 2);

            Rect minRect = new(rect.x, rect.y, fieldWidth, rect.height);
            Rect sliderRect = new(minRect.xMax + spacing, rect.y, sliderWidth, rect.height);
            Rect maxRect = new(sliderRect.xMax + spacing, rect.y, fieldWidth, rect.height);

            if (isFloat)
            {
                float minVal = minProp.floatValue;
                float maxVal = maxProp.floatValue;

                minVal = EditorGUI.FloatField(minRect, (float)Math.Round(minVal, 2));
                EditorGUI.MinMaxSlider(sliderRect, ref minVal, ref maxVal, minLimit, maxLimit);
                maxVal = EditorGUI.FloatField(maxRect, (float)Math.Round(maxVal, 2));

                minProp.floatValue = minVal;
                maxProp.floatValue = maxVal;
            }
            else
            {
                int minVal = Mathf.RoundToInt(minProp.floatValue);
                int maxVal = Mathf.RoundToInt(maxProp.floatValue);

                minVal = EditorGUI.IntField(minRect, minVal);

                var (fMin, fMax) = ((float)minVal, (float)maxVal);
                EditorGUI.MinMaxSlider(sliderRect, ref fMin, ref fMax, minLimit, maxLimit);

                maxVal = EditorGUI.IntField(maxRect, Mathf.RoundToInt(fMax));
                minVal = Mathf.Clamp(Mathf.RoundToInt(fMin), (int)minLimit, maxVal);
                maxVal = Mathf.Clamp(maxVal, minVal, (int)maxLimit);

                minProp.floatValue = minVal;
                maxProp.floatValue = maxVal;
            }
        }

        // ── Private Helpers ───────────────────────────────────────────────
        private static float GetDynamicMax(SerializedProperty property, float defaultMax, DynamicRangeAttribute attr)
        {
            if (attr == null || string.IsNullOrEmpty(attr.DynamicMaxList))
                return defaultMax;

            int lastDotIndex = property.propertyPath.LastIndexOf('.');
            string parentPath = lastDotIndex == -1 ? "" : property.propertyPath[..lastDotIndex] + ".";
            string listPath = $"{parentPath}{attr.DynamicMaxList}";

            var listProp = property.serializedObject.FindProperty(listPath);
            if (listProp != null && listProp.isArray)
            {
                return Mathf.Max(attr.Min, listProp.arraySize - 1);
            }

            return defaultMax;
        }
    }
}
