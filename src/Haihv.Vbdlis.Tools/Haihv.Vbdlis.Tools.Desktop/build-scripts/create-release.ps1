# PowerShell script to create a GitHub release using version from local build
# This script:
# 1. Reads version from version.json (created by build-local-windows.ps1 or build-local-macos.sh)
# 2. Creates git tag in format v<version> (matches .github/workflows/release.yml trigger)
# 3. Pushes commits (optional) and tag to GitHub to trigger release workflow
#
# Workflow:
# Step 1: .\build-local-windows.ps1  (builds locally, increments version on Windows)
#    OR:  ./build-local-macos.sh     (builds locally, increments version on macOS)
# Step 2: .\create-release.ps1       (uses that version for release)

param(
    [string]$Version = "",
    [string]$Message = "",
    [string]$AppUpdateNotes = "",
    [string]$AppUpdateNotesFile = ""
)

$ErrorActionPreference = "Stop"

Write-Host "=== VBDLIS Tools Desktop - Create GitHub Release ===" -ForegroundColor Green
Write-Host "This script creates a release using the version from local build scripts" -ForegroundColor Cyan
Write-Host ""

# Read current version from version.json
$ScriptDir = $PSScriptRoot
$VersionFile = Join-Path $ScriptDir "version.json"

if (-not (Test-Path $VersionFile)) {
    Write-Host "❌ version.json not found!" -ForegroundColor Red
    Write-Host "   Please run .\build-local-windows.ps1 first to build and generate version." -ForegroundColor Yellow
    exit 1
}

$VersionContent = Get-Content $VersionFile -Raw | ConvertFrom-Json
$CurrentVersion = $VersionContent.currentVersion

if ([string]::IsNullOrEmpty($CurrentVersion)) {
    Write-Host "❌ No version found in version.json!" -ForegroundColor Red
    Write-Host "   Please run .\build-local-windows.ps1 first to build and generate version." -ForegroundColor Yellow
    exit 1
}

Write-Host "📦 Version from local build: $CurrentVersion" -ForegroundColor Cyan
Write-Host "📅 Last build date: $($VersionContent.lastBuildDate)" -ForegroundColor Cyan
Write-Host "🔢 Build number: $($VersionContent.buildNumber)" -ForegroundColor Cyan
Write-Host ""

# Determine version to use
if ([string]::IsNullOrEmpty($Version)) {
    $UseVersion = Read-Host "Use this version for release? (y/n/custom)"

    if ($UseVersion -eq "n") {
        Write-Host "Aborted." -ForegroundColor Yellow
        exit 0
    }
    elseif ($UseVersion -eq "custom") {
        $Version = Read-Host "Enter custom version (e.g., 1.0.26021102)"
    }
    else {
        $Version = $CurrentVersion
    }
}
else {
    Write-Host "Using provided version: $Version" -ForegroundColor Cyan
}

if ([string]::IsNullOrEmpty($Version)) {
    Write-Host "❌ Version cannot be empty!" -ForegroundColor Red
    exit 1
}

# Read app update notes from file if provided
if (-not [string]::IsNullOrEmpty($AppUpdateNotesFile)) {
    if (-not (Test-Path $AppUpdateNotesFile)) {
        Write-Host "❌ App update notes file not found: $AppUpdateNotesFile" -ForegroundColor Red
        exit 1
    }
    $AppUpdateNotes = Get-Content $AppUpdateNotesFile -Raw
}

# Persist app update notes to version.json (workflow will use this on tag-triggered runs)
if (-not [string]::IsNullOrWhiteSpace($AppUpdateNotes)) {
    if ($VersionContent.PSObject.Properties.Name -contains "appUpdateNotes") {
        $VersionContent.appUpdateNotes = $AppUpdateNotes
    }
    else {
        $VersionContent | Add-Member -NotePropertyName "appUpdateNotes" -NotePropertyValue $AppUpdateNotes
    }
    $VersionContent | ConvertTo-Json -Depth 10 | Set-Content $VersionFile
    Write-Host "📝 Saved APP_UPDATE_NOTES to version.json" -ForegroundColor Cyan
}

