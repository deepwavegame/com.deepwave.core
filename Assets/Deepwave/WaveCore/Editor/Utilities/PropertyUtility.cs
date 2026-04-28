using UnityEditor;
using UnityEngine;

namespace Deepwave.Core.Editor
{
    /// <summary>
    /// Utility methods for custom PropertyDrawers.
    /// </summary>
    public static class PropertyUtility
    {
        // ── Public API ────────────────────────────────────────────────────
        /// <summary>
        /// Gets the dynamic maximum limit based on a DynamicRangeAttribute's DynamicMaxList property.
        /// If no dynamic max is configured, the default max is returned.
        /// </summary>
        public static float GetDynamicMax(SerializedProperty property, float defaultMax, DynamicRangeAttribute attr)
        {
            if (attr == null || string.IsNullOrEmpty(attr.DynamicMaxList))
                return defaultMax;

            int lastDotIndex = property.propertyPath.LastIndexOf('.');
            string parentPath = lastDotIndex == -1 ? string.Empty : property.propertyPath[..lastDotIndex] + ".";
            string listPath = $"{parentPath}{attr.DynamicMaxList}";

            var listProp = property.serializedObject.FindProperty(listPath);
            if (listProp != null && listProp.isArray)
            {
                return Mathf.Max(attr.Min, listProp.arraySize - 1);
            }

            return defaultMax;
        }
    }
}
