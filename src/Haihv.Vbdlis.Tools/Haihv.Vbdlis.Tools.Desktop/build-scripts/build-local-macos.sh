#!/bin/bash
# Script to build Desktop project locally with auto-incrementing version (macOS)
# This script builds the application for LOCAL TESTING on macOS
# - Auto-increments version based on date and build number
# - Updates version.json with new version
# - Use this for development and testing
#
# For RELEASE builds, use: ./create-release-macos.sh

set -e

CONFIGURATION="${1:-Release}"
ARCH="${2:-arm64}"

echo "=== Building VBDLIS Tools Desktop LOCALLY with Velopack (macOS) ==="
echo "Configuration: $CONFIGURATION"
echo "Architecture: $ARCH"
echo "Build Mode: LOCAL (auto-increment version)"
echo ""

# Check if Velopack is installed
echo "Checking for Velopack CLI..."
if ! command -v vpk &> /dev/null; then
    echo "Velopack CLI not found. Installing..."
    dotnet tool install --global vpk
else
    echo "Velopack CLI found!"
fi

# Paths - relative to this script
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_PATH="$(dirname "$SCRIPT_DIR")"
PROJECT_FILE="$PROJECT_PATH/Haihv.Vbdlis.Tools.Desktop.csproj"
PUBLISH_PATH="$PROJECT_PATH/bin/publish/velopack"
OUTPUT_PATH="$PROJECT_PATH/dist/velopack"
VERSION_LOG_FILE="$PROJECT_PATH/build-scripts/version.json"

# Read or create version log
echo ""
echo "Reading version log..."
if [ -f "$VERSION_LOG_FILE" ]; then
    VERSION_LOG=$(cat "$VERSION_LOG_FILE")
    echo "Found existing version log"
else
    echo "Creating new version log"
    VERSION_LOG='{
        "majorMinor": "1.0",
        "currentVersion": "",
        "assemblyVersion": "",
        "buildNumber": 0,
        "lastBuildDate": "",
        "dateCode": "",
        "history": []
    }'
fi

# Parse version log using jq or python
if command -v jq &> /dev/null; then
    MAJOR_MINOR=$(echo "$VERSION_LOG" | jq -r '.majorMinor')
    BUILD_NUMBER=$(echo "$VERSION_LOG" | jq -r '.buildNumber // 0')
    LAST_BUILD_DATE=$(echo "$VERSION_LOG" | jq -r '.lastBuildDate // ""')
else
    # Fallback to python
    MAJOR_MINOR=$(echo "$VERSION_LOG" | python3 -c "import sys, json; print(json.load(sys.stdin).get('majorMinor', '1.0'))")
    BUILD_NUMBER=$(echo "$VERSION_LOG" | python3 -c "import sys, json; print(json.load(sys.stdin).get('buildNumber', 0))")
    LAST_BUILD_DATE=$(echo "$VERSION_LOG" | python3 -c "import sys, json; print(json.load(sys.stdin).get('lastBuildDate', ''))")
fi

# Get current date info
TODAY=$(date +%Y-%m-%d)
DATE_YY=$(date +%y)
DATE_MM=$(date +%m)
DATE_DD=$(date +%d)
TIMESTAMP=$(date +"%Y-%m-%d %H:%M:%S")

# Version increment logic
if [ "$LAST_BUILD_DATE" = "$TODAY" ]; then
    # Same day, increment build number
    BUILD_NUMBER=$((BUILD_NUMBER + 1))
    echo "Same day build detected. Incrementing build number to: $BUILD_NUMBER"
else
    # New day, reset build number
    echo "New day detected. Resetting build number."
    BUILD_NUMBER=1
    LAST_BUILD_DATE="$TODAY"
fi

# Format build number to 2 digits (01, 02, ..., 99)
BUILD_NUMBER_PADDED=$(printf "%02d" $BUILD_NUMBER)

