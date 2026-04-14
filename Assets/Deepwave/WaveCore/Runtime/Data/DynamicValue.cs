using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Deepwave.Core
{
    /// <summary>
    /// A data structure that allows switching between a fixed value and a randomized range.
    /// This unified type handles both float and integer use cases (via float internally).
    /// </summary>
    [Serializable]
    public struct DynamicValue
    {
        // ── Serialized Fields ─────────────────────────────────────────────
        [SerializeField] private float _value;
        [SerializeField] private bool _randomize;
        [SerializeField] private Vector2Range _range;

        [SerializeField] private bool _isInteger;
        [SerializeField] private float _minLimit;
        [SerializeField] private float _maxLimit;

        // ── Properties ────────────────────────────────────────────────────
        public bool Randomize { readonly get => _randomize; set => _randomize = value; }
        public float Value { readonly get => _value; set => _value = value; }
        public Vector2Range Range { readonly get => _range; set => _range = value; }

        // ── Static Methods ────────────────────────────────────────────────
        /// <summary>Initializes a default DynamicValue with a fixed float value.</summary>
        public static DynamicValue Default(float val) => new()
        {
            _value = val,
            _range = new Vector2Range(val, val),
            _randomize = false,
            _isInteger = false,
        };

        /// <summary>Initializes a default DynamicValue with a fixed integer value.</summary>
        public static DynamicValue Default(int val, int minLimit = 0, int maxLimit = 100) => new()
        {
            _value = (float)val,
            _range = new Vector2Range(val, val),
            _randomize = false,
            _isInteger = true,
        };

        // ── Public API ────────────────────────────────────────────────────
        /// <summary>Returns either the fixed value or a random value within the specified range.</summary>
        public readonly float Evaluate()
        {
            if (!_randomize) return _value;
            return _isInteger
                ? Random.Range((int)_range.min, (int)_range.max)
                : Random.Range(_range.min, _range.max);
        }
    }
}
