# Script to build and package Windows installer
# Requires: .NET 10.0 SDK

param(
    [string]$Configuration = "Release",
    [string]$Version = "1.0.0"
)

$ErrorActionPreference = "Stop"

Write-Host "=== Building VBDLIS Tools for Windows ===" -ForegroundColor Green
Write-Host "Configuration: $Configuration" -ForegroundColor Cyan
Write-Host "Version: $Version" -ForegroundColor Cyan

# Paths
$ProjectPath = Join-Path $PSScriptRoot "..\src\Haihv.Vbdlis.Tools\Haihv.Vbdlis.Tools.Desktop"
$PublishPath = Join-Path $ProjectPath "bin\publish\win-x64"
$OutputPath = Join-Path $PSScriptRoot "..\dist\windows"

# Clean previous builds
Write-Host "`nCleaning previous builds..." -ForegroundColor Yellow
if (Test-Path $PublishPath) {
    Remove-Item -Path $PublishPath -Recurse -Force
}
if (Test-Path $OutputPath) {
    Remove-Item -Path $OutputPath -Recurse -Force
}
New-Item -ItemType Directory -Path $OutputPath -Force | Out-Null

# Build and publish
Write-Host "`nPublishing application..." -ForegroundColor Yellow
dotnet publish $ProjectPath `
    --configuration $Configuration `
    --runtime win-x64 `
    --self-contained true `
    --output $PublishPath `
    -p:PublishReadyToRun=true `
    -p:PublishSingleFile=false `
    -p:PublishTrimmed=false `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -p:Version=$Version

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}

# Remove Playwright browsers if they exist (they will be downloaded on first run)
Write-Host "`nRemoving Playwright browsers from output (will be downloaded on first run)..." -ForegroundColor Yellow
$PlaywrightBrowsersPath = Join-Path $PublishPath ".playwright"
if (Test-Path $PlaywrightBrowsersPath) {
    Remove-Item -Path $PlaywrightBrowsersPath -Recurse -Force
    Write-Host "Removed Playwright browsers folder" -ForegroundColor Green
}

# Copy to dist folder
Write-Host "`nCopying to dist folder..." -ForegroundColor Yellow
Copy-Item -Path "$PublishPath\*" -Destination $OutputPath -Recurse -Force

# Create ZIP package
Write-Host "`nCreating ZIP package..." -ForegroundColor Yellow
$ZipFileName = "VbdlisTools-Windows-x64-v$Version.zip"
$ZipPath = Join-Path (Split-Path $OutputPath -Parent) $ZipFileName

if (Test-Path $ZipPath) {
    Remove-Item -Path $ZipPath -Force
}

Compress-Archive -Path "$OutputPath\*" -DestinationPath $ZipPath -CompressionLevel Optimal

Write-Host "`n=== Build Completed Successfully ===" -ForegroundColor Green
Write-Host "Output folder: $OutputPath" -ForegroundColor Cyan
Write-Host "ZIP package: $ZipPath" -ForegroundColor Cyan
Write-Host "`nNote: Playwright browsers are NOT included. They will be downloaded automatically on first run." -ForegroundColor Yellow