# Determine release message
if ([string]::IsNullOrEmpty($Message)) {
    Write-Host "📝 Enter release message:" -ForegroundColor Yellow
    $Message = Read-Host
    if ([string]::IsNullOrEmpty($Message)) {
        $Message = "Release $Version"
    }
}

$TagName = "v$Version"

Write-Host ""
Write-Host "=== Release Configuration ===" -ForegroundColor Cyan
Write-Host "Version: $Version" -ForegroundColor White
Write-Host "Tag: $TagName" -ForegroundColor White
Write-Host "Message: $Message" -ForegroundColor White
Write-Host ""

# Check if tag already exists
$TagExists = $false
try {
    git rev-parse $TagName 2>&1 | Out-Null
    if ($LASTEXITCODE -eq 0) {
        $TagExists = $true
    }
}
catch {
    $TagExists = $false
}

if ($TagExists) {
    Write-Host "⚠️  Tag $TagName already exists!" -ForegroundColor Yellow
    $DeleteChoice = Read-Host "Delete and recreate? (y/n)"
    if ($DeleteChoice -eq "y") {
        Write-Host "Deleting local tag..." -ForegroundColor Yellow
        git tag -d $TagName | Out-Null
        Write-Host "Deleting remote tag..." -ForegroundColor Yellow
        git push origin ":refs/tags/$TagName" 2>&1 | Out-Null
        Write-Host "✅ Old tag deleted (local and remote)" -ForegroundColor Green
    }
    else {
        Write-Host "Aborted." -ForegroundColor Yellow
        exit 0
    }
}

# Check for uncommitted changes
Write-Host ""
Write-Host "Checking git status..." -ForegroundColor Yellow
$Status = git status --porcelain
if ($Status) {
    Write-Host "📋 Uncommitted changes detected." -ForegroundColor Yellow
    $CommitChoice = Read-Host "Commit changes? (y/n)"
    if ($CommitChoice -eq "y") {
        $CommitMsg = Read-Host "Commit message"
        git add .
        git commit -m $CommitMsg
        Write-Host "✅ Changes committed" -ForegroundColor Green
    }
}

# Push commits
Write-Host ""
Write-Host "⬆️  Pushing commits to origin..." -ForegroundColor Yellow
$CurrentBranch = git rev-parse --abbrev-ref HEAD
git push origin $CurrentBranch

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Failed to push commits" -ForegroundColor Red
    exit 1
}

# Create annotated tag
Write-Host ""
Write-Host "🏷️  Creating tag $TagName..." -ForegroundColor Yellow
git tag -a $TagName -m $Message

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Failed to create tag" -ForegroundColor Red
    exit 1
}

# Push tag
Write-Host ""
Write-Host "⬆️  Pushing tag to origin..." -ForegroundColor Yellow
git push origin $TagName

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Failed to push tag" -ForegroundColor Red
    exit 1
}

# Get repo URL
$RepoUrl = git config --get remote.origin.url
$RepoPath = $RepoUrl -replace '.*github.com[:/](.*?)(.git)?$', '$1'

Write-Host ""
Write-Host "✅ Release tag created successfully!" -ForegroundColor Green
Write-Host ""
Write-Host "📺 GitHub Actions will now:" -ForegroundColor Cyan
Write-Host "   1. Trigger release workflow with tag: $TagName" -ForegroundColor White
Write-Host "   2. Build using LOCKED version from version.json: $Version" -ForegroundColor White
Write-Host "   3. Publish release artifacts" -ForegroundColor White
Write-Host ""
Write-Host "🔗 Check progress at:" -ForegroundColor Cyan
Write-Host "   https://github.com/$RepoPath/actions" -ForegroundColor White
Write-Host ""
Write-Host "💡 Notes:" -ForegroundColor Cyan
Write-Host "   - Version source: build-scripts/version.json" -ForegroundColor White
Write-Host "   - Tag format must be: v<version> (matches release workflow trigger)" -ForegroundColor White
Write-Host "   - Run local build first to update version.json" -ForegroundColor White
Write-Host ""
Write-Host "🎉 Done!" -ForegroundColor Green
