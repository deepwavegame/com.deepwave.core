using System;
using UnityEngine;

namespace Deepwave.Core
{
    /// <summary>
    /// Use this attribute on a class (MonoBehaviour) to display a stylized, branded header in the Unity Inspector.
    /// Supports a title and an optional icon name from the Unity EditorResources.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class InspectorHeaderAttribute : PropertyAttribute
    {
        // ── Public Fields ─────────────────────────────────────────────────
        /// <summary>Title text displayed in the header.</summary>
        public string Title;
        /// <summary>Optional icon name (e.g., "d_UserControl") to display alongside the title.</summary>
        public string Icon;

        // ── Constructor ───────────────────────────────────────────────────
        /// <summary>Defines a branded header with a specific title and optional icon.</summary>
        public InspectorHeaderAttribute(string title, string icon = "")
        {
            Title = title;
            Icon = icon;
        }
    }
}