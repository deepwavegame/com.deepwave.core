using UnityEngine;
using System;

namespace Deepwave.Core
{
    public class DynamicRangeAttribute : PropertyAttribute
    {
        public float Min;
        public float Max;
        public string DynamicMaxList; // Tên biến List để lấy Count làm Max

        // Constructor cũ
        public DynamicRangeAttribute(float min, float max)
        {
            Min = min;
            Max = max;
            DynamicMaxList = null;
        }

        // Constructor mới hỗ trợ Dynamic Max
        public DynamicRangeAttribute(float min, string dynamicMaxList)
        {
            Min = min;
            Max = 1f; // Giá trị tạm, sẽ bị ghi đè trong Drawer
            DynamicMaxList = dynamicMaxList;
        }
    }

    // 2. Struct chứa dữ liệu cho số thực (Float)
    [Serializable]
    public struct DynamicFloat
    {
        public bool randomize;
        public float value;
        public Vector2 range; // x=min, y=max

        // Constructor tiện ích
        public static DynamicFloat Default(float val) => new()
        {
            value = val,
            range = new(val, val),
            randomize = false
        };
    }

    // 3. Struct chứa dữ liệu cho số nguyên (Int)
    [Serializable]
    public struct DynamicInt
    {
        public bool randomize;
        public int value;
        public Vector2Int range; // x=min, y=max

        public static DynamicInt Default(int val) => new()
        {
            value = val,
            range = new(val, val),
            randomize = false
        };
    }
}