# 🚀 Quick Start: Tạo Release trên GitHub

## TL;DR - Cách nhanh nhất

### Một nền tảng (Windows hoặc macOS):
```powershell
# 1. Test build local
.\build-all.ps1

# 2. Tạo release
.\create-release.ps1

# 3. Đợi GitHub Actions build (~10 phút)
# Done! ✅
```

### Nhiều nền tảng (Windows + macOS cùng version):
```powershell
# 1. Lock version trước
.\prepare-release.ps1

# 2. Build Windows
.\build\windows-velopack.ps1

# 3. Build macOS (trên máy Mac hoặc GitHub Actions)
./build/macos.sh

# 4. Tạo release
.\create-release.ps1

# Done! Windows và macOS có cùng version number ✅
```

## Chi tiết từng bước

### 🔒 Workflow với Version Lock (Recommended cho multi-platform)

**Khi nào dùng:** Build nhiều nền tảng với cùng version number

#### Bước 1: Lock version
```powershell
# Chạy script để khóa version trước khi build
.\prepare-release.ps1

# Script sẽ:
# - Tạo version mới dựa trên ngày hiện tại
# - Cập nhật version.json
# - Cập nhật .csproj file
# - Hiển thị version đã lock
```

**Output:**
```
Version locked: 1.0.25012901
Assembly version: 1.0.2501.2901

Next steps:
1. Build Windows: .\build\windows-velopack.ps1
2. Build macOS:   ./build/macos.sh
3. Create release: .\create-release.ps1

Both builds will use the same version: 1.0.25012901
```

#### Bước 2: Build platforms
```powershell
# Windows
.\build\windows-velopack.ps1

# macOS (on Mac machine or via GitHub Actions)
./build/macos.sh
```

**Lưu ý:** Build scripts sẽ tự động detect version đã lock và KHÔNG tăng build number.

#### Bước 3: Create release
```powershell
.\create-release.ps1
```

### 🚀 Workflow tự động (Nhanh nhất)

**Khi nào dùng:** Build qua GitHub Actions, không quan tâm version khác nhau

### 1️⃣ Chuẩn bị code

```powershell
# Commit changes
git add .
git commit -m "feat: add new features"
git push origin main
```

### 2️⃣ Build và test local

```powershell
# Build Windows
.\build-all.ps1

# Test installer
.\dist\velopack\VbdlisTools-*.exe
```

### 3️⃣ Tạo release tag

**Tự động (Recommended):**
```powershell
.\create-release.ps1
```

**Thủ công:**
```bash
git tag -a "v1.0.25120905" -m "Release v1.0.25120905"
git push origin "v1.0.25120905"
```

### 4️⃣ GitHub Actions tự động

Sau khi push tag, GitHub sẽ:
- ✅ Build Windows (Velopack installer)
- ✅ Build macOS arm64 (Apple Silicon M1/M2/M3/M4)
- ✅ Tạo GitHub Release
- ✅ Upload tất cả installers

**Xem tiến trình:** https://github.com/haitnmt/Vbdlis-Tools/actions

### 5️⃣ Verify release

**Kiểm tra:** https://github.com/haitnmt/Vbdlis-Tools/releases

Files nên có:
- ✅ `VbdlisTools-[version]-win-Setup.exe`
- ✅ `VbdlisTools-[version]-win-full.nupkg`
- ✅ `VbdlisTools-[version]-osx-arm64.zip`
- ✅ `RELEASES` (manifest)

### 6️⃣ Test auto-update

1. Install old version
2. Open app → Nhận thông báo update
3. Click update → Download và restart
4. App được cập nhật tự động ✅

## 📚 Đọc thêm

- **[GITHUB_RELEASES.md](GITHUB_RELEASES.md)** - Hướng dẫn chi tiết
- **[RELEASE_CHECKLIST.md](RELEASE_CHECKLIST.md)** - Checklist đầy đủ
- **[build/VERSION_MANAGEMENT.md](build/VERSION_MANAGEMENT.md)** - Quản lý version

## 🆘 Troubleshooting

### Version tăng lên giữa các lần build?

**Nguyên nhân:** Mỗi lần chạy build script, version tự động tăng build number.

**Giải pháp:** Dùng `prepare-release.ps1` để lock version TRƯỚC khi build:
```powershell
# 1. Lock version
.\prepare-release.ps1

# 2. Build tất cả platforms (version sẽ giống nhau)
.\build\windows-velopack.ps1
./build/macos.sh
```

### Build failed trên GitHub Actions?

**Kiểm tra:**
1. Vào **Actions** → Click vào run bị fail
2. Xem logs để tìm lỗi
3. Fix và push lại

### Auto-update không hoạt động?

**Check:**
1. Release phải **public** (không phải draft)
2. File `RELEASES` phải có trong release
3. `UpdateService.cs` config đúng repo

### Permission denied?

1. **Settings** → **Actions** → **General**
2. Workflow permissions: **Read and write permissions**
3. Save

## ✨ Tips

### Kiểm tra version hiện tại

```powershell
# Xem version log
Get-Content .\build\version.json | ConvertFrom-Json

# Check .csproj version
Select-String -Path "src\Haihv.Vbdlis.Tools\Haihv.Vbdlis.Tools.Desktop\Haihv.Vbdlis.Tools.Desktop.csproj" -Pattern "<Version>"
```

### Build cùng version cho cả Windows + macOS

**RECOMMENDED: Dùng prepare-release.ps1**
```powershell
# Lock version trước
.\prepare-release.ps1

# Build cả 2 platforms
.\build\windows-velopack.ps1
./build/macos.sh

# Cả 2 sẽ có CÙNG version number ✅
```

**Hoặc: Dùng GitHub Actions**
```powershell
# Push tag và để GitHub Actions build cả 2 platforms tự động
.\create-release.ps1

# GitHub sẽ build Windows và macOS cùng lúc với CÙNG version
```

### Skip build local, chỉ dùng GitHub Actions

```bash
# Chỉ cần push tag
git tag -a "v1.0.25120905" -m "Release"
git push origin "v1.0.25120905"

# GitHub tự build tất cả
```

### Pre-release / Beta version

```bash
git tag -a "v1.0.25120905-beta" -m "Beta release"
git push origin "v1.0.25120905-beta"
```

Sửa workflow để mark là pre-release:
```yaml
- name: Create Release
  uses: softprops/action-gh-release@v2
  with:
    prerelease: true  # Mark as pre-release
```

## 🎯 Best Practices

✅ **DO:**
- Test build local trước khi tạo release
- Viết release notes chi tiết
- Tag theo semantic versioning
- Commit `version.json` để team sync version

❌ **DON'T:**
- Tạo release từ untested code
- Skip version bump
- Force push tags
- Build production trên dirty working directory

## 📊 Version Format

```
v1.0.25120905
│ │  │  │  └─ Build number (01-99)
│ │  │  └──── Day (09)
│ │  └─────── Month + Year (2512 = Dec 2025)
│ └────────── Minor version
└──────────── Major version
```

Auto-increment mỗi lần build trong cùng ngày.

---

**🎉 Happy Releasing!**

Có thắc mắc? Xem [GITHUB_RELEASES.md](GITHUB_RELEASES.md) hoặc [RELEASE_CHECKLIST.md](RELEASE_CHECKLIST.md)
