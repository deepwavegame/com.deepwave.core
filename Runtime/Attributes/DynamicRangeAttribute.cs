using System;
using UnityEngine;

namespace Deepwave.Core
{
    public class DynamicRangeAttribute : PropertyAttribute
    {
        public float Min;
        public float Max;
        public string DynamicMaxList;

        public DynamicRangeAttribute(float min, float max)
        {
            Min = min;
            Max = max;
            DynamicMaxList = null;
        }

        public DynamicRangeAttribute(float min, string dynamicMaxList)
        {
            Min = min;
            Max = 1f;
            DynamicMaxList = dynamicMaxList;
        }
    }

    // Tách biệt cấu trúc dữ liệu giới hạn đầu cuối
    [Serializable]
    public struct Vector2Range
    {
        public float min;
        public float max;

        public Vector2Range(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
    }

    [Serializable]
    public struct Vector2IntRange
    {
        public int min;
        public int max;

        public Vector2IntRange(int min, int max)
        {
            this.min = min;
            this.max = max;
        }
    }

    [Serializable]
    public struct DynamicFloat
    {
        public bool randomize;
        public float value;
        public Vector2Range range;

        public static DynamicFloat Default(float val) => new()
        {
            value = val,
            range = new Vector2Range(val, val),
            randomize = false
        };

        public readonly float Evaluate()
        {
            return randomize ? UnityEngine.Random.Range(range.min, range.max) : value;
        }
    }

    [Serializable]
    public struct DynamicInt
    {
        public bool randomize;
        public int value;
        public Vector2IntRange range;

        public static DynamicInt Default(int val) => new()
        {
            value = val,
            range = new Vector2IntRange(val, val),
            randomize = false
        };

        public readonly float Evaluate()
        {
            return randomize ? UnityEngine.Random.Range(range.min, range.max) : value;
        }
    }
}