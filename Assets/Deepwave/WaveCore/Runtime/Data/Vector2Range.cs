using System;
using UnityEngine;

namespace Deepwave.Core
{
    /// <summary>
    /// Represents a range with a floating-point minimum and maximum value.
    /// </summary>
    [Serializable]
    public struct Vector2Range
    {
        // ── Serialized Fields ─────────────────────────────────────────────
        /// <summary>Minimum value of the range.</summary>
        public float min;

        /// <summary>Maximum value of the range.</summary>
        public float max;

        [SerializeField] private float _minLimit;
        [SerializeField] private float _maxLimit;
        [SerializeField] private bool _isInteger;

        // ── Constructors ──────────────────────────────────────────────────
        public Vector2Range(float min, float max)
        {
            this.min = min;
            this.max = max;
            _minLimit = min;
            _maxLimit = max;
            _isInteger = false;
        }

        public Vector2Range(int min, int max)
        {
            this.min = min;
            this.max = max;
            _minLimit = min;
            _maxLimit = max;
            _isInteger = true;
        }
    }
}
