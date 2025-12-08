# Simple build script for all platforms
# Use this if you just want the published files without packaging

param(
    [string]$Platform = "all",  # windows, macos-x64, macos-arm64, or all
    [string]$Configuration = "Release",
    [string]$Version = "1.0.0"
)

$ErrorActionPreference = "Stop"

$ProjectPath = Join-Path $PSScriptRoot "..\src\Haihv.Vbdlis.Tools\Haihv.Vbdlis.Tools.Desktop"
$DistPath = Join-Path $PSScriptRoot "..\dist"

function Build-Platform {
    param(
        [string]$Runtime,
        [string]$PlatformName
    )

    Write-Host "`n=== Building for $PlatformName ===" -ForegroundColor Green

    $OutputPath = Join-Path $DistPath $PlatformName

    # Clean
    if (Test-Path $OutputPath) {
        Remove-Item -Path $OutputPath -Recurse -Force
    }
    New-Item -ItemType Directory -Path $OutputPath -Force | Out-Null

    # Publish
    dotnet publish $ProjectPath `
        --configuration $Configuration `
        --runtime $Runtime `
        --self-contained true `
        --output $OutputPath `
        -p:PublishSingleFile=false `
        -p:PublishTrimmed=false `
        -p:Version=$Version

    if ($LASTEXITCODE -ne 0) {
        Write-Host "Build failed for $PlatformName!" -ForegroundColor Red
        return $false
    }

    # Remove Playwright browsers
    $PlaywrightPath = Join-Path $OutputPath ".playwright"
    if (Test-Path $PlaywrightPath) {
        Remove-Item -Path $PlaywrightPath -Recurse -Force
    }

    Write-Host "Build completed: $OutputPath" -ForegroundColor Cyan
    return $true
}

Write-Host "=== VBDLIS Tools - Simple Build ===" -ForegroundColor Green
Write-Host "Platform: $Platform" -ForegroundColor Cyan
Write-Host "Configuration: $Configuration" -ForegroundColor Cyan
Write-Host "Version: $Version" -ForegroundColor Cyan

$success = $true

if ($Platform -eq "windows" -or $Platform -eq "all") {
    $success = $success -and (Build-Platform "win-x64" "windows-x64")
}

if ($Platform -eq "macos-x64" -or $Platform -eq "all") {
    $success = $success -and (Build-Platform "osx-x64" "macos-x64")
}

if ($Platform -eq "macos-arm64" -or $Platform -eq "all") {
    $success = $success -and (Build-Platform "osx-arm64" "macos-arm64")
}

if ($success) {
    Write-Host "`n=== All Builds Completed Successfully ===" -ForegroundColor Green
    Write-Host "Output folder: $DistPath" -ForegroundColor Cyan
    Write-Host "`nNote: Playwright browsers are NOT included." -ForegroundColor Yellow
} else {
    Write-Host "`n=== Build Failed ===" -ForegroundColor Red
    exit 1
}
