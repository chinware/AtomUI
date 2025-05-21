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
     xmake config --project=$sourceDir --buildir=$buildDir -p msys -a x86_64 -m $buildType
     xmake build
     xmake install --installdir=$installPrefix
     Copy-Item -Path $installPrefix/x86_64/lib/$libName -Destination $deployDir
} else {
    xmake config --project=$sourceDir --buildir=$buildDir -p macosx -a arm64 -m $buildType --toolchain=atomui --sdk=/opt/homebrew/opt/llvm@19
    xmake build
    xmake install --installdir=$installPrefix
    xmake config --project=$sourceDir --buildir=$buildDir -p macosx -a x86_64 -m $buildType --toolchain=atomui --sdk=/opt/homebrew/opt/llvm@19
    xmake build
    xmake install --installdir=$installPrefix
    Write-Output "generate universal binary"
    lipo -create $installPrefix/arm64/lib/$libName $installPrefix/x86_64/lib/$libName -output $deployDir/$libName
    Write-Output "generate success, saved to ${deployDir}"
}
