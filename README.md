# VBDLIS Tools

Công cụ hỗ trợ làm việc với hệ thống VBDLIS.

## 🚀 Quick Start

### Local Build (Windows)

```powershell
# Build locally with auto-increment version
.\build-local.ps1

# Output: dist/velopack/VbdlisTools-{version}-Setup.zip
```

### Local Build (macOS)

```bash
# Build locally with auto-increment version
# Set BUNDLE_PLAYWRIGHT=1 to include browsers (~200MB DMG)
BUNDLE_PLAYWRIGHT=1 ./build-local-macos.sh

# Output: dist/velopack-macos-local/VbdlisTools-{version}-osx-arm64.dmg
```

---

## 📦 Create GitHub Release

```powershell
# Step 1: Build locally (auto-increments version)
.\build-local.ps1

# Step 2: Create release (uses version from build-local.ps1)
.\create-release.ps1

# GitHub Actions will:
# - Build Windows ONLY (no version increment)
# - Create GitHub Release
# - Upload Windows installer
```

**Note:** macOS builds must be done locally and manually uploaded to GitHub Release.

---

## 🔧 Build Scripts

| Script | Platform | Purpose |
|--------|----------|---------|
| **build-local.ps1** | Windows | Local build with auto-increment version |
| **build-local-macos.sh** | macOS | Local build with auto-increment version |
| **build\windows-velopack.ps1** | Windows | Build script (called by build-local.ps1 and GitHub Actions) |

---

## 📝 Version Management

Version format: `Major.Minor.YYMMDDBB`
- Example: `1.0.25121001`
  - `1.0` - Major.Minor version
  - `251210` - Date (2025-12-10)
  - `01` - Build number (increments per day)

### Version File: `build/version.json`

```json
{
  "majorMinor": "1.0",
  "currentVersion": "1.0.25121001",
  "assemblyVersion": "1.0.2512.1001",
  "lastBuildDate": "2025-12-10",
  "buildNumber": 1,
  "platforms": {
    "windows": {
      "lastBuilt": "2025-12-10T07:45:00",
      "version": "1.0.25121001"
    },
    "macos": {
      "lastBuilt": "",
      "version": ""
    }
  }
}
```

### Auto-Increment Behavior

- **Local builds** (`build-local.ps1` or `build-local-macos.sh`):
  - ✅ Auto-increments version
  - ✅ Updates `build/version.json`
  - ✅ Updates `.csproj` file

- **GitHub Actions** (`.github/workflows/release.yml`):
  - 🔒 Uses LOCKED version from `build/version.json`
  - ❌ Does NOT auto-increment
  - ✅ Builds Windows ONLY

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

## 📋 Requirements

### For Building:
- **.NET 10.0 SDK**
- **Velopack CLI** (auto-installed by build scripts)
- **Playwright browsers** (auto-installed: `playwright install chromium`)

### For Running:
- **Windows 10+** or **macOS 10.15+**
- **.NET 10.0 Runtime** (included in installer)
- **Internet connection** (first run only)

---

## ⚠️ Playwright Installation Issue

If you encounter the error:
```
Couldn't find project using Playwright. Ensure a project or a solution exists
```

**Fix:**
```powershell
# Windows
cd src\Haihv.Vbdlis.Tools\Haihv.Vbdlis.Tools.Desktop
dotnet build
playwright install chromium

# macOS
cd src/Haihv.Vbdlis.Tools/Haihv.Vbdlis.Tools.Desktop
dotnet build
playwright install chromium
```

The build scripts now handle Playwright browser installation automatically.

---

## 📝 License

© 2025 vpdkbacninh.vn | haihv.vn

---

## 🆘 Support

For issues or questions, please open an issue on GitHub.
