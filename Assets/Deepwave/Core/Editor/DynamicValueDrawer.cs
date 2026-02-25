using System;
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

            DynamicRangeAttribute rangeAttr = null;

            // 1. Lấy Attribute bằng fieldInfo
            if (fieldInfo != null)
            {
                var attrs = fieldInfo.GetCustomAttributes(typeof(DynamicRangeAttribute), true);
                if (attrs.Length > 0)
                {
                    rangeAttr = attrs[0] as DynamicRangeAttribute;
                }
            }

            if (rangeAttr != null)
            {
                minLimit = rangeAttr.Min;
                maxLimit = rangeAttr.Max;

                // --- LOGIC MỚI: XỬ LÝ DYNAMIC LIST ---
                if (!string.IsNullOrEmpty(rangeAttr.DynamicMaxList))
                {
                    // Tìm property của List nằm cùng cấp với biến SmartFloat này
                    // Cách làm: Lấy đường dẫn cha của property hiện tại, nối thêm tên List
                    string propertyPath = property.propertyPath; // Ví dụ: TextureMapping.panelColorIndex
                    int lastDot = propertyPath.LastIndexOf('.');

                    if (lastDot != -1)
                    {
                        string parentPath = propertyPath[..lastDot]; // TextureMapping
                        string listPath = parentPath + "." + rangeAttr.DynamicMaxList; // TextureMapping.panelColors

                        SerializedProperty listProp = property.serializedObject.FindProperty(listPath);

                        if (listProp != null && listProp.isArray)
                        {
                            // Max Limit = Số lượng phần tử - 1 (vì index bắt đầu từ 0)
                            // Hoặc lấy count tùy logic bạn muốn (ở đây mình lấy Count - 1 để khớp index)
                            int count = listProp.arraySize;
                            maxLimit = Mathf.Max(minLimit, count - 1);
                        }
                    }
                }
                // -------------------------------------
            }

            bool isFloat = valueProp.propertyType == SerializedPropertyType.Float;

            float lineHeight = EditorGUIUtility.singleLineHeight;
            Rect headerRect = new(position.x, position.y, position.width, lineHeight);
            Rect sliderRect = new(position.x, position.y + lineHeight + HeaderSpacing, position.width, lineHeight);

            EditorGUI.LabelField(headerRect, label, EditorStyles.boldLabel);

            float toggleWidth = 100f;
            Rect toggleRect = new(headerRect.xMax - toggleWidth, headerRect.y, toggleWidth, lineHeight);

            randomizeProp.boolValue = EditorGUI.ToggleLeft(toggleRect, "Randomize", randomizeProp.boolValue);

            sliderRect = EditorGUI.IndentedRect(sliderRect);

            if (randomizeProp.boolValue)
            {
                if (isFloat)
                {
                    Vector2 rangeVal = rangeProp.vector2Value;
                    float rMin = rangeVal.x;
                    float rMax = rangeVal.y;
                    DrawMinMaxSlider(sliderRect, ref rMin, ref rMax, minLimit, maxLimit, true);
                    rangeProp.vector2Value = new Vector2(rMin, rMax);
                }
                else
                {
                    Vector2Int rangeVal = rangeProp.vector2IntValue;
                    float rMin = rangeVal.x;
                    float rMax = rangeVal.y;
                    DrawMinMaxSlider(sliderRect, ref rMin, ref rMax, minLimit, maxLimit, false);
                    rangeProp.vector2IntValue = new Vector2Int(Mathf.RoundToInt(rMin), Mathf.RoundToInt(rMax));
                }
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

        private void DrawMinMaxSlider(Rect rect, ref float minVal, ref float maxVal, float minLimit, float maxLimit, bool isFloat)
        {
            float fieldWidth = 40f;
            float spacing = 5f;
            float sliderWidth = rect.width - (fieldWidth * 2) - (spacing * 2);

            Rect minRect = new(rect.x, rect.y, fieldWidth, rect.height);
            Rect sliderRect = new(minRect.xMax + spacing, rect.y, sliderWidth, rect.height);
            Rect maxRect = new(sliderRect.xMax + spacing, rect.y, fieldWidth, rect.height);

            if (isFloat)
            {
                minVal = EditorGUI.FloatField(minRect, (float)Math.Round(minVal, 2));
                EditorGUI.MinMaxSlider(sliderRect, ref minVal, ref maxVal, minLimit, maxLimit);
                maxVal = EditorGUI.FloatField(maxRect, (float)Math.Round(maxVal, 2));
                return;
            }

            int minInt = Mathf.Clamp(Mathf.RoundToInt(minVal), (int)minLimit, (int)maxLimit);
            int maxInt = Mathf.Clamp(Mathf.RoundToInt(maxVal), (int)minLimit, (int)maxLimit);

            minInt = EditorGUI.IntField(minRect, minInt);

            float minFloat = minInt;
            float maxFloat = maxInt;
            EditorGUI.MinMaxSlider(sliderRect, ref minFloat, ref maxFloat, minLimit, maxLimit);

            maxInt = EditorGUI.IntField(maxRect, Mathf.RoundToInt(maxFloat));

            minInt = Mathf.Clamp(Mathf.RoundToInt(minFloat), (int)minLimit, (int)maxLimit);
            maxInt = Mathf.Clamp(maxInt, (int)minLimit, (int)maxLimit);

            if (minInt > maxInt)
            {
                (minInt, maxInt) = (maxInt, minInt);
            }

            minVal = minInt;
            maxVal = maxInt;
        }
    }
}
