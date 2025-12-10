#!/bin/bash
# Script to upload macOS release files to GitHub
# This uploads all necessary files for Velopack auto-update

set -e

if [ -z "$1" ]; then
    echo "Usage: ./upload-macos-release.sh <version>"
    echo "Example: ./upload-macos-release.sh 1.0.25121030"
    exit 1
fi

VERSION="$1"
TAG="v$VERSION"
DIST_DIR="dist/velopack"

echo "=== Uploading macOS Release Files ==="
echo "Version: $VERSION"
echo "Tag: $TAG"
echo ""

# Check if gh CLI is installed
if ! command -v gh &> /dev/null; then
    echo "❌ GitHub CLI (gh) is not installed!"
    echo "Install it with: brew install gh"
    exit 1
fi

# Check if release exists
if ! gh release view "$TAG" &> /dev/null; then
    echo "❌ Release $TAG does not exist!"
    echo "Please wait for GitHub Actions to create the release first."
    exit 1
fi

echo "✅ Release $TAG found"
echo ""

# Files to upload for Velopack auto-update
FILES=(
    "$DIST_DIR/VbdlisTools-$VERSION-osx-arm64.dmg"
    "$DIST_DIR/Haihv.Vbdlis.Tools.Desktop-$VERSION-osx-full.nupkg"
    "$DIST_DIR/Haihv.Vbdlis.Tools.Desktop-osx-Portable.zip"
    "$DIST_DIR/RELEASES-osx"
    "$DIST_DIR/releases.osx.json"
    "$DIST_DIR/assets.osx.json"
)

echo "📦 Files to upload:"
for file in "${FILES[@]}"; do
    if [ -f "$file" ]; then
        SIZE=$(ls -lh "$file" | awk '{print $5}')
        echo "  ✅ $(basename "$file") ($SIZE)"
    else
        echo "  ❌ $(basename "$file") - NOT FOUND"
        exit 1
    fi
done

echo ""
read -p "Continue with upload? (y/n): " CONFIRM

if [ "$CONFIRM" != "y" ]; then
    echo "Aborted."
    exit 0
fi

echo ""
echo "⬆️  Uploading files..."
echo ""

for file in "${FILES[@]}"; do
    filename=$(basename "$file")
    echo "Uploading $filename..."
    
    # Check if file already exists in release
    if gh release view "$TAG" --json assets --jq ".assets[].name" | grep -q "^$filename$"; then
        echo "  ⚠️  File already exists, deleting..."
        gh release delete-asset "$TAG" "$filename" -y 2>/dev/null || true
    fi
    
    gh release upload "$TAG" "$file" --clobber
    echo "  ✅ Uploaded"
done

echo ""
echo "✅ All files uploaded successfully!"
echo ""
echo "📝 Velopack Auto-Update Files:"
echo "   ✅ DMG installer for users"
echo "   ✅ .nupkg update package"
echo "   ✅ RELEASES-osx metadata"
echo "   ✅ releases.osx.json metadata"
echo "   ✅ assets.osx.json metadata"
echo "   ✅ Portable ZIP (optional)"
echo ""
echo "🔗 View release at:"
echo "   https://github.com/$(git config --get remote.origin.url | sed -E 's|.*github\.com[:/](.+)(\.git)?$|\1|' | sed 's|\.git$||')/releases/tag/$TAG"
echo ""
echo "🎉 macOS auto-update is now enabled!"
echo ""
echo "Users can:"
echo "   1. Download and install DMG"
echo "   2. App will auto-check for updates on startup"
echo "   3. Updates download and install automatically"
echo ""
