# VBDLIS Tools

Công cụ hỗ trợ làm việc với hệ thống VBDLIS.

## 🚀 Bắt đầu nhanh

### Build Desktop Project

#### Windows
```powershell
# Build từ root directory
.\build-desktop-windows.ps1

# Hoặc build trực tiếp từ project directory
cd src\Haihv.Vbdlis.Tools\Haihv.Vbdlis.Tools.Desktop\build-scripts
.\build-local-windows.ps1

# Output: dist/velopack/VbdlisTools-{version}-Setup.zip
```

#### macOS
```bash
# Build từ root directory
./build-desktop-macos.sh

# Hoặc build trực tiếp từ project directory
cd src/Haihv.Vbdlis.Tools/Haihv.Vbdlis.Tools.Desktop/build-scripts
./build-local-macos.sh

# Output: dist/velopack/VbdlisTools-{version}-osx-arm64.dmg
```

---

## 📦 Tạo GitHub Release

```powershell
# Bước 1: Build local (tự động tăng version)
.\build-desktop-windows.ps1

# Bước 2: Tạo release (sử dụng version từ build)
.\create-desktop-release.ps1

# GitHub Actions sẽ:
# - Build Windows ONLY (không tăng version)
# - Tạo GitHub Release
# - Upload Windows installer
```

**Lưu ý:** macOS builds phải build local và upload thủ công lên GitHub Release.

---

## 🔧 Build Scripts

### Root Level Scripts (Wrappers)
| Script | Platform | Mục đích |
|--------|----------|---------|
| **build-desktop-windows.ps1** | Windows | Build Desktop project (wrapper) |
| **build-desktop-macos.sh** | macOS | Build Desktop project (wrapper) |
| **create-desktop-release.ps1** | Cross | Tạo GitHub release cho Desktop (wrapper) |

### Project Level Scripts
Mỗi project có thư mục `build-scripts` riêng:
- **Desktop**: `src/Haihv.Vbdlis.Tools/Haihv.Vbdlis.Tools.Desktop/build-scripts/`
  - `build-local-windows.ps1` - Build Windows với Velopack
  - `build-local-macos.sh` - Build macOS với Velopack
  - `create-release.ps1` - Tạo GitHub release
  - `version.json` - Quản lý version
  - `README.md` - Hướng dẫn chi tiết

---

## 📝 Quản lý Version

Version format sử dụng hai chuẩn khác nhau:
- **Package Version** (SemVer2 - 3 parts): `Major.Minor.yyMMDDBB` - Cho Velopack
- **Assembly Version** (4 parts): `Major.Minor.yyMM.DDBB` - Cho .NET

Ví dụ cho build ngày 11/02/2026, build #2:
- Package Version: `1.0.26021102` (dùng cho Velopack installer)
- Assembly Version: `1.0.2602.1102` (dùng cho .NET runtime)
- File Version: `1.0.2602.1102` (hiển thị trong file properties)

### File Version

Mỗi project có file `version.json` riêng trong thư mục `build-scripts/`:
- Desktop: `src/Haihv.Vbdlis.Tools/Haihv.Vbdlis.Tools.Desktop/build-scripts/version.json`

```json
{
  "majorMinor": "1.0",
  "currentVersion": "1.0.26021102",
  "assemblyVersion": "1.0.2602.1102",
  "lastBuildDate": "2026-02-11",
  "dateCode": "2602",
  "buildNumber": 2,
  "history": [
    {
      "version": "1.0.26021102",
      "date": "2026-02-11",
      "timestamp": "2026-02-11 11:15:30"
    }
  ]
}
```

### Cơ chế tự động tăng Version

- **Local builds** (project-level scripts):
  - ✅ Tự động tăng version
  - ✅ Cập nhật `build-scripts/version.json`
  - ✅ Cập nhật file `.csproj` khi build
  - 📝 Lưu lịch sử build

- **GitHub Actions** (`.github/workflows/release.yml`):
  - 🔒 Sử dụng version ĐÃ KHÓA từ `version.json`
  - ❌ KHÔNG tự động tăng version
  - ✅ Build Windows ONLY

---

## 🛠️ Tech Stack

- **.NET 10.0** - Framework
- **Avalonia UI** - Cross-platform UI
- **SQLite** - Database
- **Playwright** - Browser automation
- **Serilog** - Logging
- **EPPlus** - Excel processing
- **Velopack** - Auto-update installer

---

## 📋 Yêu cầu hệ thống

### Để Build:
- **.NET 10.0 SDK**
- **Velopack CLI** (tự động cài bởi build scripts)

### Để chạy ứng dụng:
- **Windows 10+** hoặc **macOS 10.15+**
- **.NET 10.0 Runtime** (đã bao gồm trong installer)
- **Kết nối Internet** (lần chạy đầu tiên - ứng dụng sẽ tự động tải Chromium ~150MB)

---

## 🌐 Playwright Browsers

Ứng dụng sử dụng Playwright để tự động hóa browser. **Chromium browser KHÔNG được bundle** trong installer/DMG để giữ kích thước file nhỏ (~50MB thay vì ~200MB).

### Hành vi lần chạy đầu tiên

Khi chạy lần đầu, ứng dụng sẽ tự động:
1. Phát hiện Chromium chưa được cài đặt
2. Tải Chromium (~150MB)
3. Cài đặt vào thư mục cache của user
4. Khởi động bình thường

**Yêu cầu:**
- Kết nối Internet khi chạy lần đầu
- ~150MB dung lượng trống
- Cho phép download trong firewall/antivirus

**Lợi ích:**
- ✅ Installer/DMG nhẹ hơn (~50MB)
- ✅ Download và cài đặt nhanh hơn
- ✅ Chromium luôn được cập nhật từ Playwright
- ⚠️ Cần internet lần chạy đầu tiên

---

## 📝 License

© 2025 vpdkbacninh.vn | haihv.vn

---

## 🆘 Hỗ trợ

Nếu gặp vấn đề hoặc có câu hỏi, vui lòng mở issue trên GitHub.
