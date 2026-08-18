param (
    [string]$localSourcesDir = "D:/nuget.local",
    [string]$buildType = "Release"
)

$ErrorActionPreference = "Stop"
$PSNativeCommandUseErrorActionPreference = $true
$packageOutputDir = Join-Path $PSScriptRoot "../output/Nuget/$buildType"
. "$PSScriptRoot/NuGetPackageProjects.ps1"

function Push-NuGetPackages {
    [CmdletBinding(SupportsShouldProcess = $true)]
    param(
        [string]$PackagePath = "../output/Nuget",
        [Parameter(Mandatory = $true)]
        [string]$Source
    )

    # 获取所有NuGet包
    $packages = Get-ChildItem -Path $PackagePath -Filter *.nupkg -Recurse -File

    if (-not $packages) {
        Write-Warning "未找到任何.nupkg文件"
        return
    }

    # 处理每个包
    foreach ($pkg in $packages) {
        if ($PSCmdlet.ShouldProcess($pkg.Name, "推送并删除")) {
            try {
                # 推送包
                dotnet nuget push $pkg.FullName --source $Source

                if ($LASTEXITCODE -eq 0) {
                    # 删除成功推送的包
                    Remove-Item $pkg.FullName -Force
                    Write-Host "✓ 成功: $($pkg.Name)" -ForegroundColor Green
                } else {
                    Write-Warning "推送失败: $($pkg.Name) (退出码: $LASTEXITCODE)"
                }
            }
            catch {
                Write-Error "处理 $($pkg.Name) 时出错: $_"
            }
        }
    }
}

foreach ($project in $AtomUIBasePackageProjects) {
    dotnet build -v minimal --configuration $buildType $project
}

foreach ($project in $AtomUIBasePackageProjects) {
    dotnet pack --no-build --configuration $buildType $project
}

Push-NuGetPackages -PackagePath $packageOutputDir -Source $localSourcesDir

foreach ($project in $AtomUIExtensionPackageProjects) {
    dotnet build -v minimal --configuration $buildType $project
    dotnet pack --no-build --configuration $buildType $project
}

foreach ($project in $AtomUILanguagePackageProjects) {
    dotnet build -v minimal --configuration $buildType $project
    dotnet pack --no-build --configuration $buildType $project
}

Push-NuGetPackages -PackagePath $packageOutputDir -Source $localSourcesDir
