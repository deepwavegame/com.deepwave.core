using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Deepwave.Core
{
    /// <summary>
    /// Generic Singleton class cho các thành phần MonoBehaviour.
    /// Đảm bảo chỉ có một instance duy nhất và tồn tại qua các scene.
    /// </summary>
    /// <typeparam name="T">Kiểu của thành phần MonoBehaviour.</typeparam>
    [DisallowMultipleComponent]
    public class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static readonly object _lock = new();
        private static bool _applicationIsQuitting;

        /// <summary>
        /// Lấy instance singleton. Ném ngoại lệ nếu không tìm thấy instance.
        /// </summary>
        public static T Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_applicationIsQuitting)
                    {
                        Debug.LogWarning($"[Singleton] Instance của {typeof(T).Name} đã bị hủy. Trả về null.");
                        return null;
                    }

                    if (_instance == null)
                    {
                        // Tìm instance hiện có
                        _instance = FindAnyObjectByType<T>();

                        if (_instance == null)
                        {
                            throw new MissingReferenceException($"[Singleton] Không tìm thấy instance của {typeof(T).Name} trong scene. Hãy thêm thủ công.");
                        }
                    }

                    return _instance;
                }
            }
        }

        /// <summary>
        /// Kiểm tra xem instance singleton có tồn tại không mà không khởi tạo.
        /// </summary>
        public static bool HasInstance => _instance != null || FindAnyObjectByType<T>() != null;

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
            }

            OnAwake();
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _applicationIsQuitting = true;
            }
        }

        /// <summary>
        /// Ghi đè phương thức này để thêm logic khởi tạo tùy chỉnh trong các lớp dẫn xuất.
        /// </summary>
        protected virtual void OnAwake() { }

#if UNITY_EDITOR
        protected virtual void Reset()
        {
            var instances = FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            if (instances.Length > 1)
            {
                EditorUtility.DisplayDialog("Singleton Error", $"Chỉ được phép có một instance của {typeof(T).Name} trong scene!", "OK");
                DestroyImmediate(this);
            }
        }
#endif
    }
}