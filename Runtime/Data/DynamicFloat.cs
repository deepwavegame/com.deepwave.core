using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Deepwave.Core
{
    /// <summary>
    /// A data structure that allows switching between a fixed value and a randomized range.
    /// </summary>
    [Serializable]
    public struct DynamicFloat
    {
        // ── Serialized Fields ─────────────────────────────────────────────
        [SerializeField] private bool _randomize;
        [SerializeField] private float _value;
        [SerializeField] private Vector2Range _range;

        // ── Properties ────────────────────────────────────────────────────
        public bool Randomize { readonly get => _randomize; set => _randomize = value; }
        public float Value { readonly get => _value; set => _value = value; }
        public Vector2Range Range { readonly get => _range; set => _range = value; }

        // ── Static Methods ────────────────────────────────────────────────
        /// <summary>Initializes a default DynamicFloat with a fixed value.</summary>
        public static DynamicFloat Default(float val) => new()
        {
            _value = val,
            _range = new Vector2Range(val, val),
            _randomize = false
        };

        // ── Public API ────────────────────────────────────────────────────
        /// <summary>Returns either the fixed value or a random value within the specified range.</summary>
        public readonly float Evaluate()
        {
            return _randomize ? Random.Range(_range.min, _range.max) : _value;
        }
    }
}
