param (
    [string]$localSourcesDir = "D:/nuget.local",
    [string]$buildType = "Release"
)

dotnet build -v diag --configuration $buildType ../AtomUI.sln
dotnet pack --no-build --configuration $buildType /p:IsNightlyBuild=true ../packages/AtomUI/AtomUI.csproj

# 探测包
$packages = Get-ChildItem -Path ../output/Nuget -Filter *.nupkg -Recurse -File

foreach ($pkg in $packages) {
    try {        
        # 执行 NuGet 推送命令
        dotnet nuget push $pkg.FullName --source $localSourcesDir
        
        if ($LASTEXITCODE -ne 0) {
            Write-Warning "推送失败: $($pkg.Name)"
        }
    }
    catch {
        Write-Error "推送 $($pkg.Name) 时发生异常: $_"
    }
}