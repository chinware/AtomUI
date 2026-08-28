[CmdletBinding()]
param(
    [ValidateSet("Debug", "Release")]
    [string]$BuildType = "Release",
    [string]$PackageOutputDir = ""
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

. "$PSScriptRoot/NuGetPackageProjects.ps1"

if ([string]::IsNullOrWhiteSpace($PackageOutputDir)) {
    $PackageOutputDir = Join-Path $repositoryRoot ".artifacts/Nuget/$BuildType"
}
$PackageOutputDir = [System.IO.Path]::GetFullPath($PackageOutputDir)
New-Item -Path $PackageOutputDir -ItemType Directory -Force | Out-Null

function Invoke-AtomUIDotNet {
    param(
        [Parameter(Mandatory = $true)]
        [string[]]$Arguments
    )

    Write-Host "dotnet $($Arguments -join ' ')"
    & dotnet @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw "dotnet $($Arguments[0]) failed with exit code $LASTEXITCODE"
    }
}

$buildArguments = @(
    "--configuration", $BuildType,
    "--disable-build-servers",
    "-m:1",
    "/nr:false"
)

foreach ($project in $AtomUIReleaseBuildPrerequisiteProjects) {
    Invoke-AtomUIDotNet -Arguments (@("build", $project) + $buildArguments)
}

foreach ($project in $AtomUIReleasePackageProjects) {
    Invoke-AtomUIDotNet -Arguments (@("build", $project) + $buildArguments)
}

foreach ($project in $AtomUIReleasePackageProjects) {
    Invoke-AtomUIDotNet -Arguments (@(
        "pack",
        $project,
        "--no-build",
        "--output", $PackageOutputDir
    ) + $buildArguments)
}

[xml]$versionProps = Get-Content -LiteralPath (Join-Path $repositoryRoot "build/Versions.props")
$version = [string]$versionProps.Project.PropertyGroup.AtomUIVersion
if ([string]::IsNullOrWhiteSpace($version)) {
    throw "Unable to read AtomUIVersion from build/Versions.props"
}

$expectedPackageNames = @($AtomUIExpectedPackageIds | ForEach-Object { "$($_).$version.nupkg" })
$actualPackageNames = @(
    Get-ChildItem -LiteralPath $PackageOutputDir -Filter "*.nupkg" -File |
        Select-Object -ExpandProperty Name
)
$missingPackages = @($expectedPackageNames | Where-Object { $_ -notin $actualPackageNames })
$unexpectedPackages = @($actualPackageNames | Where-Object { $_ -notin $expectedPackageNames })

if ($missingPackages.Count -gt 0) {
    throw "Missing NuGet packages: $($missingPackages -join ', ')"
}
if ($unexpectedPackages.Count -gt 0) {
    throw "Unexpected NuGet packages: $($unexpectedPackages -join ', ')"
}

Write-Output "Verified $($actualPackageNames.Count) AtomUI NuGet packages for version $version"
