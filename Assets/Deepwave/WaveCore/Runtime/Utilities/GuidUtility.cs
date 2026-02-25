using System;

namespace Deepwave.Core
{
    public static class GuidUtility
    {
        /// <summary>
        /// Tạo một GUID mới và trả về dưới dạng chuỗi ngắn (22 ký tự).
        /// An toàn cho URL và tên file (URL-safe Base64).
        /// </summary>
        public static string NewId()
        {
            // 1. Tạo GUID mới và lấy mảng byte (16 bytes)
            byte[] bytes = Guid.NewGuid().ToByteArray();

            // 2. Chuyển sang Base64
            string base64 = Convert.ToBase64String(bytes);
            // 3. Xử lý ký tự đặc biệt để an toàn và gọn hơn:
            // - Thay thế '+' bằng '-'
            // - Thay thế '/' bằng '_'
            // - Xóa dấu '=' ở cuối (padding không cần thiết)
            return base64
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('=');
        }

        // (Tùy chọn) Nếu bạn cần chuyển ngược từ String ngắn về Guid gốc
        public static Guid Parse(string shortId)
        {
            if (string.IsNullOrEmpty(shortId)) return Guid.Empty;

            string base64 = shortId
                .Replace('-', '+')
                .Replace('_', '/');

            // Thêm lại padding '=' cho đủ độ dài Base64 chuẩn
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }

            try
            {
                return new Guid(Convert.FromBase64String(base64));
            }
            catch
            {
                return Guid.Empty;
            }
        }
    }
}