# Version Locking System

## 🎯 Mục đích

Đảm bảo Windows và macOS builds có **cùng version number** khi release.

## ❌ Vấn đề trước đây

Khi build nhiều nền tảng tuần tự, mỗi lần build sẽ tự động tăng build number:

```powershell
# Build Windows
.\build\windows-velopack.ps1
# Output: Version 1.0.25012901

# Đợi vài phút, build macOS
./build/macos.sh
# Output: Version 1.0.25012902  ⚠️ Version khác!
```

**Kết quả:**
- Windows release: `v1.0.25012901`
- macOS release: `v1.0.25012902`
- ❌ Người dùng bối rối vì version không khớp
- ❌ Khó quản lý releases

## ✅ Giải pháp: Version Locking

### Cách hoạt động

1. **prepare-release.ps1**: Lock version TRƯỚC khi build
2. **Build scripts**: Detect version đã lock và KHÔNG tăng build number

### Workflow

```powershell
# Bước 1: Lock version
.\prepare-release.ps1

# Script sẽ:
# - Đọc version.json
# - Tính version mới dựa trên ngày hiện tại
# - Cập nhật version.json với version mới
# - Cập nhật .csproj file với version mới
# - Hiển thị version đã lock

# Output:
# ================================================================================
# Version Lock Summary
# ================================================================================
# Version locked: 1.0.25012901
# Assembly version: 1.0.2501.2901
# 
# Next steps:
# 1. Build Windows: .\build\windows-velopack.ps1
# 2. Build macOS:   ./build/macos.sh
# 3. Create release: .\create-release.ps1
# 
# Both builds will use the same version: 1.0.25012901
# ================================================================================

# Bước 2: Build Windows
.\build\windows-velopack.ps1

# Script detect version locked:
# Version is LOCKED - using existing version from .csproj
# Locked Version: 1.0.2501.2901 (Assembly)
# Locked Package Version: 1.0.25012901 (Velopack 3-part SemVer2)
# Build Number: 1

# Bước 3: Build macOS
./build/macos.sh

# Script detect version locked:
# Version is LOCKED - using existing version from .csproj
# Locked Version: 1.0.2501.2901 (Assembly)
# Locked Package Version: 1.0.25012901 (Velopack 3-part SemVer2)
# Build Number: 1

# Bước 4: Create release
.\create-release.ps1

# Result: Cả Windows và macOS có CÙNG version: 1.0.25012901 ✅
```

## 🔧 Implementation Details

### prepare-release.ps1

**Chức năng:**
- Parse version từ `version.json`
- Tạo version mới: `Major.Minor.YYMMDDBB`
- Cập nhật `version.json`:
  - `currentVersion`: Package version (3-part SemVer2)
  - `assemblyVersion`: Assembly version (4-part .NET)
  - `lastBuildDate`: Ngày hiện tại
  - `buildNumber`: Build number mới
- Cập nhật `.csproj` file với version mới
- Hiển thị next steps

**Code key logic:**
```powershell
# Tính version mới
$buildNum = if ($versionLog.lastBuildDate -eq $todayString) {
    $versionLog.buildNumber + 1
} else {
    1
}

# Format version
$packageVersion = "$majorMinor.$dateString$buildNumString"  # 1.0.25012901
$assemblyVersion = "$majorMinor.$yearMonth.$dayBuild"       # 1.0.2501.2901

# Update version.json
$versionLog.currentVersion = $packageVersion
$versionLog.assemblyVersion = $assemblyVersion
$versionLog.lastBuildDate = $todayString
$versionLog.buildNumber = $buildNum

# Update .csproj
# ... update Version, AssemblyVersion, FileVersion
```

### windows-velopack.ps1

**Version Lock Detection:**
```powershell
# Check if version is locked
$isVersionLocked = $false
if ($versionLog.lastBuildDate -eq $todayString) {
    # Read .csproj version
    if ($csprojContent -match '<Version>([\d\.]+)</Version>') {
        $csprojVersion = $matches[1]
        # Compare patch number (last part)
        $csprojPatch = $csprojVersion.Split('.')[3]
        $logPatch = $versionLog.assemblyVersion.Split('.')[3]
        
        if ($csprojPatch -eq $logPatch) {
            $isVersionLocked = $true
            # Use locked version
            $assemblyVersion = $csprojVersion
            $packageVersion = $versionLog.currentVersion
        }
    }
}

# Only update .csproj if NOT locked
if (-not $isVersionLocked) {
    Set-Content -Path $ProjectFile -Value $csprojContent -NoNewline
}
```

