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

    $assetsContent = Get-Content -Path $AssetsPath -Raw | ConvertFrom-Json -Depth 100
    $libraryNames = @($assetsContent.libraries.PSObject.Properties.Name)
    $targetProperties = @($assetsContent.targets.PSObject.Properties)
    $runtimeTarget = $targetProperties | Where-Object { $_.Name -like "*/$Runtime" } | Select-Object -First 1

    if (-not ($libraryNames | Where-Object { $_ -like "Microsoft.DotNet.ILCompiler/*" })) {
        throw "NativeAOT restore validation failed for runtime '$Runtime': Microsoft.DotNet.ILCompiler was not found in '$AssetsPath'."
    }

    if (-not $runtimeTarget) {
        throw "NativeAOT restore validation failed for runtime '$Runtime': no runtime-specific target was found in '$AssetsPath'."
    }

    $runtimeTargetLibraryNames = @($runtimeTarget.Value.PSObject.Properties.Name)
    if (-not ($runtimeTargetLibraryNames | Where-Object { $_ -like "Microsoft.DotNet.ILCompiler/*" })) {
        throw "NativeAOT restore validation failed for runtime '$Runtime': Microsoft.DotNet.ILCompiler was not found in target '$($runtimeTarget.Name)' in '$AssetsPath'."
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

function Copy-InstallerAssets {
    param(
        [Parameter(Mandatory=$true)]
        [string]$PackagesPath
    )

    $installerAssetsPath = Join-Path -Path $PSScriptRoot -ChildPath "../Assets"
    $targetAssetsPath = Join-Path -Path $PackagesPath -ChildPath "Assets"

    if (-not (Test-Path -Path $installerAssetsPath -PathType Container)) {
        throw "Installer assets directory was not found: $installerAssetsPath"
    }

    if (Test-Path -Path $targetAssetsPath -PathType Container) {
        Remove-Item -Path $targetAssetsPath -Recurse -Force
    }

    New-Item -Path $targetAssetsPath -ItemType Directory -Force | Out-Null
    Copy-Item -Path (Join-Path -Path $installerAssetsPath -ChildPath "*") `
              -Destination $targetAssetsPath `
              -Recurse `
              -Force `
              -ErrorAction Stop

    Write-Host "Installer assets copied to $targetAssetsPath." -ForegroundColor Green
}

function Test-PackageFile {
    param(
        [Parameter(Mandatory=$true)]
        [string]$RelativePath
    )

    $packageFilePath = Join-Path -Path $packagesPath -ChildPath $RelativePath
    if (-not (Test-Path -Path $packageFilePath -PathType Leaf)) {
        throw "Required installer package file was not found: $packageFilePath"
    }
}

$publishAotEnabled = ConvertTo-Bool -Value $publishAot
$projectPath = Join-Path -Path $PSScriptRoot -ChildPath "../AtomUIGallery.Desktop.csproj"
$assetsPath = Join-Path -Path $PSScriptRoot -ChildPath "../../../output/AtomUIGallery.Desktop/obj/project.assets.json"
$configsPath = Join-Path -Path $PSScriptRoot -ChildPath "../configs"
$versionPropsPath = Join-Path -Path $PSScriptRoot -ChildPath "../../../build/Version.props"

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
        "-p:GalleryPublishAot=true",
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
        "-p:GalleryPublishAot=true",
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
        "-p:GalleryPublishAot=false",
        "-p:PublishSingleFile=true"
    )
}

Copy-InstallerAssets -PackagesPath $packagesPath

if ($IsMacOS) {
    Test-PackageFile -RelativePath "Assets/Images/AtomUIGallery.icns"
    Test-PackageFile -RelativePath "Assets/Images/DmgInstallerBg@2x.png"
    Copy-Item -Path (Join-Path -Path $configsPath -ChildPath "InstallerConfig.dmg.xml") -Destination $configPath/InstallerConfig.xml -Force -ErrorAction Stop
} elseif ($IsWindows) {
    Copy-Item -Path (Join-Path -Path $configsPath -ChildPath "InstallerConfig.wix.xml") -Destination $configPath/InstallerConfig.xml -Force -ErrorAction Stop
} elseif ($IsLinux) {
    Copy-Item -Path (Join-Path -Path $configsPath -ChildPath "InstallerConfig.appimage.xml") -Destination $configPath/InstallerConfig.xml -Force -ErrorAction Stop
} else {
    throw "Unsupported operating system for Gallery installer config generation."
}

if (-not (Update-VersionInFile -XmlFilePath $versionPropsPath -TargetFilePath "$configPath/InstallerConfig.xml")) {
    throw "Failed to update Gallery installer config version."
}
