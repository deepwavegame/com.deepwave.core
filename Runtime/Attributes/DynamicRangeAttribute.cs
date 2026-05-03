using UnityEngine;

namespace Deepwave.Core
{
    /// <summary>
    /// Use this attribute on a float or DynamicFloat field to visualize it with a min/max slider in the Inspector.
    /// Supports dynamic adjustment of the maximum value if a list name is provided.
    /// </summary>
    public sealed class DynamicRangeAttribute: PropertyAttribute
    {
        // ── Public Fields ─────────────────────────────────────────────────
        /// <summary>Lower bound of the range.</summary>
        public float Min;
        /// <summary>Upper bound of the range.</summary>
        public float Max;
        /// <summary>Optional property name containing a collection of values to derive a dynamic maximum from.</summary>
        public string DynamicMaxList;
        /// <summary>Whether this range represents integer values.</summary>
        public bool IsInteger;
        /// <summary>Whether the dynamic maximum should be squared (e.g., for GridSize * GridSize).</summary>
        public bool Squared;

        // ── Constructors ─────────────────────────────────────────────────
        /// <summary>Defines a fixed range with specific min and max values.</summary>
        public DynamicRangeAttribute(float min, float max)
        {
            Min = min;
            Max = max;
            DynamicMaxList = null;
            IsInteger = false;
        }

        public DynamicRangeAttribute(int min, int max)
        {
            Min = min;
            Max = max;
            DynamicMaxList = null;
            IsInteger = true;
        }

        /// <summary>Defines a range where the maximum value depends on another collection's size.</summary>
        public DynamicRangeAttribute(float min, string dynamicMaxList)
        {
            Min = min;
            Max = 1f;
            DynamicMaxList = dynamicMaxList;
            IsInteger = false;
        }

        public DynamicRangeAttribute(int min, string dynamicMaxList)
        {
            Min = min;
            Max = 1f;
            DynamicMaxList = dynamicMaxList;
            IsInteger = true;
        }
    }
}