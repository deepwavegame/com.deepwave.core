using System.Collections.Generic;
using UnityEngine;

namespace Deepwave.Core.Tests
{
    public class DynamicRangeTester : MonoBehaviour
    {
        [Header("Raw Range Drawers")]
        [DynamicRange(0f, 10f)]
        public Vector2Range genericFloatRange = new(0f, 10f);

        [DynamicRange(0, 10)]
        public Vector2Range genericIntRange = new(0, 5);

        [Header("Static Range Attributes")]
        [DynamicRange(-5f, 5f)]
        public DynamicValue staticFloatRange;

        [DynamicRange(1, 10)]
        public DynamicValue staticIntRange;

        [Header("Dynamic List Limits")]
        public List<string> referenceList = new() { "Item 1", "Item 2", "Item 3" };

        [DynamicRange(0f, nameof(referenceList))]
        public DynamicValue dynamicListLimit;

        [Header("Persistence Test (No Attribute)")]
        public Vector2Range autoIntRange = new(1, 10);
        public DynamicValue autoIntDynamic = DynamicValue.Default(5);
    }
}