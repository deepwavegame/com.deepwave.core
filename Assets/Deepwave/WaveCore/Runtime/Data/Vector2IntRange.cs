using System;

namespace Deepwave.Core
{
    /// <summary>
    /// Represents a range with an integer minimum and maximum value.
    /// </summary>
    [Serializable]
    public struct Vector2IntRange
    {
        // ── Public Fields ─────────────────────────────────────────────────
        /// <summary>Minimum value of the range.</summary>
        public int min;
        /// <summary>Maximum value of the range.</summary>
        public int max;

        // ── Constructor ───────────────────────────────────────────────────
        public Vector2IntRange(int min, int max)
        {
            this.min = min;
            this.max = max;
        }
    }
}
