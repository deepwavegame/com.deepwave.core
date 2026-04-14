using UnityEngine;

namespace Deepwave.Core.Utilities
{
    /// <summary>
    /// Provides high-performance, framerate-independent mathematical functions.
    /// Designed for zero-GC execution in hot paths.
    /// </summary>
    public static class MathUtils
    {
        // ── Constants ─────────────────────────────────────────────────────
        /// <summary>Smallest positive value for float comparison.</summary>
        public const float FloatMin = 1e-10f;
        /// <summary>Squared epsilon used for distance comparison.</summary>
        public const float SqrEpsilon = 1e-8f;

        // ── Public API (Math Logic) ───────────────────────────────────────
        /// <summary>Calculates the square of a floating-point value.</summary>
        public static float Square(float value) => value * value;

        /// <summary>Calculates the squared magnitude of the distance between two points (more efficient than distance).</summary>
        public static float SqrDistance(Vector3 a, Vector3 b) => (b - a).sqrMagnitude;

        /// <summary>Normalizes an Euler angle to be within the range [-180, 180).</summary>
        public static float NormalizeEulerAngle(float angle)
        {
            while (angle < -180f) angle += 360f;
            while (angle >= 180f) angle -= 360f;
            return angle;
        }

        /// <summary>Calculates the internal angle of a triangle given its three side lengths using the Law of Cosines.</summary>
        public static float TriangleAngle(float aLen, float bLen, float cLen)
        {
            float cosAngle = Mathf.Clamp((bLen * bLen + cLen * cLen - aLen * aLen) / (2.0f * bLen * cLen), -1.0f, 1.0f);
            return Mathf.Acos(cosAngle);
        }

        // ── Public API (Rotation Utilities) ────────────────────────────────
        /// <summary>Creates a rotation from one vector to another, handling parallel and anti-parallel cases safely.</summary>
        public static Quaternion FromToRotation(Vector3 from, Vector3 to)
        {
            float theta = Vector3.Dot(from.normalized, to.normalized);
            if (theta >= 1f) return Quaternion.identity;

            if (theta <= -1f)
            {
                Vector3 axis = Vector3.Cross(from, Vector3.right);
                if (axis.sqrMagnitude == 0f) axis = Vector3.Cross(from, Vector3.up);
                return Quaternion.AngleAxis(180f, axis);
            }

            return Quaternion.AngleAxis(Mathf.Acos(theta) * Mathf.Rad2Deg, Vector3.Cross(from, to).normalized);
        }

        /// <summary>Safely normalizes a quaternion to avoid potential NaN results.</summary>
        public static Quaternion NormalizeSafe(Quaternion q)
        {
            float dot = Quaternion.Dot(q, q);
            if (dot > FloatMin)
            {
                float rsqrt = 1.0f / Mathf.Sqrt(dot);
                return new Quaternion(q.x * rsqrt, q.y * rsqrt, q.z * rsqrt, q.w * rsqrt);
            }
            return Quaternion.identity;
        }

        // ── Public API (Interpolation) ────────────────────────────────────
        /// <summary>Calculates the inverse-lerp alpha for a value within a range [a, b]. Returns 0-1 clamped.</summary>
        public static float InvLerp(float value, float a, float b)
        {
            if (Mathf.Approximately(a, b)) return 0f;
            return Mathf.Clamp01((value - a) / (b - a));
        }

        /// <summary>Calculates a framerate-independent alpha based on exponential decay.</summary>
        public static float ExpDecayAlpha(float speed, float deltaTime) => 1 - Mathf.Exp(-speed * deltaTime);

        /// <summary>Framerate-independent linear interpolation using exponential decay.</summary>
        public static float FloatInterp(float a, float b, float speed, float deltaTime)
        {
            return speed > 0f ? Mathf.Lerp(a, b, ExpDecayAlpha(speed, deltaTime)) : b;
        }

        /// <summary>Framerate-independent spherical linear interpolation for rotations using exponential decay.</summary>
        public static Quaternion SmoothSlerp(Quaternion a, Quaternion b, float speed, float deltaTime)
        {
            return speed > 0f ? Quaternion.Slerp(a, b, ExpDecayAlpha(speed, deltaTime)) : b;
        }

        // ── Public API (System Logic) ─────────────────────────────────────
        /// <summary>Computes the look-at rotation input (x, y Euler angles) in root-local space.</summary>
        public static Vector2 ComputeLookAtInput(Transform root, Transform from, Transform to)
        {
            Quaternion rot = Quaternion.LookRotation(to.position - from.position);
            rot = Quaternion.Inverse(root.rotation) * rot;

            Vector3 euler = rot.eulerAngles;
            return new Vector2(NormalizeEulerAngle(euler.x), NormalizeEulerAngle(euler.y));
        }
    }
}