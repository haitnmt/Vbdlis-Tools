# Build Scripts - Desktop Project

This directory contains build scripts for the **Haihv.Vbdlis.Tools.Desktop** project.

## 📁 Scripts

### Windows Build
- **`build-local-windows.ps1`** - Build for Windows with Velopack
  ```powershell
  .\build-local-windows.ps1
  .\build-local-windows.ps1 -Configuration Debug
  ```

### macOS Build
- **`build-local-macos.sh`** - Build for macOS with Velopack
  ```bash
  chmod +x build-local-macos.sh
  ./build-local-macos.sh
  ./build-local-macos.sh Release arm64
  ./build-local-macos.sh Release x64
  ```

### Release
- **`create-release.ps1`** - Create GitHub release
  ```powershell
  .\create-release.ps1
  .\create-release.ps1 -Version "1.0.240211.1" -Message "Custom release message"
  ```

## 🔄 Workflow

### Development Build
1. Run build script for your platform:
   - Windows: `.\build-local-windows.ps1`
   - macOS: `./build-local-macos.sh`
2. Version is auto-incremented based on date and build number
3. Output is in `dist/velopack` directory

### Create Release
1. Build locally first (to generate version)
2. Run `.\create-release.ps1`
3. Script will create and push git tag
4. GitHub Actions will build and publish release

## 📦 Version Management

Version format uses two different standards:
- **Package Version** (SemVer2 - 3 parts): `MAJOR.MINOR.yyMMDDBB` - For Velopack
- **Assembly Version** (4 parts): `MAJOR.MINOR.yyMM.DDBB` - For .NET

Example for build on Feb 11, 2026, build #2:
- Package Version: `1.0.26021102` (used by Velopack installer)
- Assembly Version: `1.0.2602.1102` (used by .NET runtime)
- File Version: `1.0.2602.1102` (shown in file properties)

Where:
- `1.0` - Major.Minor (manual)
- `2602` or `260211` - Date code (YYMM or YYMMDD)
- `1102` or `02` - Day + Build number (DDBB)

Version is stored in `version.json` with history of all builds.

## 🎯 Output

- **Windows**: `Haihv.Vbdlis.Tools.Desktop.exe` packaged with Velopack
- **macOS**: `Haihv.Vbdlis.Tools.Desktop.app` packaged with Velopack
- **Updates**: Delta updates supported through Velopack

## 🛠️ Requirements

- .NET SDK 8.0+
- Velopack CLI (`dotnet tool install --global vpk`)
- Git (for release scripts)

## 📝 Notes

- Each platform maintains its own version tracking
- Build scripts are self-contained and work from project directory
- macOS builds require code signing for distribution

