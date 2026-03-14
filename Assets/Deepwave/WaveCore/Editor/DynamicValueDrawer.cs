using UnityEditor;
using UnityEngine;

namespace Deepwave.Core.Editor
{
    [CustomPropertyDrawer(typeof(DynamicFloat))]
    [CustomPropertyDrawer(typeof(DynamicInt))]
    public class DynamicValueDrawer : PropertyDrawer
    {
        private const float HeaderSpacing = 2f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 2 + HeaderSpacing;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty randomizeProp = property.FindPropertyRelative("randomize");
            SerializedProperty valueProp = property.FindPropertyRelative("value");
            SerializedProperty rangeProp = property.FindPropertyRelative("range");

            float minLimit = 0f;
            float maxLimit = 1f;

            // Tính toán giới hạn dựa trên Attribute
            if (fieldInfo != null)
            {
                var attrs = fieldInfo.GetCustomAttributes(typeof(DynamicRangeAttribute), true);
                if (attrs.Length > 0 && attrs[0] is DynamicRangeAttribute rangeAttr)
                {
                    minLimit = rangeAttr.Min;
                    maxLimit = rangeAttr.Max;

                    // Lấy số lượng phần tử của list lân cận làm giá trị Max
                    if (!string.IsNullOrEmpty(rangeAttr.DynamicMaxList))
                    {
                        string propertyPath = property.propertyPath;
                        int lastDot = propertyPath.LastIndexOf('.');

                        if (lastDot != -1)
                        {
                            string parentPath = propertyPath[..lastDot];
                            string listPath = parentPath + "." + rangeAttr.DynamicMaxList;
                            SerializedProperty listProp = property.serializedObject.FindProperty(listPath);

                            if (listProp != null && listProp.isArray)
                            {
                                int count = listProp.arraySize;
                                maxLimit = Mathf.Max(minLimit, count - 1);
                            }
                        }
                    }
                }
            }

            bool isFloat = valueProp.propertyType == SerializedPropertyType.Float;
            float lineHeight = EditorGUIUtility.singleLineHeight;

            Rect headerRect = new(position.x, position.y, position.width, lineHeight);
            Rect sliderRect = EditorGUI.IndentedRect(new Rect(position.x, position.y + lineHeight + HeaderSpacing, position.width, lineHeight));

            EditorGUI.LabelField(headerRect, label, EditorStyles.boldLabel);

            float toggleWidth = 100f;
            Rect toggleRect = new(headerRect.xMax - toggleWidth, headerRect.y, toggleWidth, lineHeight);
            randomizeProp.boolValue = EditorGUI.ToggleLeft(toggleRect, "Randomize", randomizeProp.boolValue);

            // Phân luồng hiển thị giao diện tùy theo trạng thái Random
            if (randomizeProp.boolValue)
            {
                // Tái sử dụng logic vẽ đã được tách biệt ở Vector2RangeDrawer
                Vector2RangeDrawer.DrawUI(sliderRect, rangeProp, minLimit, maxLimit, isFloat);
            }
            else
            {
                if (isFloat)
                {
                    valueProp.floatValue = EditorGUI.Slider(sliderRect, valueProp.floatValue, minLimit, maxLimit);
                }
                else
                {
                    valueProp.intValue = EditorGUI.IntSlider(sliderRect, valueProp.intValue, (int)minLimit, (int)maxLimit);
                }
            }

            EditorGUI.EndProperty();
        }
    }
}