#!/bin/bash
# Script to build macOS with Velopack for GitHub Actions
# This script builds the application for RELEASE on macOS
# - Uses LOCKED version from version.json (no auto-increment)
# - For GitHub Actions release workflow only

set -e

CONFIGURATION="${1:-Release}"
ARCH="${2:-arm64}"

echo "=== Building VBDLIS Tools for macOS with Velopack ==="
echo "Configuration: $CONFIGURATION"
echo "Architecture: $ARCH"
echo "Build Mode: RELEASE (locked version from version.json)"
echo ""

# Check if running in GitHub Actions
if [ "$GITHUB_ACTIONS" = "true" ]; then
    echo "🔒 Running in GitHub Actions - using LOCKED version"
else
    echo "⚠️  This script is intended for GitHub Actions"
    echo "   For local builds, use: ./build-local-macos.sh"
fi

# Check if Velopack is installed
echo "Checking for Velopack CLI..."
if ! command -v vpk &> /dev/null; then
    echo "Velopack CLI not found. Installing..."
    dotnet tool install --global vpk
    echo "Velopack CLI installed!"
else
    echo "Velopack CLI found!"
fi

# Paths
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"
PROJECT_PATH="$PROJECT_ROOT/src/Haihv.Vbdlis.Tools/Haihv.Vbdlis.Tools.Desktop"
PROJECT_FILE="$PROJECT_PATH/Haihv.Vbdlis.Tools.Desktop.csproj"
PUBLISH_PATH="$PROJECT_PATH/bin/publish/velopack"
OUTPUT_PATH="$PROJECT_ROOT/dist/velopack"
VERSION_LOG_FILE="$SCRIPT_DIR/version.json"

# Read version from version.json (NO AUTO-INCREMENT)
echo ""
echo "Reading version from version.json..."
if [ ! -f "$VERSION_LOG_FILE" ]; then
    echo "❌ version.json not found!"
    echo "   This file should be committed to the repository."
    exit 1
fi

# Read package version from version.json
PACKAGE_VERSION=$(grep -o '"currentVersion"[[:space:]]*:[[:space:]]*"[^"]*"' "$VERSION_LOG_FILE" | cut -d'"' -f4)
ASSEMBLY_VERSION=$(grep -o '"assemblyVersion"[[:space:]]*:[[:space:]]*"[^"]*"' "$VERSION_LOG_FILE" | cut -d'"' -f4)

if [ -z "$PACKAGE_VERSION" ]; then
    echo "❌ No version found in version.json!"
    exit 1
fi

echo ""
echo "=== VERSION FROM version.json (LOCKED) ==="
echo "Package Version:  $PACKAGE_VERSION (for Velopack)"
echo "Assembly Version: $ASSEMBLY_VERSION (for .NET)"
echo "🔒 NO auto-increment on GitHub Actions"
echo ""

# Update .csproj with locked version
echo ""
echo "Updating .csproj with locked version..."
if [ -f "$PROJECT_FILE" ]; then
    # Create backup
    cp "$PROJECT_FILE" "$PROJECT_FILE.bak"

    # Update or add version properties
    if grep -q '<AssemblyVersion>' "$PROJECT_FILE"; then
        sed -i.tmp "s|<AssemblyVersion>.*</AssemblyVersion>|<AssemblyVersion>$ASSEMBLY_VERSION</AssemblyVersion>|" "$PROJECT_FILE"
    else
        sed -i.tmp "s|<PropertyGroup>|<PropertyGroup>\n    <AssemblyVersion>$ASSEMBLY_VERSION</AssemblyVersion>|" "$PROJECT_FILE"
    fi

    if grep -q '<FileVersion>' "$PROJECT_FILE"; then
        sed -i.tmp "s|<FileVersion>.*</FileVersion>|<FileVersion>$ASSEMBLY_VERSION</FileVersion>|" "$PROJECT_FILE"
    else
        sed -i.tmp "s|<PropertyGroup>|<PropertyGroup>\n    <FileVersion>$ASSEMBLY_VERSION</FileVersion>|" "$PROJECT_FILE"
    fi

    if grep -q '<Version>' "$PROJECT_FILE"; then
        sed -i.tmp "s|<Version>.*</Version>|<Version>$ASSEMBLY_VERSION</Version>|" "$PROJECT_FILE"
    else
        sed -i.tmp "s|<PropertyGroup>|<PropertyGroup>\n    <Version>$ASSEMBLY_VERSION</Version>|" "$PROJECT_FILE"
    fi

    # Add InformationalVersion for Velopack (3-part version)
    if grep -q '<InformationalVersion>' "$PROJECT_FILE"; then
        sed -i.tmp "s|<InformationalVersion>.*</InformationalVersion>|<InformationalVersion>$PACKAGE_VERSION</InformationalVersion>|" "$PROJECT_FILE"
    else
        # Add after Version tag
        sed -i.tmp "/<Version>.*<\/Version>/a\\
    <InformationalVersion>$PACKAGE_VERSION</InformationalVersion>
