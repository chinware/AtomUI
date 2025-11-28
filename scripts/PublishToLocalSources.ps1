param (
    [string]$localSourcesDir = "D:/nuget.local",
    [string]$buildType = "Release"
)

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

dotnet build -v diag --configuration $buildType ../src/AtomUI.Core/AtomUI.Core.csproj
dotnet build -v diag --configuration $buildType ../src/AtomUI.Fonts.AlibabaSans/AtomUI.Fonts.AlibabaSans.csproj
dotnet build -v diag --configuration $buildType ../src/AtomUI.Controls.Shared/AtomUI.Controls.Shared.csproj
dotnet build -v diag --configuration $buildType ../src/AtomUI.Desktop.Controls/AtomUI.Desktop.Controls.csproj
dotnet build -v diag --configuration $buildType ../src/AtomUI.Generator/AtomUI.Generator.csproj
dotnet build -v diag --configuration $buildType ../src/AtomUI.Icons.Shared/AtomUI.Icons.Shared.csproj
dotnet build -v diag --configuration $buildType ../src/AtomUI.Icons.AntDesign/AtomUI.Icons.AntDesign.csproj

dotnet pack --no-build --configuration $buildType ../src/AtomUI.Core/AtomUI.Core.csproj
dotnet pack --no-build --configuration $buildType ../src/AtomUI.Fonts.AlibabaSans/AtomUI.Fonts.AlibabaSans.csproj
dotnet pack --no-build --configuration $buildType ../src/AtomUI.Controls.Shared/AtomUI.Controls.Shared.csproj
dotnet pack --no-build --configuration $buildType ../src/AtomUI.Desktop.Controls/AtomUI.Desktop.Controls.csproj
dotnet pack --no-build --configuration $buildType ../src/AtomUI.Generator/AtomUI.Generator.csproj
dotnet pack --no-build --configuration $buildType ../src/AtomUI.Icons.Shared/AtomUI.Icons.Shared.csproj
dotnet pack --no-build --configuration $buildType ../src/AtomUI.Icons.AntDesign/AtomUI.Icons.AntDesign.csproj

Push-NuGetPackages -Source $localSourcesDir

dotnet build -v diag --configuration $buildType ../src/AtomUI.Desktop.Controls.DataGrid/AtomUI.Desktop.Controls.DataGrid.csproj
dotnet pack --no-build --configuration $buildType ../src/AtomUI.Desktop.Controls.DataGrid/AtomUI.Desktop.Controls.DataGrid.csproj

dotnet build -v diag --configuration $buildType ../src/AtomUI.Desktop.Controls.ColorPicker/AtomUI.Desktop.Controls.ColorPicker.csproj
dotnet pack --no-build --configuration $buildType ../src/AtomUI.Desktop.Controls.ColorPicker/AtomUI.Desktop.Controls.ColorPicker.csproj

Push-NuGetPackages -Source $localSourcesDir
