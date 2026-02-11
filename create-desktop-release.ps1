# Wrapper script to create Desktop project release
# This script forwards to the Desktop project's release script

param(
    [string]$Version = "",
    [string]$Message = "",
    [string]$AppUpdateNotes = "",
    [string]$AppUpdateNotesFile = ""
)

$DesktopScriptPath = Join-Path $PSScriptRoot "src\Haihv.Vbdlis.Tools\Haihv.Vbdlis.Tools.Desktop\build-scripts\create-release.ps1"

Write-Host "=== Creating Desktop Project Release ===" -ForegroundColor Green
Write-Host "Forwarding to: $DesktopScriptPath" -ForegroundColor Cyan
Write-Host ""

if (Test-Path $DesktopScriptPath) {
    & $DesktopScriptPath `
        -Version $Version `
        -Message $Message `
        -AppUpdateNotes $AppUpdateNotes `
        -AppUpdateNotesFile $AppUpdateNotesFile
}
else {
    Write-Error "Desktop release script not found at: $DesktopScriptPath"
    exit 1
}