" "$PROJECT_FILE"
    fi

    rm -f "$PROJECT_FILE.bak" "$PROJECT_FILE.tmp"
    echo ".csproj updated with version $ASSEMBLY_VERSION"
fi

# Use packageVersion for Velopack
VERSION="$PACKAGE_VERSION"

# Clean previous builds
echo ""
echo "Cleaning previous builds..."
if [ -d "$PUBLISH_PATH" ]; then
    rm -rf "$PUBLISH_PATH"
fi
if [ -d "$OUTPUT_PATH" ]; then
    rm -rf "$OUTPUT_PATH"
fi
mkdir -p "$OUTPUT_PATH"

# Step 1: Publish application
echo ""
echo "Step 1: Publishing application..."
RUNTIME="osx-$ARCH"

dotnet publish "$PROJECT_FILE" \
    --configuration "$CONFIGURATION" \
    --runtime "$RUNTIME" \
    --self-contained true \
    --output "$PUBLISH_PATH" \
    /p:PublishSingleFile=false \
    /p:IncludeNativeLibrariesForSelfExtract=true \
    /p:DebugType=embedded

if [ $? -ne 0 ]; then
    echo "Publish failed with exit code $?"
    exit 1
fi

echo "✅ Application published successfully!"

# Step 2: Code signing (if certificates are available)
echo ""
echo "Step 2: Code signing..."

# Check for code signing environment variables
if [ -n "$MACOS_CERTIFICATE" ] && [ -n "$MACOS_CERTIFICATE_PWD" ] && [ -n "$MACOS_KEYCHAIN_PWD" ]; then
    echo "📝 Code signing certificates detected"

    # Decode certificate
    echo "$MACOS_CERTIFICATE" | base64 --decode > certificate.p12

    # Create temporary keychain
    KEYCHAIN_PATH=$RUNNER_TEMP/app-signing.keychain-db
    security create-keychain -p "$MACOS_KEYCHAIN_PWD" "$KEYCHAIN_PATH"
    security set-keychain-settings -lut 21600 "$KEYCHAIN_PATH"
    security unlock-keychain -p "$MACOS_KEYCHAIN_PWD" "$KEYCHAIN_PATH"

    # Import certificate
    security import certificate.p12 -P "$MACOS_CERTIFICATE_PWD" -A -t cert -f pkcs12 -k "$KEYCHAIN_PATH"
    security list-keychain -d user -s "$KEYCHAIN_PATH"

    # Sign the app bundle
    APP_BUNDLE=$(find "$PUBLISH_PATH" -type d -name "*.app" -maxdepth 1 | head -n 1)
    if [ -n "$APP_BUNDLE" ]; then
        echo "Signing app bundle: $(basename "$APP_BUNDLE")"

        # Get signing identity
        SIGNING_IDENTITY=$(security find-identity -v -p codesigning "$KEYCHAIN_PATH" | grep "Developer ID Application" | head -n 1 | awk '{print $2}')

        if [ -n "$SIGNING_IDENTITY" ]; then
            codesign --force --deep --sign "$SIGNING_IDENTITY" \
                --options runtime \
                --entitlements "$PROJECT_PATH/Entitlements.plist" \
                "$APP_BUNDLE"

            echo "✅ App signed successfully"

            # Notarize (if Apple ID credentials are available)
            if [ -n "$MACOS_NOTARIZATION_APPLE_ID" ] && [ -n "$MACOS_NOTARIZATION_TEAM_ID" ] && [ -n "$MACOS_NOTARIZATION_PWD" ]; then
                echo "Notarizing app..."

                # Create ZIP for notarization
                NOTARIZE_ZIP="$OUTPUT_PATH/app-for-notarization.zip"
                ditto -c -k --keepParent "$APP_BUNDLE" "$NOTARIZE_ZIP"

                # Submit for notarization
                xcrun notarytool submit "$NOTARIZE_ZIP" \
                    --apple-id "$MACOS_NOTARIZATION_APPLE_ID" \
                    --team-id "$MACOS_NOTARIZATION_TEAM_ID" \
                    --password "$MACOS_NOTARIZATION_PWD" \
                    --wait

                # Staple notarization ticket
                xcrun stapler staple "$APP_BUNDLE"

                echo "✅ App notarized successfully"
                rm -f "$NOTARIZE_ZIP"
            else
                echo "⚠️  Notarization credentials not found - skipping notarization"
            fi
        else
            echo "⚠️  Signing identity not found - skipping code signing"
        fi
    fi

    # Clean up
    security delete-keychain "$KEYCHAIN_PATH"
    rm -f certificate.p12
