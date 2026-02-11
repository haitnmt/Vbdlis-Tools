# Build Scripts Overview

## 📁 Cấu trúc Build Scripts

Build scripts được tổ chức theo từng project để dễ quản lý và mở rộng.

### Root Level (Wrapper Scripts)
```
Vbdlis-Tools/
├── build-desktop-windows.ps1      # Build Desktop trên Windows
├── build-desktop-macos.sh         # Build Desktop trên macOS  
└── create-desktop-release.ps1     # Tạo release cho Desktop
```

### Project Level (Actual Build Scripts)
```
src/Haihv.Vbdlis.Tools/
└── Haihv.Vbdlis.Tools.Desktop/
    └── build-scripts/
        ├── build-local-windows.ps1    # Build Windows với Velopack
        ├── build-local-macos.sh       # Build macOS với Velopack
        ├── create-release.ps1         # Tạo GitHub release
        ├── version.json              # Version tracking
        └── README.md                 # Hướng dẫn chi tiết
```

## 🚀 Sử dụng

### Option 1: Từ Root Directory (Khuyến nghị)
```powershell
# Windows
.\build-desktop-windows.ps1

# macOS
./build-desktop-macos.sh

# Create release
.\create-desktop-release.ps1
```

### Option 2: Trực tiếp từ Project Directory
```powershell
# Navigate to project
cd src\Haihv.Vbdlis.Tools\Haihv.Vbdlis.Tools.Desktop\build-scripts

# Windows
.\build-local-windows.ps1

# macOS  
./build-local-macos.sh

# Create release
.\create-release.ps1
```

## 📦 Version Management

Mỗi project quản lý version riêng trong file `build-scripts/version.json`:
- Format: `Major.Minor.yyMMDDBB` (SemVer2 - 3 parts)
- Ví dụ: `1.0.26021102` = version 1.0, ngày 11/02/2026, build 02
- Tự động tăng theo ngày và build number (2 chữ số)

## ➕ Thêm Project Mới

Khi thêm project mới (API, App, etc.):

1. Tạo thư mục `build-scripts/` trong project
2. Copy và điều chỉnh scripts từ Desktop project
3. Tạo wrapper scripts ở root (tùy chọn)
4. Cập nhật README.md

## 📝 Lợi ích của cấu trúc này

✅ **Tách biệt rõ ràng**: Mỗi project tự quản lý build của mình
✅ **Dễ mở rộng**: Thêm project mới không ảnh hưởng project cũ
✅ **Version độc lập**: Mỗi project có version riêng
✅ **Wrapper tiện lợi**: Build từ root nếu cần
✅ **Linh hoạt**: Có thể build trực tiếp từ project

## 🔗 Xem thêm

- Desktop build scripts: [src/Haihv.Vbdlis.Tools/Haihv.Vbdlis.Tools.Desktop/build-scripts/README.md](src/Haihv.Vbdlis.Tools/Haihv.Vbdlis.Tools.Desktop/build-scripts/README.md)
- Main README: [README.md](README.md)

