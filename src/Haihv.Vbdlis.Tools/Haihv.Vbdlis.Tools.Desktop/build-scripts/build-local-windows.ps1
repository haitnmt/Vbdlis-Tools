# Script to build Desktop project locally with auto-incrementing version (Windows)
# This script builds the application for LOCAL TESTING
# - Auto-increments version based on date and build number
# - Updates version.json with new version
# - Use this for development and testing
#
# For RELEASE builds, use: .\create-release.ps1

param(
    [string]$Configuration = "Release"
)

$ErrorActionPreference = "Stop"

Write-Host "=== Building VBDLIS Tools Desktop LOCALLY with Velopack (Windows) ===" -ForegroundColor Green
Write-Host "Configuration: $Configuration" -ForegroundColor Cyan
Write-Host "Build Mode: LOCAL (auto-increment version)" -ForegroundColor Yellow
Write-Host ""

# Check if Velopack is installed
Write-Host "Checking for Velopack CLI..." -ForegroundColor Yellow
try {
    $null = vpk --version 2>&1
    Write-Host "Velopack CLI found!" -ForegroundColor Green
}
catch {
    Write-Host "Velopack CLI not found. Installing..." -ForegroundColor Yellow
    dotnet tool install --global vpk
}

# Paths - relative to this script
$ScriptRoot = $PSScriptRoot
$ProjectPath = Split-Path $ScriptRoot -Parent
$ProjectFile = Join-Path $ProjectPath "Haihv.Vbdlis.Tools.Desktop.csproj"
$PublishPath = Join-Path $ProjectPath "bin\publish\velopack"
$OutputPath = Join-Path $ProjectPath "dist\velopack"
$VersionLogFile = Join-Path $ProjectPath "build-scripts\version.json"

# Read or create version log
Write-Host "`nReading version log..." -ForegroundColor Yellow
$versionLog = $null
if (Test-Path $VersionLogFile) {
    $versionLog = Get-Content $VersionLogFile -Raw | ConvertFrom-Json
    Write-Host "Found existing version log" -ForegroundColor Green
}
else {
    Write-Host "Creating new version log" -ForegroundColor Yellow
    $versionLog = @{
        majorMinor      = "1.0"
        currentVersion  = ""
        assemblyVersion = ""
        buildNumber     = 0
        lastBuildDate   = ""
        dateCode        = ""
        history         = @()
    }
}

# Get current date info
$today = Get-Date
$dateYY = $today.ToString("yy")
$dateMM = $today.ToString("MM")
$dateDD = $today.ToString("dd")
$dateCode = "$dateYY$dateMM"
$currentDateStr = $today.ToString("yyyy-MM-dd")

# Version increment logic
if ($versionLog.lastBuildDate -eq $currentDateStr) {
    # Same day, increment build number
    $versionLog.buildNumber++
    Write-Host "Same day build detected. Incrementing build number to: $($versionLog.buildNumber)" -ForegroundColor Cyan
}
else {
    # New day, reset build number
    Write-Host "New day detected. Resetting build number." -ForegroundColor Cyan
    $versionLog.buildNumber = 1
    $versionLog.lastBuildDate = $currentDateStr
    $versionLog.dateCode = $dateCode
}

# Format build number to 2 digits (01, 02, ..., 99)
$buildNumberPadded = $versionLog.buildNumber.ToString("00")

# Build version strings:
# - PackageVersion (SemVer2 - 3 parts): Major.Minor.yyMMDDBB (for Velopack)
# - AssemblyVersion (4 parts): Major.Minor.yyMM.DDBB (for .NET)
# Example: Package=1.0.26021102, Assembly=1.0.2602.1102 (Feb 11, 2026, build 02)
$patchVersion = "$dateYY$dateMM$dateDD$buildNumberPadded"
$newVersion = "$($versionLog.majorMinor).$patchVersion"
$assemblyVersion = "$($versionLog.majorMinor).$dateYY$dateMM.$dateDD$buildNumberPadded"
$fileVersion = $assemblyVersion

Write-Host "`n📦 Version Information:" -ForegroundColor Green
Write-Host "   Package Version: $newVersion (for Velopack)" -ForegroundColor Cyan
Write-Host "   Assembly Version: $assemblyVersion (for .NET)" -ForegroundColor Cyan
Write-Host "   File Version: $fileVersion" -ForegroundColor Cyan
Write-Host "   Date: $dateYY$dateMM$dateDD" -ForegroundColor Cyan
Write-Host "   Build Number: $buildNumberPadded" -ForegroundColor Cyan
Write-Host "   Build Number: $($versionLog.buildNumber)" -ForegroundColor Cyan
Write-Host ""

# Update version log
$versionLog.currentVersion = $newVersion
$versionLog.assemblyVersion = $assemblyVersion

# Add to history
$historyEntry = @{
    version   = $newVersion
    date      = $currentDateStr
    timestamp = $today.ToString("yyyy-MM-dd HH:mm:ss")
}
if (-not $versionLog.history) {
    $versionLog.history = @()
}
$versionLog.history += $historyEntry

# Save version log
$versionLog | ConvertTo-Json -Depth 10 | Set-Content $VersionLogFile
Write-Host "✅ Updated version log: $VersionLogFile" -ForegroundColor Green

# Clean previous build outputs
Write-Host "`nCleaning previous builds..." -ForegroundColor Yellow
if (Test-Path $PublishPath) {
    Remove-Item $PublishPath -Recurse -Force
    Write-Host "Cleaned: $PublishPath" -ForegroundColor Green
}
if (Test-Path $OutputPath) {
    Remove-Item $OutputPath -Recurse -Force
    Write-Host "Cleaned: $OutputPath" -ForegroundColor Green
}

# Create output directory
New-Item -ItemType Directory -Path $OutputPath -Force | Out-Null

# Build and publish with dotnet
Write-Host "`nBuilding project..." -ForegroundColor Yellow
Write-Host "Project: $ProjectFile" -ForegroundColor Cyan

dotnet publish $ProjectFile `
    -c $Configuration `
    -o $PublishPath `
    -r win-x64 `
    --self-contained `
    -p:Version=$newVersion `
    -p:AssemblyVersion=$assemblyVersion `
    -p:FileVersion=$fileVersion `
    -p:InformationalVersion=$newVersion

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build failed!" -ForegroundColor Red
    exit 1
}

Write-Host "✅ Build completed successfully!" -ForegroundColor Green

# Package with Velopack
Write-Host "`nPackaging with Velopack..." -ForegroundColor Yellow

vpk pack `
    --packId "VbdlisTools" `
    --packVersion $newVersion `
    --packDir $PublishPath `
    --mainExe "Haihv.Vbdlis.Tools.Desktop.exe" `
    --outputDir $OutputPath `
    --icon "$ProjectPath\Assets\appicon.ico" `
    --packTitle "VBDLIS Tools" `
    --packAuthors "Haihv"

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Packaging failed!" -ForegroundColor Red
    exit 1
}

Write-Host "`n✅ Build and packaging completed!" -ForegroundColor Green
Write-Host "📦 Output: $OutputPath" -ForegroundColor Cyan
Write-Host "🔢 Version: $newVersion" -ForegroundColor Cyan
Write-Host "`n📝 Files created:" -ForegroundColor Yellow
Get-ChildItem $OutputPath | ForEach-Object {
    Write-Host "   - $($_.Name)" -ForegroundColor Cyan
}

Write-Host "`n✨ Done! You can now test the application or create a release." -ForegroundColor Green