else
    echo "⚠️  Code signing certificates not found - creating unsigned build"
    echo "   To enable code signing, set these secrets in GitHub:"
    echo "   - MACOS_CERTIFICATE (base64 encoded .p12 file)"
    echo "   - MACOS_CERTIFICATE_PWD (certificate password)"
    echo "   - MACOS_KEYCHAIN_PWD (keychain password)"
    echo "   - MACOS_NOTARIZATION_APPLE_ID (optional, for notarization)"
    echo "   - MACOS_NOTARIZATION_TEAM_ID (optional, for notarization)"
    echo "   - MACOS_NOTARIZATION_PWD (optional, app-specific password)"
fi

# Step 3: Create Velopack package
echo ""
echo "Step 3: Creating Velopack package..."

vpk pack \
    --packId "Haihv.Vbdlis.Tools.Desktop" \
    --packVersion "$VERSION" \
    --packDir "$PUBLISH_PATH" \
    --mainExe "Haihv.Vbdlis.Tools.Desktop" \
    --outputDir "$OUTPUT_PATH" \
    --packTitle "VBDLIS Tools" \
    --packAuthors "haitnmt" \
    --icon "$PROJECT_PATH/Assets/appicon.icns" \
    --runtime "$RUNTIME"

if [ $? -ne 0 ]; then
    echo "Velopack packaging failed with exit code $?"
    exit 1
fi

echo "✅ Velopack package created!"

# Step 4: Create DMG from portable package
echo ""
echo "Step 4: Creating DMG installer..."

# Find the portable ZIP created by Velopack
PORTABLE_ZIP=$(find "$OUTPUT_PATH" -type f -name "*-Portable.zip" | head -n 1)

if [ -n "$PORTABLE_ZIP" ]; then
    echo "Found portable package: $(basename "$PORTABLE_ZIP")"

    # Create temporary directory to extract portable app
    TEMP_EXTRACT="$OUTPUT_PATH/temp_extract"
    rm -rf "$TEMP_EXTRACT"
    mkdir -p "$TEMP_EXTRACT"

    # Extract portable ZIP
    echo "Extracting portable package..."
    unzip -q "$PORTABLE_ZIP" -d "$TEMP_EXTRACT"

    # Find the .app bundle in extracted files
    APP_BUNDLE=$(find "$TEMP_EXTRACT" -type d -name "*.app" -maxdepth 2 | head -n 1)

    if [ -n "$APP_BUNDLE" ]; then
        echo "Found app bundle: $(basename "$APP_BUNDLE")"

        DMG_NAME="VbdlisTools-$PACKAGE_VERSION-osx-$ARCH.dmg"
        DMG_PATH="$OUTPUT_PATH/$DMG_NAME"
        TEMP_DMG="$OUTPUT_PATH/temp.dmg"
        TEMP_DIR="$OUTPUT_PATH/dmg_temp"

        # Create temporary directory for DMG contents
        rm -rf "$TEMP_DIR"
        mkdir -p "$TEMP_DIR"

        # Copy app bundle to temp directory
        cp -R "$APP_BUNDLE" "$TEMP_DIR/"

        # Create Applications symlink
        ln -s /Applications "$TEMP_DIR/Applications"

        # Create comprehensive README
        cat > "$TEMP_DIR/README.txt" << 'READMEEOF'
VBDLIS Tools - macOS (Unsigned)
================================

⚠️ "App is damaged and can't be opened" ERROR?

This is NORMAL for unsigned apps. Choose one of these methods:

────────────────────────────────────────────────────────────
METHOD 1: Terminal Command (RECOMMENDED - Easiest)
────────────────────────────────────────────────────────────

