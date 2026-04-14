using UnityEditor;
using UnityEngine;

namespace Deepwave.Core.Editor
{
    /// <summary>
    /// Custom PropertyDrawer for DynamicFloat and DynamicInt types.
    /// Manages the layout for switching between a single value slider and a min/max range slider.
    /// </summary>
    [CustomPropertyDrawer(typeof(DynamicFloat))]
    [CustomPropertyDrawer(typeof(DynamicInt))]
    public sealed class DynamicValueDrawer : PropertyDrawer
    {
        // ── Constants ─────────────────────────────────────────────────────
        private const float ToggleWidth = 82.5f;

        // ── Private Fields ────────────────────────────────────────────────
        private bool _isInitialized;
        private float _minLimit = 0f;
        private float _maxLimit = 1f;
        private DynamicRangeAttribute _rangeAttr;

        // ── PropertyDrawer Overrides ──────────────────────────────────────
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (EditorGUIUtility.singleLineHeight * 2) + EditorGUIUtility.standardVerticalSpacing;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            InitializeAttributeData();

            EditorGUI.BeginProperty(position, label, property);

            var randomizeProp = property.FindPropertyRelative("_randomize");
            var valueProp = property.FindPropertyRelative("_value");
            var rangeProp = property.FindPropertyRelative("_range");

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
            float currentMax = GetDynamicMax(property, _maxLimit);

            // Draw Content
            Rect indentedContentRect = EditorGUI.IndentedRect(contentRect);
            bool isFloat = valueProp.propertyType == SerializedPropertyType.Float;

            if (randomizeProp.boolValue)
            {
                Vector2RangeDrawer.DrawUI(indentedContentRect, rangeProp, _minLimit, currentMax, isFloat);
            }
            else
            {
                DrawSingleSlider(indentedContentRect, valueProp, _minLimit, currentMax, isFloat);
            }

            EditorGUI.EndProperty();
        }

        // ── Private Helpers ───────────────────────────────────────────────
        private void DrawSingleSlider(Rect rect, SerializedProperty valueProp, float min, float max, bool isFloat)
        {
            if (isFloat)
            {
                valueProp.floatValue = EditorGUI.Slider(rect, valueProp.floatValue, min, max);
            }
            else
            {
                valueProp.intValue = EditorGUI.IntSlider(rect, valueProp.intValue, (int)min, (int)max);
            }
        }

        private void InitializeAttributeData()
        {
            if (_isInitialized) return;
            _isInitialized = true;

            if (fieldInfo == null) return;

            var attributes = fieldInfo.GetCustomAttributes(typeof(DynamicRangeAttribute), true);
            if (attributes.Length > 0 && attributes[0] is DynamicRangeAttribute attr)
            {
                _rangeAttr = attr;
                _minLimit = attr.Min;
                _maxLimit = attr.Max;
            }
        }

        private float GetDynamicMax(SerializedProperty property, float defaultMax)
        {
            if (_rangeAttr == null || string.IsNullOrEmpty(_rangeAttr.DynamicMaxList))
                return defaultMax;

            int lastDotIndex = property.propertyPath.LastIndexOf('.');
            string parentPath = lastDotIndex == -1 ? "" : property.propertyPath[..lastDotIndex] + ".";
            string listPath = $"{parentPath}{_rangeAttr.DynamicMaxList}";

            var listProp = property.serializedObject.FindProperty(listPath);
            if (listProp != null && listProp.isArray)
            {
                return Mathf.Max(_minLimit, listProp.arraySize - 1);
            }

            return defaultMax;
        }
    }
}
