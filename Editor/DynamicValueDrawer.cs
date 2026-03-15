using UnityEditor;
using UnityEngine;

namespace Deepwave.Core.Editor
{
    [CustomPropertyDrawer(typeof(DynamicFloat))]
    [CustomPropertyDrawer(typeof(DynamicInt))]
    public class DynamicValueDrawer : PropertyDrawer
    {
        private const float ToggleWidth = 82.5f;

        // Cache dữ liệu Attribute để tránh gọi Reflection liên tục trong OnGUI
        private bool _isInitialized;
        private DynamicRangeAttribute _rangeAttr;
        private float _minLimit = 0f;
        private float _maxLimit = 1f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (EditorGUIUtility.singleLineHeight * 2) + EditorGUIUtility.standardVerticalSpacing;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            InitializeAttributeData();

            EditorGUI.BeginProperty(position, label, property);

            var randomizeProp = property.FindPropertyRelative("randomize");
            var valueProp = property.FindPropertyRelative("value");
            var rangeProp = property.FindPropertyRelative("range");

            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;

            // Tính toán layout tĩnh
            Rect headerRect = new(position.x, position.y, position.width, lineHeight);
            Rect contentRect = new(position.x, position.y + lineHeight + spacing, position.width, lineHeight);

            Rect labelRect = new(headerRect.x, headerRect.y, headerRect.width - ToggleWidth, headerRect.height);
            Rect toggleRect = new(headerRect.xMax - ToggleWidth, headerRect.y, ToggleWidth, headerRect.height);

            // Vẽ Header
            EditorGUI.LabelField(labelRect, label, EditorStyles.boldLabel);
            randomizeProp.boolValue = EditorGUI.ToggleLeft(toggleRect, "Randomize", randomizeProp.boolValue);

            // Tính toán giới hạn Max (evaluate động vì size mảng tham chiếu có thể thay đổi lúc runtime)
            float currentMax = GetDynamicMax(property, _maxLimit);

            // Vẽ Content
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

        // Đóng gói logic vẽ single slider giúp hàm chính dễ đọc
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

        // Cache Attribute một lần duy nhất theo vòng đời của Drawer instance
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

        // Tính max limit dựa trên array size. Xử lý chuỗi được thu gọn an toàn.
        private float GetDynamicMax(SerializedProperty property, float defaultMax)
        {
            if (_rangeAttr == null || string.IsNullOrEmpty(_rangeAttr.DynamicMaxList))
                return defaultMax;

            int lastDotIndex = property.propertyPath.LastIndexOf('.');
            if (lastDotIndex == -1)
                return defaultMax;

            string parentPath = property.propertyPath[..lastDotIndex];
            string listPath = $"{parentPath}.{_rangeAttr.DynamicMaxList}";

            var listProp = property.serializedObject.FindProperty(listPath);
            if (listProp != null && listProp.isArray)
            {
                return Mathf.Max(_minLimit, listProp.arraySize - 1);
            }

            return defaultMax;
        }
    }
}