1. Drag VBDLIS Tools.app to Applications folder
2. Open Terminal (Applications → Utilities → Terminal)
3. Copy and paste this command:

   xattr -cr "/Applications/VBDLIS Tools.app"

4. Press Enter
5. Now open VBDLIS Tools normally (double-click or Spotlight)

✅ Done! The app will open without any issues.

────────────────────────────────────────────────────────────
METHOD 2: Right-Click (Alternative)
────────────────────────────────────────────────────────────

1. Drag VBDLIS Tools.app to Applications folder
2. DON'T double-click the app yet
3. Right-click (or Control+Click) on VBDLIS Tools.app
4. Select "Open" from the menu
5. Click "Open" in the security dialog
6. App will open and macOS will remember this choice

✅ Done! You can now open the app normally in the future.

────────────────────────────────────────────────────────────
WHY THIS HAPPENS?
────────────────────────────────────────────────────────────

• This app is NOT signed with an Apple Developer certificate ($99/year)
• macOS Gatekeeper blocks unsigned apps downloaded from internet
• This is a FREE open-source app, so we don't have Apple Developer signing
• The commands above safely bypass this security check
• Your app and data are safe - this is just a macOS security feature

────────────────────────────────────────────────────────────
FEATURES
────────────────────────────────────────────────────────────

✅ Auto-update via Velopack
   - App checks for updates on startup
   - Download updates from GitHub Releases
   - No need to re-download DMG manually

✅ Native Apple Silicon support
   - Optimized for M1/M2/M3/M4 chips
   - Fast and efficient

────────────────────────────────────────────────────────────
SYSTEM REQUIREMENTS
────────────────────────────────────────────────────────────

• macOS 10.15 (Catalina) or later
• Apple Silicon (M1/M2/M3/M4) for arm64 build
• .NET 10.0 runtime (included in app)
• Internet connection (for auto-updates)

────────────────────────────────────────────────────────────
TROUBLESHOOTING
────────────────────────────────────────────────────────────

Q: Command not working?
A: Make sure you copied the FULL command including quotes

Q: Still see error after xattr command?
A: Try Method 2 (right-click → Open)

Q: App crashes on startup?
A: Check logs at ~/Library/Logs/VbdlisTools/

────────────────────────────────────────────────────────────
MORE INFORMATION
────────────────────────────────────────────────────────────

GitHub: https://github.com/haitnmt/Vbdlis-Tools
Issues: https://github.com/haitnmt/Vbdlis-Tools/issues

────────────────────────────────────────────────────────────

Enjoy using VBDLIS Tools! 🎉
READMEEOF

        # Create temporary DMG
        echo "Creating temporary DMG..."
        hdiutil create -volname "VBDLIS Tools" -srcfolder "$TEMP_DIR" -ov -format UDRW "$TEMP_DMG" > /dev/null 2>&1

        if [ $? -eq 0 ]; then
            # Convert to compressed DMG
            echo "Compressing DMG..."
            hdiutil convert "$TEMP_DMG" -format UDZO -o "$DMG_PATH" > /dev/null 2>&1
            rm -f "$TEMP_DMG"

            echo "✅ DMG created: $DMG_NAME"

            # Show DMG size
            DMG_SIZE=$(ls -lh "$DMG_PATH" | awk '{print $5}')
            echo "   Size: $DMG_SIZE"
        else
            echo "⚠️  Failed to create DMG"
        fi

        # Clean up temp directories
        rm -rf "$TEMP_DIR"
    else
        echo "⚠️  App bundle not found in portable package"
    fi

    # Clean up extraction directory
    rm -rf "$TEMP_EXTRACT"
else
    echo "⚠️  Portable ZIP not found"
fi

# List generated files
echo ""
echo "=== BUILD COMPLETED ==="
echo ""
echo "Generated files:"
ls -lh "$OUTPUT_PATH" | tail -n +2 | awk '{print "  - " $9 " (" $5 ")"}'

echo ""
echo "✅ macOS BUILD SUCCESSFUL!"
echo ""
echo "Version built: $PACKAGE_VERSION"
echo "Architecture: $ARCH"
echo "Output directory: $OUTPUT_PATH"
echo ""
echo "📦 DISTRIBUTION FILES:"
echo "   DMG:      VbdlisTools-$PACKAGE_VERSION-osx-$ARCH.dmg"
echo "   Portable: Haihv.Vbdlis.Tools.Desktop-osx-Portable.zip"
echo ""
