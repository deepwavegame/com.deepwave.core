using System;
using UnityEngine;

namespace Deepwave.Core
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class InspectorHeaderAttribute : PropertyAttribute
    {
        public string title;
        public string icon;

        public InspectorHeaderAttribute(string title, string icon = "")
        {
            this.title = title;
            this.icon = icon;
        }
    }
}