# Phát Hành Phiên Bản Mới (WaveCore UPM)

**1. Cập nhật version**
Sửa số `"version": "x.y.z"` trong file `Assets/Deepwave/WaveCore/package.json`.

**2. Chạy lệnh Git**
Mở Terminal tại thư mục gốc, thay `[version]` bằng version mới và chạy lần lượt:

```bash
# Lưu lên nhánh main
git add .
git commit -m "Release [version]"
git push origin main

# Cắt sang nhánh upm và đẩy lên server
git subtree split --prefix="Assets/Deepwave/WaveCore" --branch upm
git push origin upm

# Gắn tag và đẩy tag
git tag `[version]` upm
git push origin `[version]`