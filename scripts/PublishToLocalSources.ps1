param (
    [string]$localSourcesDir = "D:/nuget.local",
    [string]$buildType = "Release"
)

$ErrorActionPreference = "Stop"
$PSNativeCommandUseErrorActionPreference = $true
$packageOutputDir = "../output/Nuget/$buildType"

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

$baseProjects = @(
    "../src/AtomUI.Native/AtomUI.Native.csproj",
    "../src/AtomUI.Localization/AtomUI.Localization.csproj",
    "../src/AtomUI.Core/AtomUI.Core.csproj",
    "../src/AtomUI.Fonts.AlibabaSans/AtomUI.Fonts.AlibabaSans.csproj",
    "../src/AtomUI.Controls.Shared/AtomUI.Controls.Shared.csproj",
    "../src/AtomUI.Controls/AtomUI.Controls.csproj",
    "../src/AtomUI.Desktop.Controls/AtomUI.Desktop.Controls.csproj",
    "../src/AtomUI.Toolkits.GalleryBase/AtomUI.Toolkits.GalleryBase.csproj",
    "../src/AtomUI.Generator/AtomUI.Generator.csproj",
    "../src/AtomUI.Icons.Shared/AtomUI.Icons.Shared.csproj",
    "../src/AtomUI.Icons.AntDesign/AtomUI.Icons.AntDesign.csproj"
)

foreach ($project in $baseProjects) {
    dotnet build -v minimal --configuration $buildType $project
}

foreach ($project in $baseProjects) {
    dotnet pack --no-build --configuration $buildType $project
}

Push-NuGetPackages -PackagePath $packageOutputDir -Source $localSourcesDir

$extensionProjects = @(
    "../src/AtomUI.Desktop.Controls.DataGrid/AtomUI.Desktop.Controls.DataGrid.csproj",
    "../src/AtomUI.Desktop.Controls.ColorPicker/AtomUI.Desktop.Controls.ColorPicker.csproj"
)

foreach ($project in $extensionProjects) {
    dotnet build -v minimal --configuration $buildType $project
    dotnet pack --no-build --configuration $buildType $project
}

$languageProjects = @(
    "../src/AtomUI.LanguagePack.Template/AtomUI.LanguagePack.Template.csproj",
    "../src/LanguagePacks/pt-BR/AtomUI.Controls.I18n.PtBR/AtomUI.Controls.I18n.PtBR.csproj",
    "../src/LanguagePacks/pt-BR/AtomUI.Desktop.Controls.I18n.PtBR/AtomUI.Desktop.Controls.I18n.PtBR.csproj",
    "../src/LanguagePacks/pt-BR/AtomUI.Desktop.Controls.DataGrid.I18n.PtBR/AtomUI.Desktop.Controls.DataGrid.I18n.PtBR.csproj",
    "../src/LanguagePacks/pt-BR/AtomUI.Desktop.Controls.ColorPicker.I18n.PtBR/AtomUI.Desktop.Controls.ColorPicker.I18n.PtBR.csproj",
    "../src/LanguagePacks/pt-BR/AtomUI.I18n.PtBR/AtomUI.I18n.PtBR.csproj"
)

foreach ($project in $languageProjects) {
    dotnet build -v minimal --configuration $buildType $project
    dotnet pack --no-build --configuration $buildType $project
}

Push-NuGetPackages -PackagePath $packageOutputDir -Source $localSourcesDir
