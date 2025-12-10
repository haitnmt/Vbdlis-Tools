# Build Scripts Restructure Summary

Date: 2025-12-10

## Overview

Restructured build scripts and release process to be cleaner and more straightforward:
- **Local builds**: Auto-increment version for both Windows and macOS
- **GitHub Actions**: Build Windows ONLY using locked version from `version.json`
- **Cleaned up**: Removed 14 unnecessary documentation files

---

## Key Changes

### 1. Version Management

**New file:** `build/version.json`
- Central version tracking file
- Auto-updated by local build scripts
- Used by GitHub Actions (locked version, no increment)

**Format:**
```json
{
  "majorMinor": "1.0",
  "currentVersion": "1.0.25121001",
  "assemblyVersion": "1.0.2512.1001",
  "lastBuildDate": "2025-12-10",
  "buildNumber": 1,
  "platforms": {
    "windows": { "lastBuilt": "...", "version": "..." },
    "macos": { "lastBuilt": "...", "version": "..." }
  }
}
```

---

### 2. Build Scripts

#### Windows: `build-local.ps1`
- ✅ Auto-increments version
- ✅ Updates `build/version.json`
- ✅ Updates `.csproj` file
- ✅ Builds Velopack installer
- ✅ Bundles Playwright browsers

#### macOS: `build-local-macos.sh`
- ✅ Auto-increments version
- ✅ Updates `build/version.json`
- ✅ Updates `.csproj` file
- ✅ Builds DMG
- ✅ Bundles Playwright browsers (with `BUNDLE_PLAYWRIGHT=1`)

#### Build Engine: `build/windows-velopack.ps1`
- Used by both `build-local.ps1` and GitHub Actions
- Detects GitHub Actions environment via `GITHUB_ACTIONS` env var
- If GitHub Actions: Uses locked version (no increment)
- If local: Auto-increments version

---

### 3. Release Process

#### Script: `create-release.ps1`
1. Reads version from `build/version.json`
2. Creates git tag (e.g., `v1.0.25121001`)
3. Pushes tag to GitHub
4. Triggers GitHub Actions workflow

#### GitHub Actions: `.github/workflows/release.yml`
- Triggered by version tags (`v*.*.*`)
- Builds **Windows ONLY**
- Uses **LOCKED version** from `version.json`
- **Does NOT auto-increment version**
- Creates GitHub Release with Windows artifacts

---

### 4. Workflow

```
┌─────────────────────────────────────────────────────────────┐
│ LOCAL BUILD (Developer)                                     │
├─────────────────────────────────────────────────────────────┤
│ Windows:  .\build-local.ps1                                 │
│ macOS:    ./build-local-macos.sh                            │
│                                                             │
│ ✅ Auto-increments version                                  │
│ ✅ Updates build/version.json                               │
│ ✅ Updates .csproj                                          │
│ ✅ Builds installer/DMG                                     │
└─────────────────────────────────────────────────────────────┘
                          ↓
┌─────────────────────────────────────────────────────────────┐
│ CREATE RELEASE                                              │
├─────────────────────────────────────────────────────────────┤
│ .\create-release.ps1                                        │
│                                                             │
│ ✅ Reads version from version.json                          │
│ ✅ Creates git tag: v{version}                              │
│ ✅ Pushes to GitHub                                         │
└─────────────────────────────────────────────────────────────┘
                          ↓
┌─────────────────────────────────────────────────────────────┐
│ GITHUB ACTIONS (Automated)                                  │
├─────────────────────────────────────────────────────────────┤
│ .github/workflows/release.yml                               │
│                                                             │
│ 🔒 Uses LOCKED version from version.json                    │
│ ❌ Does NOT auto-increment                                  │
│ ✅ Builds Windows ONLY                                      │
│ ✅ Creates GitHub Release                                   │
│ ✅ Uploads Windows installer                                │
└─────────────────────────────────────────────────────────────┘
```

---

### 5. Files Removed

Cleaned up 14 unnecessary documentation files:

**Root directory:**
- `prepare-release.ps1` (replaced by simplified workflow)
- `build-all.ps1` (no longer needed)
- `create-release.sh` (Windows-only release process)
- `BUILD_DEPLOY.md` (consolidated into README)
- `BUILD_WORKFLOW.md` (consolidated into README)
- `CLICKONCE_MIGRATION.md` (not relevant)
- `DEPLOYMENT_COMPARISON.md` (not relevant)
- `GITHUB_RELEASES.md` (consolidated into README)
- `QUICKSTART_RELEASE.md` (consolidated into README)
- `RELEASE_CHECKLIST.md` (consolidated into README)
- `VERSION_LOCKING.md` (consolidated into README)
- `VELOPACK_AVALONIA_SETUP.md` (not relevant)
- `MACOS_FIX_DAMAGED.md` (not relevant)
- `PLAYWRIGHT_SETUP.md` (addressed in README)

**Copilot files:**
- `.copilot-commit-message-instructions.md`
- `.copilot-pull-request-description-instructions.md`

**Build folder:**
- `build/README.md` (consolidated into README)
- `build/VERSION_MANAGEMENT.md` (consolidated into README)

---

### 6. Files Kept

**Root directory:**
- `README.md` - Updated with complete documentation
- `LICENSE` - License file
- `build-local.ps1` - Local Windows build script
- `build-local-macos.sh` - Local macOS build script
- `create-release.ps1` - Release creation script

**Build folder:**
- `build/version.json` - Version tracking (NEW)
- `build/windows-velopack.ps1` - Build engine
- `build/.gitignore` - Git ignore rules

**GitHub:**
- `.github/workflows/release.yml` - GitHub Actions workflow

---

## Usage

### Local Development

**Windows:**
```powershell
# Build with auto-increment
.\build-local.ps1

# Output: dist/velopack/VbdlisTools-{version}-Setup.zip
```

**macOS:**
```bash
# Build with auto-increment (with bundled browsers)
BUNDLE_PLAYWRIGHT=1 ./build-local-macos.sh

# Output: dist/velopack-macos-local/VbdlisTools-{version}-osx-arm64.dmg
```

### Create Release

```powershell
# Step 1: Build locally (Windows or macOS)
.\build-local.ps1

# Step 2: Create release
.\create-release.ps1

# GitHub Actions will build Windows and create release
```

---

## Benefits

1. **Simpler**: Fewer scripts and docs to maintain
2. **Clearer**: Version management in one place (`build/version.json`)
3. **Consistent**: Same version across Windows and macOS builds
4. **Automated**: GitHub Actions handles Windows builds
5. **Flexible**: macOS builds done locally (no GitHub macOS runners needed)

---

## Version Format

`Major.Minor.YYMMDDBB`

Example: `1.0.25121001`
- `1.0` - Major.Minor (manual)
- `251210` - Date (2025-12-10)
- `01` - Build number (auto-increments per day)

---

## Playwright Issue Fix

The Playwright installation error is now documented in README.md with clear instructions.

**Issue:**
```
Couldn't find project using Playwright
```

**Fix:**
```powershell
cd src\Haihv.Vbdlis.Tools\Haihv.Vbdlis.Tools.Desktop
dotnet build
playwright install chromium
```

Build scripts now handle this automatically.

---

## Next Steps

1. Commit these changes
2. Test local build: `.\build-local.ps1`
3. Test release: `.\create-release.ps1`
4. Verify GitHub Actions build works

---

**End of restructure summary**
