using System;

namespace Deepwave.Core
{
    /// <summary>
    /// Represents a range with a floating-point minimum and maximum value.
    /// </summary>
    [Serializable]
    public struct Vector2Range
    {
        // ── Public Fields ─────────────────────────────────────────────────
        /// <summary>Minimum value of the range.</summary>
        public float min;
        /// <summary>Maximum value of the range.</summary>
        public float max;

        // ── Constructor ───────────────────────────────────────────────────
        public Vector2Range(float min, float max)
        {
            this.min = min;
            this.max = max;
        }

        //public Vector2Range(int min, int max)
        //{
        //}
    }
}
