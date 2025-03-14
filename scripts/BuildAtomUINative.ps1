<#
    Compile native AtomUI native library
#>
param (
    [string]$buildDir,
    [string]$sourceDir,
    [string]$installPrefix,
    [string]$deployDir,
    [string]$buildType = "Debug",
    [string]$libName
)

$cmakeGenerator = "Ninja"

if ($IsWindows) {
    $cmakeGenerator = "Visual Studio 17 2022"
}

$buildMarker = Join-Path -Path $buildDir -ChildPath ".native_compiled"
$lockFile = Join-Path -Path $buildDir -ChildPath ".lock"
try {
    # 创建锁文件（原子操作）
    $lock = [System.IO.File]::Open($lockFile, [System.IO.FileMode]::CreateNew, [System.IO.FileAccess]::Write, [System.IO.FileShare]::None)

    if (-not (Test-Path -Path $buildMarker -PathType Leaf)) {
        # 如果文件不存在，则创建（自动创建父目录）
        New-Item -Path $buildMarker -ItemType File -Force | Out-Null
    } else {
        return
    }
} catch [System.IO.IOException] {
    if (Test-Path $buildMarker) {
        Write-Host "目录已被其他进程创建: $buildMarker"
    } else {
        Write-Host "创建失败: $_，跳过编译"
    }
    return
} finally {
    # 释放锁文件
    if ($lock) { $lock.Close() }
    Remove-Item $lockFile -ErrorAction SilentlyContinue
}

if ($IsWindows) {
    $possiblePaths = @(
        "C:\Program Files (x86)\Microsoft Visual Studio\2019\Community\MSBuild\Current\Bin\MSBuild.exe",
        "C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\MSBuild\Current\Bin\MSBuild.exe",
        "C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\MSBuild\Current\Bin\MSBuild.exe",
        "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\MSBuild.exe",
        "C:\Program Files\Microsoft Visual Studio\2022\Professional\MSBuild\Current\Bin\MSBuild.exe",
        "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe"
    )

    $msbuildExecutale = $possiblePaths | Where-Object { Test-Path $_ }
    if (![string]::IsNullOrEmpty($msbuildExecutale)) {
        $msbuildPath = Split-Path (Get-Item $msbuildExecutale).FullName -Parent
        $env:PATH += ";$msbuildPath"
    }
    cmake -B $buildDir -S $sourceDir -DCMAKE_INSTALL_PREFIX="$installPrefix" -DCMAKE_BUILD_TYPE="$buildType" -G $cmakeGenerator
    msbuild $buildDir/atomui.sln /p:Configuration=$buildType
    cmake --install $buildDir --config $buildType
    Copy-Item -Path $installPrefix/bin/$libName -Destination $deployDir
} else {
    cmake -B $buildDir -S $sourceDir -DCMAKE_INSTALL_PREFIX="$installPrefix" -DCMAKE_OSX_ARCHITECTURES:STRING="x86_64;arm64" -DCMAKE_BUILD_TYPE="$buildType" -G $cmakeGenerator
    $cpuCount = [Environment]::ProcessorCount
    ninja -j $cpuCount -C $buildDir
    ninja install
    Copy-Item -Path $installPrefix/lib/$libName -Destination $deployDir
}
