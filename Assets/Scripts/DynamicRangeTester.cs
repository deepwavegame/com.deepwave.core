using System.Collections.Generic;
using UnityEngine;

namespace Deepwave.Core.Tests
{
    public class DynamicRangeTester : MonoBehaviour
    {
        [Header("Raw Range Drawers")]
        public Vector2Range genericFloatRange = new(0f, 10f);
        public Vector2IntRange genericIntRange = new(0, 5);

        [Header("Static Range Attributes")]
        [DynamicRange(-5f, 5f)]
        public DynamicFloat staticFloatRange;

        [DynamicRange(1f, 10f)]
        public DynamicInt staticIntRange;


        [Header("Dynamic List Limits")]
        public List<string> referenceList = new() { "Item 1", "Item 2", "Item 3" };

        [DynamicRange(0f, nameof(referenceList))]
        public DynamicInt dynamicListLimit;
    }
}