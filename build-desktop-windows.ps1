# Wrapper script to build Desktop project on Windows
# This script forwards to the Desktop project's build script

param(
    [string]$Configuration = "Release"
)

$DesktopScriptPath = Join-Path $PSScriptRoot "src\Haihv.Vbdlis.Tools\Haihv.Vbdlis.Tools.Desktop\build-scripts\build-local-windows.ps1"

Write-Host "=== Building Desktop Project ===" -ForegroundColor Green
Write-Host "Forwarding to: $DesktopScriptPath" -ForegroundColor Cyan
Write-Host ""

if (Test-Path $DesktopScriptPath) {
    & $DesktopScriptPath -Configuration $Configuration
}
else {
    Write-Error "Desktop build script not found at: $DesktopScriptPath"
    exit 1
}

