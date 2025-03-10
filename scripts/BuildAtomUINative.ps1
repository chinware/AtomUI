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

if (-not (Test-Path -Path $buildMarker -PathType Leaf)) {
    # 如果文件不存在，则创建（自动创建父目录）
    New-Item -Path $buildMarker -ItemType File -Force
} else {
    return
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
