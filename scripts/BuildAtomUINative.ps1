<#
    Compile native AtomUI native library
#>
param (
    [string]$buildDir,
    [string]$sourceDir,
    [string]$installPrefix,
    [string]$deployDir,
    [string]$buildType = "Debug"
)

$os = [System.Environment]::OSVersion.Platform
$isWindows = $false
$isMacOS = $false
$isLinux = $false

if ($os -eq "Win32NT") {
    $isWindows = $true
}

$cmakeGenerator = "Ninja"

if ($isWindows) {
    $cmakeGenerator = "Visual Studio 17 2022"
}

cmake -B $buildDir -S $sourceDir -DCMAKE_INSTALL_PREFIX="$installPrefix" -DCMAKE_BUILD_TYPE="$buildType" -G "Visual Studio 17 2022"

if ($isWindows) {
    msbuild $buildDir/atomui.sln /p:Configuration=$buildType
    cmake  --install $buildDir --config $buildType
    Copy-Item -Path $installPrefix/bin/AtomUINative.dll -Destination $deployDir
}

