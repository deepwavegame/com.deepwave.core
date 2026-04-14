using System;
using UnityEditor;
using UnityEngine;

namespace Deepwave.Core.Editor
{
    /// <summary>
    /// Custom PropertyDrawer for Vector2Range and Vector2IntRange.
    /// Provides a min/max slider interface for editing range values.
    /// </summary>
    [CustomPropertyDrawer(typeof(Vector2Range))]
    [CustomPropertyDrawer(typeof(Vector2IntRange))]
    public sealed class Vector2RangeDrawer : PropertyDrawer
    {
        // ── PropertyDrawer Overrides ──────────────────────────────────────
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            float minLimit = 0f;
            float maxLimit = 1f;

            if (fieldInfo != null)
            {
                var attrs = fieldInfo.GetCustomAttributes(typeof(DynamicRangeAttribute), true);
                if (attrs.Length > 0 && attrs[0] is DynamicRangeAttribute rangeAttr)
                {
                    minLimit = rangeAttr.Min;
                    maxLimit = GetDynamicMax(property, rangeAttr.Max, rangeAttr);
                }
            }

            bool isFloat = property.type == nameof(Vector2Range) || property.type == "Vector2Range";
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
                int minVal = minProp.intValue;
                int maxVal = maxProp.intValue;

                minVal = EditorGUI.IntField(minRect, minVal);

                float minFloat = minVal;
                float maxFloat = maxVal;
                EditorGUI.MinMaxSlider(sliderRect, ref minFloat, ref maxFloat, minLimit, maxLimit);

                maxVal = EditorGUI.IntField(maxRect, Mathf.RoundToInt(maxFloat));

                minVal = Mathf.Clamp(Mathf.RoundToInt(minFloat), (int)minLimit, maxVal);
                maxVal = Mathf.Clamp(maxVal, minVal, (int)maxLimit);

                minProp.intValue = minVal;
                maxProp.intValue = maxVal;
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
