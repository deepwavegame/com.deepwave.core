using System.Collections.Generic;
using UnityEngine;

namespace Deepwave.Core.Tests
{
    public class DynamicRangeTester : MonoBehaviour
    {
        [Header("Static Range Attributes")]
        // Khóa gi?i h?n c?ng cho s? th?c
        [DynamicRange(-5f, 5f)]
        public DynamicFloat staticFloatRange;

        // Khóa gi?i h?n c?ng cho s? nguyên
        [DynamicRange(1f, 10f)]
        public DynamicInt staticIntRange;


        [Header("Dynamic List Limits")]
        // Danh sách dùng ?? tham chi?u ?? dài
        public List<string> referenceList = new() { "Item 1", "Item 2", "Item 3" };

        // Gi?i h?n Max t? ??ng thay ??i theo Count c?a referenceList
        [DynamicRange(0f, nameof(referenceList))]
        public DynamicInt dynamicListLimit;


        [Header("Raw Range Drawers")]
        // Hi?n th? thanh tr??t Min-Max ??c l?p không c?n Attribute
        public Vector2Range genericFloatRange = new(0f, 1f);
        public Vector2IntRange genericIntRange = new(0, 5);
    }
}