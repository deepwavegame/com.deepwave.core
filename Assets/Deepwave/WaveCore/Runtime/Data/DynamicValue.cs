using System;
using UnityEngine;

namespace Deepwave.Core
{
    /// <summary>
    /// A data structure that allows switching between a fixed value or a dynamic evaluation (Curve * Random Range).
    /// This unified type handles both float and integer use cases.
    /// </summary>
    [Serializable]
    public struct DynamicValue
    {
        // ── Serialized Fields ─────────────────────────────────────────────
        [SerializeField]
        private bool _isDynamic;

        [SerializeField]
        private float _value;

        [SerializeField]
        private Vector2Range _range;

        [SerializeField]
        private bool _isInteger;

        [SerializeField]
        private float _minLimit;

        [SerializeField]
        private float _maxLimit;

        // ── Properties ────────────────────────────────────────────────────
        public bool IsDynamic
        {
            readonly get => _isDynamic;
            set => _isDynamic = value;
        }
        public float Value
        {
            readonly get => _value;
            set => _value = value;
        }
        public Vector2Range Range
        {
            readonly get => _range;
            set => _range = value;
        }

        // ── Static Methods ────────────────────────────────────────────────
        /// <summary>Initializes a default DynamicValue with a fixed float value.</summary>
        public static DynamicValue Default(float val) =>
            new()
            {
                _isDynamic = false,
                _value = val,
                _range = new Vector2Range(val, val),
                _isInteger = false,
            };

        /// <summary>Initializes a default DynamicValue with a fixed integer value.</summary>
        public static DynamicValue Default(int val, int minLimit = 0, int maxLimit = 100) =>
            new()
            {
                _isDynamic = false,
                _value = val,
                _range = new Vector2Range(val, val, minLimit, maxLimit),
                _isInteger = true,
            };

        // ── Public API ────────────────────────────────────────────────────
        /// <summary>Returns the value based on the dynamic state and evaluation time.</summary>
        public readonly float Evaluate(float t = 0f)
        {
            if (!_isDynamic)
                return _value;

            return _isInteger
                ? Mathf.Lerp((int)_range.min, (int)_range.max, t)
                : Mathf.Lerp(_range.min, _range.max, t);
        }
    }
}
