param (
    [Parameter(Mandatory=$true)]
    [string]$appImageOutputDir,

    [string]$appDirName = "AtomUIGallery.AppDir",
    [string]$executableName = "AtomUIGallery.Desktop"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

function Invoke-RequiredCommand {
    param(
        [Parameter(Mandatory=$true)]
        [string]$Command,

        [Parameter(Mandatory=$true)]
        [string[]]$Arguments,

        [string]$WorkingDirectory = (Get-Location).Path
    )

    Write-Host "$Command $($Arguments -join ' ')" -ForegroundColor Cyan
    Push-Location $WorkingDirectory
    try {
        & $Command @Arguments
        if ($LASTEXITCODE -ne 0) {
            throw "$Command $($Arguments -join ' ') failed with exit code $LASTEXITCODE."
        }
    }
    finally {
        Pop-Location
    }
}

function Get-SingleAppImage {
    param(
        [Parameter(Mandatory=$true)]
        [string]$OutputDir
    )

    $appImages = @(Get-ChildItem -Path $OutputDir -Filter "*.AppImage" -File)
    if ($appImages.Count -eq 0) {
        throw "No AppImage artifact was found in '$OutputDir'."
    }

    if ($appImages.Count -gt 1) {
        $artifactList = ($appImages | ForEach-Object { $_.FullName }) -join ", "
        throw "Expected exactly one AppImage artifact in '$OutputDir', but found $($appImages.Count): $artifactList"
    }

    return $appImages[0]
}

function Test-AppImageContainsAppRun {
    param(
        [Parameter(Mandatory=$true)]
        [string]$AppImagePath
    )

    $validationRoot = Join-Path -Path ([System.IO.Path]::GetTempPath()) -ChildPath "atomui-appimage-validation-$([System.Guid]::NewGuid().ToString('N'))"
    New-Item -Path $validationRoot -ItemType Directory -Force | Out-Null

    try {
        Invoke-RequiredCommand -Command $AppImagePath -Arguments @("--appimage-extract", "AppRun") -WorkingDirectory $validationRoot

        $extractedAppRunPath = Join-Path -Path $validationRoot -ChildPath "squashfs-root/AppRun"
        if (-not (Test-Path -Path $extractedAppRunPath -PathType Leaf)) {
            throw "AppImage validation failed: '$AppImagePath' does not contain a root AppRun file."
        }

        Invoke-RequiredCommand -Command "sh" -Arguments @("-n", $extractedAppRunPath)
    }
    finally {
        if (Test-Path -Path $validationRoot -PathType Container) {
            Remove-Item -Path $validationRoot -Recurse -Force
        }
    }
}

if (-not $IsLinux) {
    throw "AppImage launcher validation is only supported on Linux."
}

if (-not (Test-Path -Path $appImageOutputDir -PathType Container)) {
    throw "AppImage output directory was not found: $appImageOutputDir"
}

$appDirPath = Join-Path -Path $appImageOutputDir -ChildPath $appDirName
if (-not (Test-Path -Path $appDirPath -PathType Container)) {
    throw "AppImage AppDir was not found: $appDirPath"
}

$appExecutableRelativePath = Join-Path -Path "usr/bin" -ChildPath $executableName
$appExecutablePath = Join-Path -Path $appDirPath -ChildPath $appExecutableRelativePath
if (-not (Test-Path -Path $appExecutablePath -PathType Leaf)) {
    throw "AppImage executable was not found: $appExecutablePath"
}

$appRunPath = Join-Path -Path $appDirPath -ChildPath "AppRun"
$appRunWasCreated = $false
if (-not (Test-Path -Path $appRunPath -PathType Leaf)) {
    $appRunContent = @'
#!/bin/sh
HERE="$(dirname "$(readlink -f "$0")")"
export LD_LIBRARY_PATH="$HERE/usr/lib${LD_LIBRARY_PATH:+:$LD_LIBRARY_PATH}"
exec "$HERE/usr/bin/__EXECUTABLE_NAME__" "$@"
'@.Replace("__EXECUTABLE_NAME__", $executableName)

    Set-Content -Path $appRunPath -Value $appRunContent -Encoding utf8NoBOM
    $appRunWasCreated = $true
    Write-Host "Created missing AppImage AppRun: $appRunPath" -ForegroundColor Yellow
}
else {
    Write-Host "AppImage AppRun already exists: $appRunPath" -ForegroundColor Green
}

Invoke-RequiredCommand -Command "chmod" -Arguments @("+x", $appRunPath)
Invoke-RequiredCommand -Command "sh" -Arguments @("-n", $appRunPath)

$appImage = Get-SingleAppImage -OutputDir $appImageOutputDir
if ($appRunWasCreated) {
    $appImageFileName = $appImage.Name
    $appImagePath = $appImage.FullName
    Remove-Item -Path $appImagePath -Force

    Invoke-RequiredCommand -Command "appimagetool" -Arguments @($appDirPath, $appImageFileName) -WorkingDirectory $appImageOutputDir
    $appImage = Get-SingleAppImage -OutputDir $appImageOutputDir
}

Test-AppImageContainsAppRun -AppImagePath $appImage.FullName
Write-Host "AppImage launcher validation passed: $($appImage.FullName)" -ForegroundColor Green
