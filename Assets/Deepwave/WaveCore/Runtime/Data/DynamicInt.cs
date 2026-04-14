using System;
using UnityEngine;

namespace Deepwave.Core
{
    /// <summary>
    /// A data structure that allows switching between a fixed integer value and a randomized integer range.
    /// </summary>
    [Serializable]
    public struct DynamicInt
    {
        // ── Serialized Fields ─────────────────────────────────────────────
        [SerializeField] private bool _randomize;
        [SerializeField] private int _value;
        [SerializeField] private Vector2IntRange _range;

        // ── Properties ────────────────────────────────────────────────────
        public bool Randomize { get => _randomize; set => _randomize = value; }
        public int Value { get => _value; set => _value = value; }
        public Vector2IntRange Range { get => _range; set => _range = value; }

        // ── Static Methods ────────────────────────────────────────────────
        /// <summary>Initializes a default DynamicInt with a fixed value.</summary>
        public static DynamicInt Default(int val) => new()
        {
            _value = val,
            _range = new Vector2IntRange(val, val),
            _randomize = false
        };

        // ── Public API ────────────────────────────────────────────────────
        /// <summary>Returns either the fixed value or a random value within the specified integer range (exclusive max for integers).</summary>
        public readonly int Evaluate()
        {
            return _randomize ? UnityEngine.Random.Range(_range.min, _range.max) : _value;
        }
    }
}
