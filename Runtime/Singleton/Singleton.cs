using System;
using System.Reflection;
using UnityEngine;

namespace Deepwave.Core
{
    /// <summary>
    /// Generic Singleton an toàn luồng cho các lớp C# thuần.
    /// Khởi tạo lười (Lazy) giúp tiết kiệm bộ nhớ cho đến khi thực sự được gọi.
    /// </summary>
    public abstract class Singleton<T> where T : class
    {
        // Lazy<T> đảm bảo an toàn luồng (thread-safe) mặc định với hiệu suất tối đa.
        private static readonly Lazy<T> _lazyInstance = new(CreateInstance);

        public static T Instance => _lazyInstance.Value;

        /// <summary>
        /// Dùng Reflection để gọi constructor.
        /// Cho phép ẩn constructor (private) ở class con để ngăn chặn khởi tạo bằng từ khóa 'new' bên ngoài.
        /// </summary>
        private static T CreateInstance()
        {
            // Tìm constructor không tham số (kể cả private hay protected)
            var constructor = typeof(T).GetConstructor(
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                null,
                Type.EmptyTypes,
                null);

            if (constructor == null)
            {
                throw new InvalidOperationException($"[Singleton] Lớp {typeof(T).Name} cần một constructor không tham số.");
            }

            // Cảnh báo nếu dev lỡ để constructor là public phá vỡ quy tắc Singleton
            if (constructor.IsPublic)
            {
                Debug.LogWarning($"[Singleton] Constructor của {typeof(T).Name} đang là Public. Nên đổi thành Private/Protected để ngăn tạo instance trái phép.");
            }

            return (T)constructor.Invoke(null);
        }
    }
}