**Logic:**
1. Check if `lastBuildDate` matches today
2. Read version from `.csproj`
3. Compare `.csproj` version with `version.json` assembly version
4. If match → Version is locked → Use existing version
5. If not match → Version is NOT locked → Auto-increment

### macos.sh

**Version Lock Detection:**
```bash
# Check if version is locked
IS_VERSION_LOCKED=false
if [ "$LAST_BUILD_DATE" = "$TODAY_STRING" ]; then
    # Read .csproj version
    CSPROJ_VERSION=$(grep -oP '<Version>\K[^<]+' "$PROJECT_FILE" | head -1)
    if [ -n "$CSPROJ_VERSION" ]; then
        # Compare patch number
        CSPROJ_PATCH=$(echo "$CSPROJ_VERSION" | cut -d'.' -f4)
        LOG_ASSEMBLY_VERSION=$(grep -o '"assemblyVersion"[[:space:]]*:[[:space:]]*"[^"]*"' "$VERSION_LOG_FILE" | cut -d'"' -f4)
        LOG_PATCH=$(echo "$LOG_ASSEMBLY_VERSION" | cut -d'.' -f4)
        
        if [ "$CSPROJ_PATCH" = "$LOG_PATCH" ]; then
            IS_VERSION_LOCKED=true
            # Use locked version
            ASSEMBLY_VERSION="$CSPROJ_VERSION"
            LOG_PACKAGE_VERSION=$(grep -o '"currentVersion"[[:space:]]*:[[:space:]]*"[^"]*"' "$VERSION_LOG_FILE" | cut -d'"' -f4)
            PACKAGE_VERSION="$LOG_PACKAGE_VERSION"
        fi
    fi
fi

# Only update .csproj if NOT locked
if [ "$IS_VERSION_LOCKED" = false ]; then
    sed -i.bak "s|<Version>.*</Version>|<Version>$ASSEMBLY_VERSION</Version>|g" "$PROJECT_FILE"
fi
```

## 📊 Version Format

### Package Version (3-part SemVer2)
Format: `Major.Minor.YYMMDDBB`

**Ví dụ:** `1.0.25012901`
- Major: `1`
- Minor: `0`
- Patch: `25012901` (Year=25, Month=01, Day=29, Build=01)

**Mục đích:**
- Velopack yêu cầu 3-part SemVer2
- Dùng cho package version và GitHub release tags

### Assembly Version (4-part .NET)
Format: `Major.Minor.YYMM.DDBB`

**Ví dụ:** `1.0.2501.2901`
- Major: `1`
- Minor: `0`
- Build: `2501` (Year=25, Month=01)
- Revision: `2901` (Day=29, Build=01)

**Mục đích:**
- .NET assembly versioning standard
- Dùng trong `.csproj` file

## 🧪 Testing

### Test version locking workflow

```powershell
# 1. Lock version
.\prepare-release.ps1

# Expected output:
# Version locked: 1.0.25012901
# Assembly version: 1.0.2501.2901

# 2. Verify version.json updated
Get-Content .\build\version.json | ConvertFrom-Json

# Expected:
# currentVersion    : 1.0.25012901
# assemblyVersion   : 1.0.2501.2901
# lastBuildDate     : 2025-01-29
# buildNumber       : 1

# 3. Verify .csproj updated
Select-String -Path "src\Haihv.Vbdlis.Tools\Haihv.Vbdlis.Tools.Desktop\Haihv.Vbdlis.Tools.Desktop.csproj" -Pattern "<Version>"

# Expected:
# <Version>1.0.2501.2901</Version>

# 4. Build Windows
.\build\windows-velopack.ps1

# Expected output:
# Version is LOCKED - using existing version from .csproj
# Locked Version: 1.0.2501.2901 (Assembly)
# Locked Package Version: 1.0.25012901 (Velopack 3-part SemVer2)

# 5. Build macOS
./build/macos.sh

# Expected output:
# Version is LOCKED - using existing version from .csproj
# Locked Version: 1.0.2501.2901 (Assembly)
# Locked Package Version: 1.0.25012901 (Velopack 3-part SemVer2)

# 6. Verify both builds have same version
ls .\dist\velopack\*.exe
ls .\dist\velopack-macos\arm64\*.zip

# Expected:
# VbdlisTools-1.0.25012901-win-Setup.exe
# VbdlisTools-1.0.25012901-osx-arm64.zip
# ✅ Same version!
```

