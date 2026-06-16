param (
    [string]$publishRootPath = "D:/publish",
    [string]$buildType = "Release",
    [string]$framework = "net10.0",
    [string]$runtime = "osx-arm64",
    [string]$buildTimestamp,
    [object]$publishAot = $true
)

if ([string]::IsNullOrEmpty($buildTimestamp)) {
    $buildTimestamp = Get-Date -Format 'yyyyMMddHHmm'
}

if (Test-Path -Path $publishRootPath -PathType Container) {
    Remove-Item -Path $publishRootPath -Recurse -Force
}

$configPath = Join-Path -Path $publishRootPath -ChildPath "config"
$packagesPath = Join-Path -Path $publishRootPath -ChildPath "packages"

New-Item -Path $publishRootPath -ItemType Directory -Force | Out-Null
New-Item -Path $configPath -ItemType Directory -Force | Out-Null
New-Item -Path $packagesPath -ItemType Directory -Force | Out-Null

function Update-VersionInFile {
    param(
        [Parameter(Mandatory=$true)]
        [string]$XmlFilePath,

        [Parameter(Mandatory=$true)]
        [string]$TargetFilePath
    )

    try {
        [xml]$xmlContent = Get-Content -Path $XmlFilePath -ErrorAction Stop
        
        $version = $xmlContent.Project.PropertyGroup.AtomUIGalleryVersion

        if (-not $version) {
            throw "AtomUIGalleryVersion not found in XML file"
        }

        Write-Host "Extracted version: $version" -ForegroundColor Green
        
        $targetContent = Get-Content -Path $TargetFilePath -Raw -ErrorAction Stop
        $updatedContent = $targetContent -replace '__VERSION__', $version
        
        $updatedContent | Set-Content -Path $TargetFilePath -Force

        Write-Host "Successfully updated $TargetFilePath with version $version" -ForegroundColor Cyan
    }
    catch {
        Write-Error "Error occurred: $($_.Exception.Message)"
        return $false
    }

    return $true
}

function ConvertTo-Bool {
    param(
        [Parameter(Mandatory=$true)]
        [object]$Value
    )

    if ($Value -is [bool]) {
        return $Value
    }

    $normalizedValue = ([string]$Value).Trim().ToLowerInvariant()
    switch ($normalizedValue) {
        "true" { return $true }
        "1" { return $true }
        "yes" { return $true }
        "false" { return $false }
        "0" { return $false }
        "no" { return $false }
        default {
            throw "Invalid boolean value '$Value'. Expected true or false."
        }
    }
}

function Invoke-DotNet {
    param(
        [Parameter(Mandatory=$true)]
        [string[]]$Arguments
    )

    Write-Host "dotnet $($Arguments -join ' ')" -ForegroundColor Cyan
    & dotnet @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet $($Arguments -join ' ') failed with exit code $LASTEXITCODE."
    }
}

function Test-NativeAotRestoreAssets {
    param(
        [Parameter(Mandatory=$true)]
        [string]$AssetsPath,

        [Parameter(Mandatory=$true)]
        [string]$Runtime
    )

    if (-not (Test-Path -Path $AssetsPath -PathType Leaf)) {
        throw "NativeAOT restore validation failed: assets file '$AssetsPath' was not found."
    }

    $assetsContent = Get-Content -Path $AssetsPath -Raw
    $requiredEntries = @(
        "Microsoft.DotNet.ILCompiler",
        "runtime.$Runtime.Microsoft.DotNet.ILCompiler"
    )

    foreach ($entry in $requiredEntries) {
        if (-not $assetsContent.Contains($entry)) {
            throw "NativeAOT restore validation failed for runtime '$Runtime': '$entry' was not found in '$AssetsPath'."
        }
    }

    Write-Host "NativeAOT restore assets validation passed for runtime '$Runtime'." -ForegroundColor Green
}

function Test-NativeAotOutput {
    param(
        [Parameter(Mandatory=$true)]
        [string]$PackagesPath,

        [Parameter(Mandatory=$true)]
        [string]$Runtime
    )

    $mainExecutable = if ($Runtime.StartsWith("win-")) {
        "AtomUIGallery.Desktop.exe"
    } else {
        "AtomUIGallery.Desktop"
    }

    $mainExecutablePath = Join-Path -Path $PackagesPath -ChildPath $mainExecutable
    if (-not (Test-Path -Path $mainExecutablePath -PathType Leaf)) {
        throw "NativeAOT validation failed for runtime '$Runtime': main executable '$mainExecutable' was not found in '$PackagesPath'."
    }

    $forbiddenFiles = @(
        "coreclr.dll",
        "clrjit.dll",
        "libcoreclr.so",
        "libcoreclr.dylib",
        "libclrjit.so",
        "libclrjit.dylib",
        "System.Private.CoreLib.dll",
        "AtomUIGallery.Desktop.dll"
    )

    foreach ($fileName in $forbiddenFiles) {
        $candidate = Join-Path -Path $PackagesPath -ChildPath $fileName
        if (Test-Path -Path $candidate -PathType Leaf) {
            throw "NativeAOT validation failed for runtime '$Runtime': unexpected self-contained runtime file '$fileName' was found in '$PackagesPath'."
        }
    }

    Write-Host "NativeAOT output validation passed for runtime '$Runtime'." -ForegroundColor Green
}

$publishAotEnabled = ConvertTo-Bool -Value $publishAot
$projectPath = Join-Path -Path $PSScriptRoot -ChildPath "../AtomUIGallery.Desktop.csproj"
$assetsPath = Join-Path -Path $PSScriptRoot -ChildPath "../../../output/AtomUIGallery.Desktop/obj/project.assets.json"

if ($publishAotEnabled) {
    if ($buildType -ne "Release") {
        throw "NativeAOT Gallery publishing requires buildType 'Release'. Current buildType is '$buildType'."
    }

    Invoke-DotNet -Arguments @(
        "restore",
        $projectPath,
        "--runtime",
        $runtime,
        "-p:Configuration=$buildType",
        "-p:PublishAot=true",
        "--disable-parallel",
        "-m:1",
        "/nr:false",
        "--nologo",
        "-v:minimal"
    )

    Test-NativeAotRestoreAssets -AssetsPath $assetsPath -Runtime $runtime

    Invoke-DotNet -Arguments @(
        "publish",
        $projectPath,
        "--output",
        $packagesPath,
        "--self-contained",
        "true",
        "--framework",
        $framework,
        "--runtime",
        $runtime,
        "--configuration",
        $buildType,
        "--no-restore",
        "-p:PublishAot=true",
        "--nologo",
        "-v:minimal"
    )

    Test-NativeAotOutput -PackagesPath $packagesPath -Runtime $runtime
} else {
    Invoke-DotNet -Arguments @(
        "publish",
        $projectPath,
        "--output",
        $packagesPath,
        "--self-contained",
        "--framework",
        $framework,
        "--runtime",
        $runtime,
        "--configuration",
        $buildType,
        "-p:PublishSingleFile=true"
    )
}

if ($IsMacOS) {
    Copy-Item -Path ../configs/InstallerConfig.dmg.xml -Destination $configPath/InstallerConfig.xml -Force
} elseif ($IsWindows) {
    Copy-Item -Path ../configs/InstallerConfig.wix.xml -Destination $configPath/InstallerConfig.xml -Force
} elseif ($IsLinux) {
    Copy-Item -Path ../configs/InstallerConfig.appimage.xml -Destination $configPath/InstallerConfig.xml -Force
}
Update-VersionInFile -XmlFilePath "../../../build/Version.props" -TargetFilePath "$configPath/InstallerConfig.xml"
