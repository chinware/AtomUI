param (
    [string]$publishRootPath = "D:/publish",
    [string]$buildType = "Release",
    [string]$framework = "net10.0",
    [string]$runtime = "osx-arm64",
    [string]$buildTimestamp
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

dotnet publish --output $packagesPath --self-contained --framework $framework -r $runtime --configuration $buildType -p:PublishSingleFile=true ../AtomUIGallery.Desktop.csproj
if ($IsMacOS) {
    Copy-Item -Path ../configs/InstallerConfig.dmg.xml -Destination $configPath/InstallerConfig.xml -Force
} elseif ($IsWindows) {
    Copy-Item -Path ../configs/InstallerConfig.wix.xml -Destination $configPath/InstallerConfig.xml -Force
} elseif ($IsLinux) {
    Copy-Item -Path ../configs/InstallerConfig.appimage.xml -Destination $configPath/InstallerConfig.xml -Force
}
Update-VersionInFile -XmlFilePath "../../../build/Version.props" -TargetFilePath "$configPath/InstallerConfig.xml"