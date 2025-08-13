<#
    Compile native AtomUI native library
#>
param (
    [string]$buildDir,
    [string]$sourceDir,
    [string]$installPrefix,
    [string]$deployDir,
    [string]$buildType = "debug",
    [string]$libName
)

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

$env:XMAKE_COLORTERM = 'nocolor'
$buildType = $buildType.ToLower()

if ($IsWindows) {
    
    cmake -B $buildDir -S $sourceDir -DCMAKE_INSTALL_PREFIX="$installPrefix" -DCMAKE_BUILD_TYPE="$buildType" -G Ninja
    cmake --build $buildDir
    cmake --install $buildDir --config $buildType
    Copy-Item -Path $installPrefix/bin/$libName -Destination $deployDir
    
} elseif ($IsMacOS) {
    cmake -B $buildDir -S $sourceDir -DCMAKE_INSTALL_PREFIX="$installPrefix" -DCMAKE_BUILD_TYPE="$buildType" -G Ninja -DCMAKE_OSX_ARCHITECTURES="arm64;x86_64"
    cmake --build $buildDir
    cmake --install $buildDir --config $buildType
    Copy-Item -Path $installPrefix/lib/$libName -Destination $deployDir
} elseif ($IsLinux) {
    cmake -B $buildDir -S $sourceDir -DCMAKE_INSTALL_PREFIX="$installPrefix" -DCMAKE_BUILD_TYPE="$buildType"
    cmake --build $buildDir
    cmake --install $buildDir --config $buildType
    Copy-Item -Path $installPrefix/lib/$libName -Destination $deployDir
} else {
    $osInfo = $PSVersionTable.OS
    throw "Unsupported operating system: $osInfo. Only supported on Windows, Linux or macOS."
}

Write-Output "generate success, saved to ${deployDir}"