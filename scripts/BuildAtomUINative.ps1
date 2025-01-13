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

cmake -B $buildDir -S $sourceDir -DCMAKE_INSTALL_PREFIX="$installPrefix" -DCMAKE_BUILD_TYPE="$buildType" -G $cmakeGenerator

if ($IsWindows) {
    msbuild $buildDir/atomui.sln /p:Configuration=$buildType
    cmake  --install $buildDir --config $buildType
    Copy-Item -Path $installPrefix/bin/$libName -Destination $deployDir
} else {
    $cpuCount = [Environment]::ProcessorCount
    ninja -j $cpuCount -C $buildDir
    ninja install
    Copy-Item -Path $installPrefix/lib/$libName -Destination $deployDir
}

