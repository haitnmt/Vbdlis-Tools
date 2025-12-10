#!/bin/bash
# Script to create a GitHub release using the version from local build
# This script:
# 1. Reads version from version.json (created by build-local.ps1 or build-local-macos.sh)
# 2. Creates a git tag
# 3. Pushes tag to GitHub to trigger automated release workflow
#
# Workflow:
# Step 1: .\build-local.ps1        (builds locally, increments version on Windows)
#    OR:  ./build-local-macos.sh   (builds locally, increments version on macOS)
# Step 2: ./create-release-macos.sh (uses that version for release)
#
# Note: GitHub Actions will build Windows ONLY using the LOCKED version from version.json

set -e

VERSION=""
MESSAGE=""

# Parse arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        -v|--version)
            VERSION="$2"
            shift 2
            ;;
        -m|--message)
            MESSAGE="$2"
            shift 2
            ;;
        *)
            shift
            ;;
    esac
done

echo "=== VBDLIS Tools - Create GitHub Release ==="
echo "This script creates a release using the version from build-local-macos.sh"
echo ""

# Read current version from version.json
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
VERSION_FILE="$SCRIPT_DIR/build/version.json"

if [ ! -f "$VERSION_FILE" ]; then
    echo "❌ version.json not found!"
    echo "   Please run ./build-local-macos.sh first to build and generate version."
    exit 1
fi

# Read version from JSON
CURRENT_VERSION=$(grep -o '"currentVersion"[[:space:]]*:[[:space:]]*"[^"]*"' "$VERSION_FILE" | cut -d'"' -f4)
LAST_BUILD_DATE=$(grep -o '"lastBuildDate"[[:space:]]*:[[:space:]]*"[^"]*"' "$VERSION_FILE" | cut -d'"' -f4)
BUILD_NUMBER=$(grep -o '"buildNumber"[[:space:]]*:[[:space:]]*[0-9]*' "$VERSION_FILE" | grep -o '[0-9]*$')

if [ -z "$CURRENT_VERSION" ]; then
    echo "❌ No version found in version.json!"
    echo "   Please run ./build-local-macos.sh first to build and generate version."
    exit 1
fi

echo "📦 Version from local build: $CURRENT_VERSION"
echo "📅 Last build date: $LAST_BUILD_DATE"
echo "🔢 Build number: $BUILD_NUMBER"
echo ""

# Determine version to use
if [ -z "$VERSION" ]; then
    read -p "Use this version for release? (y/n/custom): " USE_VERSION
    
    if [ "$USE_VERSION" = "n" ]; then
        echo "Aborted."
        exit 0
    elif [ "$USE_VERSION" = "custom" ]; then
        read -p "Enter custom version (e.g., 1.0.25120906): " VERSION
    else
        VERSION="$CURRENT_VERSION"
    fi
else
    echo "Using provided version: $VERSION"
fi

TAG_NAME="v$VERSION"

echo ""
echo "🏷️  Creating release tag: $TAG_NAME"
echo ""

# Check if tag already exists
TAG_EXISTS=false
if git rev-parse "$TAG_NAME" >/dev/null 2>&1; then
    TAG_EXISTS=true
fi

if [ "$TAG_EXISTS" = true ]; then
    echo "⚠️  Tag $TAG_NAME already exists!"
    read -p "Delete and recreate? (y/n): " DELETE_CHOICE
    if [ "$DELETE_CHOICE" = "y" ]; then
        echo "Deleting local tag..."
        git tag -d "$TAG_NAME" >/dev/null
        echo "Deleting remote tag..."
        git push origin ":refs/tags/$TAG_NAME" 2>&1 >/dev/null || true
        echo "✅ Old tag deleted (local and remote)"
    else
        echo "Aborted."
        exit 0
    fi
fi

# Get release notes
if [ -z "$MESSAGE" ]; then
    read -p "📝 Enter release message: " MESSAGE
    if [ -z "$MESSAGE" ]; then
        MESSAGE="Release $VERSION"
    fi
fi

# Check for uncommitted changes
STATUS=$(git status --porcelain)
if [ -n "$STATUS" ]; then
    echo ""
    echo "📋 Uncommitted changes detected."
    read -p "Commit changes? (y/n): " COMMIT_CHOICE
    if [ "$COMMIT_CHOICE" = "y" ]; then
        read -p "Commit message: " COMMIT_MSG
        git add .
        git commit -m "$COMMIT_MSG"
        echo "✅ Changes committed"
    fi
fi

# Push commits
echo ""
echo "⬆️  Pushing commits to origin..."
CURRENT_BRANCH=$(git rev-parse --abbrev-ref HEAD)
git push origin "$CURRENT_BRANCH"

if [ $? -ne 0 ]; then
    echo "❌ Failed to push commits"
    exit 1
fi

# Create annotated tag
echo ""
echo "🏷️  Creating tag $TAG_NAME..."
git tag -a "$TAG_NAME" -m "$MESSAGE"

if [ $? -ne 0 ]; then
    echo "❌ Failed to create tag"
    exit 1
fi

# Push tag
echo ""
echo "⬆️  Pushing tag to origin..."
git push origin "$TAG_NAME"

if [ $? -ne 0 ]; then
    echo "❌ Failed to push tag"
    exit 1
fi

# Get repo URL
REPO_URL=$(git config --get remote.origin.url)
REPO_PATH=$(echo "$REPO_URL" | sed -E 's|.*github\.com[:/](.+)(\.git)?$|\1|' | sed 's|\.git$||')

echo ""
echo "✅ Release tag created successfully!"
echo ""
echo "📺 GitHub Actions will now:"
echo "   1. Build Windows ONLY with LOCKED version: $VERSION"
echo "   2. Use version from version.json (NO auto-increment)"
echo "   3. Create GitHub Release with Windows artifacts"
echo ""
echo "🔗 Check progress at:"
echo "   https://github.com/$REPO_PATH/actions"
echo ""
echo "⏱️  Build will take approximately 5-10 minutes"
echo ""
echo "💡 Workflow:"
echo "   Local build (.\build-local.ps1 or ./build-local-macos.sh)"
echo "   → Auto-increments version → Updates version.json"
echo "   GitHub Actions (.\create-release.ps1 or ./create-release-macos.sh)"
echo "   → Uses LOCKED version from version.json → Builds Windows ONLY"
echo ""
echo "📝 Note:"
echo "   - macOS builds should be done locally and manually uploaded"
echo "   - Only Windows builds on GitHub Actions"
echo ""
echo "📦 To upload macOS files (RECOMMENDED - includes all auto-update files):"
echo "   ./upload-macos-release.sh $VERSION"
echo ""
echo "   This will upload:"
echo "   ✅ DMG installer"
echo "   ✅ .nupkg update package"
echo "   ✅ RELEASES-osx metadata"
echo "   ✅ releases.osx.json metadata"
echo "   ✅ assets.osx.json metadata"
echo ""
echo "   Or manually:"
echo "   gh release upload v$VERSION \\"
echo "     dist/velopack/VbdlisTools-$VERSION-osx-arm64.dmg \\"
echo "     dist/velopack/Haihv.Vbdlis.Tools.Desktop-$VERSION-osx-full.nupkg \\"
echo "     dist/velopack/RELEASES-osx \\"
echo "     dist/velopack/releases.osx.json \\"
echo "     dist/velopack/assets.osx.json"
echo ""
echo "🎉 Done!"