# Build version strings:
# - PackageVersion (SemVer2 - 3 parts): Major.Minor.yyMMDDBB (for Velopack)
# - AssemblyVersion (4 parts): Major.Minor.yyMM.DDBB (for .NET)
# Example: Package=1.0.26021102, Assembly=1.0.2602.1102 (Feb 11, 2026, build 02)
DATE_CODE="${DATE_YY}${DATE_MM}"
PATCH_VERSION="${DATE_YY}${DATE_MM}${DATE_DD}${BUILD_NUMBER_PADDED}"
NEW_VERSION="${MAJOR_MINOR}.${PATCH_VERSION}"
ASSEMBLY_VERSION="${MAJOR_MINOR}.${DATE_YY}${DATE_MM}.${DATE_DD}${BUILD_NUMBER_PADDED}"
FILE_VERSION="$ASSEMBLY_VERSION"

echo ""
echo "📦 Version Information:"
echo "   Package Version: $NEW_VERSION (for Velopack)"
echo "   Assembly Version: $ASSEMBLY_VERSION (for .NET)"
echo "   File Version: $FILE_VERSION"
echo "   Date: ${DATE_YY}${DATE_MM}${DATE_DD}"
echo "   Build Number: $BUILD_NUMBER_PADDED"
echo ""

# Update version log using jq or python
if command -v jq &> /dev/null; then
    VERSION_LOG=$(echo "$VERSION_LOG" | jq \
        --arg ver "$NEW_VERSION" \
        --arg asm "$ASSEMBLY_VERSION" \
        --arg date "$TODAY" \
        --arg code "$DATE_CODE" \
        --arg num "$BUILD_NUMBER" \
        --arg ts "$TIMESTAMP" \
        '.currentVersion = $ver | 
         .assemblyVersion = $asm | 
         .lastBuildDate = $date | 
         .dateCode = $code | 
         .buildNumber = ($num | tonumber) |
         .history += [{version: $ver, date: $date, timestamp: $ts}]')
else
    # Fallback to python
    VERSION_LOG=$(echo "$VERSION_LOG" | python3 -c "
import sys, json
from datetime import datetime

data = json.load(sys.stdin)
data['currentVersion'] = '$NEW_VERSION'
data['assemblyVersion'] = '$ASSEMBLY_VERSION'
data['lastBuildDate'] = '$TODAY'
data['dateCode'] = '$DATE_CODE'
data['buildNumber'] = $BUILD_NUMBER
if 'history' not in data:
    data['history'] = []
data['history'].append({
    'version': '$NEW_VERSION',
    'date': '$TODAY',
    'timestamp': '$TIMESTAMP'
})
print(json.dumps(data, indent=2))
")
fi

# Save version log
echo "$VERSION_LOG" > "$VERSION_LOG_FILE"
echo "✅ Updated version log: $VERSION_LOG_FILE"

# Clean previous build outputs
echo ""
echo "Cleaning previous builds..."
[ -d "$PUBLISH_PATH" ] && rm -rf "$PUBLISH_PATH" && echo "Cleaned: $PUBLISH_PATH"
[ -d "$OUTPUT_PATH" ] && rm -rf "$OUTPUT_PATH" && echo "Cleaned: $OUTPUT_PATH"

# Create output directory
mkdir -p "$OUTPUT_PATH"

# Build and publish with dotnet
echo ""
echo "Building project..."
echo "Project: $PROJECT_FILE"

RID="osx-$ARCH"

dotnet publish "$PROJECT_FILE" \
    -c "$CONFIGURATION" \
    -o "$PUBLISH_PATH" \
    -r "$RID" \
    --self-contained \
    -p:Version="$NEW_VERSION" \
    -p:AssemblyVersion="$ASSEMBLY_VERSION" \
    -p:FileVersion="$FILE_VERSION" \
    -p:InformationalVersion="$NEW_VERSION"

echo "✅ Build completed successfully!"

# Package with Velopack
echo ""
echo "Packaging with Velopack..."

vpk pack \
    --packId "VbdlisTools" \
    --packVersion "$NEW_VERSION" \
    --packDir "$PUBLISH_PATH" \
    --mainExe "Haihv.Vbdlis.Tools.Desktop" \
    --outputDir "$OUTPUT_PATH" \
    --icon "$PROJECT_PATH/Assets/appicon.icns" \
    --packTitle "VBDLIS Tools" \
    --packAuthors "Haihv"

echo ""
echo "✅ Build and packaging completed!"
echo "📦 Output: $OUTPUT_PATH"
echo "🔢 Version: $NEW_VERSION"
echo ""
echo "📝 Files created:"
ls -lh "$OUTPUT_PATH"

echo ""
echo "✨ Done! You can now test the application or create a release."

