# Script to prepare and lock version for release
# This ensures both Windows and macOS builds use the same version number

param(
    [string]$Version = "",
    [switch]$Force
)

$ErrorActionPreference = "Stop"

Write-Host "=== Prepare Release - Lock Version ===" -ForegroundColor Green
Write-Host ""

$ScriptDir = $PSScriptRoot
$VersionFile = Join-Path $ScriptDir "build\version.json"

# Read current version.json
if (-not (Test-Path $VersionFile)) {
    Write-Host "❌ version.json not found!" -ForegroundColor Red
    exit 1
}

$VersionContent = Get-Content $VersionFile -Raw | ConvertFrom-Json
$CurrentVersion = $VersionContent.currentVersion
$CurrentBuildNumber = $VersionContent.buildNumber

Write-Host "📦 Current version: $CurrentVersion" -ForegroundColor Cyan
Write-Host "📦 Current build number: $CurrentBuildNumber" -ForegroundColor Cyan
Write-Host ""

# Determine version to lock
if ([string]::IsNullOrEmpty($Version)) {
    $Choice = Read-Host "Lock this version for release? (y/n/custom)"
    
    if ($Choice -eq "n") {
        Write-Host "Aborted." -ForegroundColor Yellow
        exit 0
    }
    elseif ($Choice -eq "custom") {
        $Version = Read-Host "Enter version to lock (e.g., 1.0.25120906)"
    }
    else {
        $Version = $CurrentVersion
    }
}

Write-Host ""
Write-Host "🔒 Locking version: $Version" -ForegroundColor Yellow
Write-Host ""

# Parse version to get build number
if ($Version -match '\.(\d{2})$') {
    $BuildNumber = [int]$Matches[1]
}
else {
    Write-Host "❌ Invalid version format. Expected: Major.Minor.YYMMDDBB" -ForegroundColor Red
    exit 1
}

# Extract date from version
if ($Version -match '\.(\d{6})\d{2}$') {
    $DatePart = $Matches[1]
    $Year = "20" + $DatePart.Substring(0, 2)
    $Month = $DatePart.Substring(2, 2)
    $Day = $DatePart.Substring(4, 2)
    $LockDate = "$Year-$Month-$Day"
}
else {
    $LockDate = Get-Date -Format "yyyy-MM-dd"
}

# Calculate assembly version
if ($Version -match '^(\d+\.\d+)\.(\d+)$') {
    $MajorMinor = $Matches[1]
    $PatchNumber = $Matches[2]
    
    # Extract YYMM and DDBB
    $YearMonth = $PatchNumber.Substring(0, 4)
    $DayBuild = $PatchNumber.Substring(4, 4)
    
    $AssemblyVersion = "$MajorMinor.$YearMonth.$DayBuild"
}
else {
    Write-Host "❌ Cannot parse version format" -ForegroundColor Red
    exit 1
}

Write-Host "Assembly Version: $AssemblyVersion" -ForegroundColor Cyan
Write-Host "Package Version:  $Version" -ForegroundColor Cyan
Write-Host "Build Number:     $BuildNumber" -ForegroundColor Cyan
Write-Host "Lock Date:        $LockDate" -ForegroundColor Cyan
Write-Host ""

# Confirm
if (-not $Force) {
    $Confirm = Read-Host "Proceed with locking this version? (y/n)"
    if ($Confirm -ne "y") {
        Write-Host "Aborted." -ForegroundColor Yellow
        exit 0
    }
}

# Update version.json with locked version
$VersionContent.majorMinor = $MajorMinor
$VersionContent.currentVersion = $Version
$VersionContent.assemblyVersion = $AssemblyVersion
$VersionContent.lastBuildDate = $LockDate
$VersionContent.buildNumber = $BuildNumber

# Save version.json
$VersionContent | ConvertTo-Json -Depth 10 | Set-Content $VersionFile -Encoding UTF8

Write-Host "✅ Version locked in version.json" -ForegroundColor Green
Write-Host ""

# Update .csproj
$ProjectFile = Join-Path $ScriptDir "src\Haihv.Vbdlis.Tools\Haihv.Vbdlis.Tools.Desktop\Haihv.Vbdlis.Tools.Desktop.csproj"
if (Test-Path $ProjectFile) {
    Write-Host "📝 Updating .csproj..." -ForegroundColor Yellow
    
    $CsprojContent = Get-Content $ProjectFile -Raw
    $CsprojContent = $CsprojContent -replace '<AssemblyVersion>[\d\.]+</AssemblyVersion>', "<AssemblyVersion>$AssemblyVersion</AssemblyVersion>"
    $CsprojContent = $CsprojContent -replace '<FileVersion>[\d\.]+</FileVersion>', "<FileVersion>$AssemblyVersion</FileVersion>"
    $CsprojContent = $CsprojContent -replace '<Version>[\d\.]+</Version>', "<Version>$AssemblyVersion</Version>"
    Set-Content -Path $ProjectFile -Value $CsprojContent -NoNewline
    
    Write-Host "✅ .csproj updated" -ForegroundColor Green
}

Write-Host ""
Write-Host "🎯 Next steps:" -ForegroundColor Yellow
Write-Host "   1. Build Windows: .\build\windows-velopack.ps1" -ForegroundColor White
Write-Host "   2. Build macOS:   ./build/macos.sh Release arm64" -ForegroundColor White
Write-Host "   3. Both will use version: $Version" -ForegroundColor Cyan
Write-Host "   4. Create release: .\create-release.ps1" -ForegroundColor White
Write-Host ""
Write-Host "💡 Tip: Commit version.json before building" -ForegroundColor Yellow
Write-Host ""
Write-Host "✅ Done!" -ForegroundColor Green
