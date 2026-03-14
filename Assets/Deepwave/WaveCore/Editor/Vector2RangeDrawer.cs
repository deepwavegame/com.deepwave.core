using System;
using UnityEditor;
using UnityEngine;

namespace Deepwave.Core.Editor
{
    [CustomPropertyDrawer(typeof(Vector2Range))]
    [CustomPropertyDrawer(typeof(Vector2IntRange))]
    public class Vector2RangeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Vẽ nhãn prefix và lấy vùng không gian còn lại cho slider
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            float minLimit = 0f;
            float maxLimit = 1f;

            // Đọc giới hạn mặc định nếu Vector2Range được sử dụng độc lập với Attribute
            if (fieldInfo != null)
            {
                var attrs = fieldInfo.GetCustomAttributes(typeof(DynamicRangeAttribute), true);
                if (attrs.Length > 0 && attrs[0] is DynamicRangeAttribute rangeAttr)
                {
                    minLimit = rangeAttr.Min;
                    maxLimit = rangeAttr.Max;
                }
            }

            bool isFloat = property.type == nameof(Vector2Range);
            DrawUI(position, property, minLimit, maxLimit, isFloat);

            EditorGUI.EndProperty();
        }

        // Tách hàm tĩnh để chia sẻ logic vẽ giao diện mà không cần khởi tạo Drawer
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

                // Khóa giá trị int không vượt giới hạn và không lặp chéo
                minVal = Mathf.Clamp(Mathf.RoundToInt(minFloat), (int)minLimit, maxVal);
                maxVal = Mathf.Clamp(maxVal, minVal, (int)maxLimit);

                minProp.intValue = minVal;
                maxProp.intValue = maxVal;
            }
        }
    }
}