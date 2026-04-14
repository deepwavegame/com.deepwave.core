using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Deepwave.Core
{
    /// <summary>
    /// Generic Singleton for MonoBehaviour components.
    /// Ensures that only one instance exists in the scene and optionally persists across scenes.
    /// </summary>
    /// <typeparam name="T">The type of the MonoBehaviour component.</typeparam>
    [DisallowMultipleComponent]
    public abstract class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        // ── Constants & Static ────────────────────────────────────────────
        private static T _instance;
        private static readonly object _lock = new();
        private static bool _isQuitting;

        // ── Properties ────────────────────────────────────────────────────
        /// <summary>Gets the unique instance of this singleton. Logs a warning if accessed after the application quits.</summary>
        public static T Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_isQuitting)
                    {
                        Debug.LogWarning($"[SingletonBehaviour] Requested {typeof(T).Name} after application quit. Returning null.");
                        return null;
                    }

                    if (_instance == null)
                    {
                        _instance = FindAnyObjectByType<T>();

                        if (_instance == null)
                        {
                            throw new MissingReferenceException($"[SingletonBehaviour] No instance of {typeof(T).Name} found in the scene.");
                        }
                    }

                    return _instance;
                }
            }
        }

        /// <summary>Returns true if an instance of the singleton currently exists in the memory or the scene.</summary>
        public static bool HasInstance => _instance != null || FindAnyObjectByType<T>() != null;

        // ── Unity Lifecycle ───────────────────────────────────────────────
        protected virtual void Awake()
        {
            lock (_lock)
            {
                if (_instance != null && _instance != this)
                {
                    Destroy(gameObject);
                    return;
                }

                _instance = this as T;
                _isQuitting = false; // Reset state if re-instantiated (e.g., scene reload)
            }

            OnInitialized();
        }

        protected virtual void OnEnable()
        {
            _isQuitting = false;
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _isQuitting = true;
            }
        }

        // ── Public API (Events / Initialization) ─────────────────────────
        /// <summary>Called when the singleton is initialized during Awake. Override for custom initialization logic.</summary>
        protected virtual void OnInitialized() { }

#if UNITY_EDITOR
        // ── Editor-Only ───────────────────────────────────────────────────
        protected virtual void Reset()
        {
            var instances = FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            if (instances.Length > 1)
            {
                EditorUtility.DisplayDialog("Singleton Conflict", $"Only one instance of {typeof(T).Name} is allowed in the scene.", "OK");
                DestroyImmediate(this);
            }
        }
#endif
    }
}