### Test auto-increment (without locking)

```powershell
# 1. Build Windows (NO prepare-release)
.\build\windows-velopack.ps1

# Expected output:
# Calculating build number for today...
# New day detected. Starting with build #1
# Version: 1.0.2501.2901 (Assembly)
# Package Version: 1.0.25012901 (Velopack 3-part SemVer2)

# 2. Build macOS (without prepare-release)
./build/macos.sh

# Expected output:
# Calculating build number for today...
# Same day build detected. Incrementing to build #2
# Version: 1.0.2501.2902 (Assembly)
# Package Version: 1.0.25012902 (Velopack 3-part SemVer2)
# ⚠️ Different version!
```

## 📚 Related Documentation

- **[QUICKSTART_RELEASE.md](QUICKSTART_RELEASE.md)** - Quick start guide
- **[GITHUB_RELEASES.md](GITHUB_RELEASES.md)** - GitHub releases guide
- **[build/VERSION_MANAGEMENT.md](build/VERSION_MANAGEMENT.md)** - Version management details
- **[RELEASE_CHECKLIST.md](RELEASE_CHECKLIST.md)** - Release checklist

## 🎓 Best Practices

1. **Luôn dùng prepare-release.ps1** khi build nhiều platforms
2. **Test local trước** khi push tag
3. **Verify version numbers** sau khi build
4. **Document version changes** trong release notes
5. **Use GitHub Actions** để build parallel (tránh version mismatch)

## 🆘 Troubleshooting

### Version vẫn tăng sau khi lock?

**Nguyên nhân:** Script không detect được version lock.

**Debug:**
```powershell
# Check version.json
Get-Content .\build\version.json | ConvertFrom-Json

# Check .csproj
Select-String -Path "src\Haihv.Vbdlis.Tools\Haihv.Vbdlis.Tools.Desktop\Haihv.Vbdlis.Tools.Desktop.csproj" -Pattern "<Version>"

# Verify lastBuildDate matches today
[DateTime]::Now.ToString("yyyy-MM-dd")
```

### Version không khớp giữa Windows và macOS?

**Nguyên nhân:** Không dùng prepare-release.ps1 TRƯỚC khi build.

**Giải pháp:**
```powershell
# 1. Reset .csproj (git checkout)
git checkout src/Haihv.Vbdlis.Tools/Haihv.Vbdlis.Tools.Desktop/Haihv.Vbdlis.Tools.Desktop.csproj

# 2. Lock version
.\prepare-release.ps1

# 3. Build lại
.\build\windows-velopack.ps1
./build/macos.sh
```

### GitHub Actions builds có version khác nhau?

**Nguyên nhân:** GitHub Actions chạy Windows và macOS parallel, không lock version.

**Giải pháp 1: Lock version trong workflow**
```yaml
# Add step before build
- name: Lock version
  run: .\prepare-release.ps1
  shell: pwsh

- name: Build Windows
  run: .\build\windows-velopack.ps1
```

**Giải pháp 2: Build sequential**
```yaml
# Build Windows first, then macOS uses same version
jobs:
  build-windows:
    # ...
  build-macos:
    needs: build-windows  # Wait for Windows to finish
    # ...
```

## ✅ Summary

**Version Locking System giải quyết:**
- ✅ Windows và macOS có cùng version number
- ✅ Tránh auto-increment giữa các lần build
- ✅ Dễ quản lý và track releases
- ✅ Người dùng không bối rối với version numbers

**Workflow:**
1. `.\prepare-release.ps1` - Lock version
2. `.\build\windows-velopack.ps1` - Build Windows (uses locked version)
3. `./build/macos.sh` - Build macOS (uses locked version)
4. `.\create-release.ps1` - Create release (same version for all platforms)

🎉 **Done!** Cả Windows và macOS có cùng version number!
