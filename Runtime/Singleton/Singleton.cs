using System;
using System.Reflection;
using UnityEngine;

namespace Deepwave.Core
{
    /// <summary>
    /// Thread-safe generic Singleton for plain C# classes.
    /// Uses <see cref="Lazy{T}"/> for lazy initialization and optimal performance.
    /// </summary>
    /// <typeparam name="T">The type of the singleton instance.</typeparam>
    public abstract class Singleton<T> where T : class
    {
        // ── Constants & Static ────────────────────────────────────────────
        private static readonly Lazy<T> _lazyInstance = new(CreateInstance);

        // ── Properties ────────────────────────────────────────────────────
        /// <summary>Gets the unique instance of the singleton.</summary>
        public static T Instance => _lazyInstance.Value;

        // ── Private Helpers ───────────────────────────────────────────────
        /// <summary>
        /// Uses reflection to invoke the parameterless constructor.
        /// Encourages the use of private/protected constructors in derived classes to enforce singleton integrity.
        /// </summary>
        private static T CreateInstance()
        {
            var constructor = typeof(T).GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                null,
                Type.EmptyTypes,
                null);

            if (constructor == null)
            {
                throw new InvalidOperationException($"[Singleton] Class {typeof(T).Name} must have a parameterless constructor.");
            }

            if (constructor.IsPublic)
            {
                Debug.LogWarning($"[Singleton] Constructor of {typeof(T).Name} is public. It should be private or protected to prevent external instantiation.");
            }

            return (T)constructor.Invoke(null);
        }
    }
}