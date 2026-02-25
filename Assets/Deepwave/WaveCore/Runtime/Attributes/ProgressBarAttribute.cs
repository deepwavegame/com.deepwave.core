using UnityEngine;

namespace Deepwave.Core
{
    public class ProgressBarAttribute : PropertyAttribute
    {
        public float min;
        public float max;
        public string unit; // Đơn vị (VD: km/h)

        public ProgressBarAttribute(float min, float max, string unit = "")
        {
            this.min = min;
            this.max = max;
            this.unit = unit;
        }
    }
}