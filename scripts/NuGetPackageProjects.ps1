$repositoryRoot = Split-Path -Parent $PSScriptRoot

$AtomUIReleasePackages = @(
    [PSCustomObject]@{ Group = "Base"; PackageId = "AtomUI.Native"; ProjectPath = "src/AtomUI.Native/AtomUI.Native.csproj" },
    [PSCustomObject]@{ Group = "Base"; PackageId = "AtomUI.Localization"; ProjectPath = "src/AtomUI.Localization/AtomUI.Localization.csproj" },
    [PSCustomObject]@{ Group = "Base"; PackageId = "AtomUI.Core"; ProjectPath = "src/AtomUI.Core/AtomUI.Core.csproj" },
    [PSCustomObject]@{ Group = "Base"; PackageId = "AtomUI.Fonts.AlibabaSans"; ProjectPath = "src/AtomUI.Fonts.AlibabaSans/AtomUI.Fonts.AlibabaSans.csproj" },
    [PSCustomObject]@{ Group = "Base"; PackageId = "AtomUI.Controls.Shared"; ProjectPath = "src/AtomUI.Controls.Shared/AtomUI.Controls.Shared.csproj" },
    [PSCustomObject]@{ Group = "Base"; PackageId = "AtomUI.Controls"; ProjectPath = "src/AtomUI.Controls/AtomUI.Controls.csproj" },
    [PSCustomObject]@{ Group = "Base"; PackageId = "AtomUI.Desktop.Controls"; ProjectPath = "src/AtomUI.Desktop.Controls/AtomUI.Desktop.Controls.csproj" },
    [PSCustomObject]@{ Group = "Base"; PackageId = "AtomUI.Toolkits.GalleryBase"; ProjectPath = "src/AtomUI.Toolkits.GalleryBase/AtomUI.Toolkits.GalleryBase.csproj" },
    [PSCustomObject]@{ Group = "Base"; PackageId = "AtomUI.Generator"; ProjectPath = "src/AtomUI.Generator/AtomUI.Generator.csproj" },
    [PSCustomObject]@{ Group = "Base"; PackageId = "AtomUI.Icons.Shared"; ProjectPath = "src/AtomUI.Icons.Shared/AtomUI.Icons.Shared.csproj" },
    [PSCustomObject]@{ Group = "Base"; PackageId = "AtomUI.Icons.AntDesign"; ProjectPath = "src/AtomUI.Icons.AntDesign/AtomUI.Icons.AntDesign.csproj" },
    [PSCustomObject]@{ Group = "Extension"; PackageId = "AtomUI.Desktop.Controls.DataGrid"; ProjectPath = "src/AtomUI.Desktop.Controls.DataGrid/AtomUI.Desktop.Controls.DataGrid.csproj" },
    [PSCustomObject]@{ Group = "Extension"; PackageId = "AtomUI.Desktop.Controls.ColorPicker"; ProjectPath = "src/AtomUI.Desktop.Controls.ColorPicker/AtomUI.Desktop.Controls.ColorPicker.csproj" },
    [PSCustomObject]@{ Group = "Extension"; PackageId = "AtomUI.Desktop.Controls.Extras"; ProjectPath = "src/AtomUI.Desktop.Controls.Extras/AtomUI.Desktop.Controls.Extras.csproj" },
    [PSCustomObject]@{ Group = "Language"; PackageId = "AtomUI.LanguagePack.Template"; ProjectPath = "src/AtomUI.LanguagePack.Template/AtomUI.LanguagePack.Template.csproj" },
    [PSCustomObject]@{ Group = "Language"; PackageId = "AtomUI.Controls.I18n.PtBR"; ProjectPath = "src/LanguagePacks/pt-BR/AtomUI.Controls.I18n.PtBR/AtomUI.Controls.I18n.PtBR.csproj" },
    [PSCustomObject]@{ Group = "Language"; PackageId = "AtomUI.Desktop.Controls.I18n.PtBR"; ProjectPath = "src/LanguagePacks/pt-BR/AtomUI.Desktop.Controls.I18n.PtBR/AtomUI.Desktop.Controls.I18n.PtBR.csproj" },
    [PSCustomObject]@{ Group = "Language"; PackageId = "AtomUI.Desktop.Controls.DataGrid.I18n.PtBR"; ProjectPath = "src/LanguagePacks/pt-BR/AtomUI.Desktop.Controls.DataGrid.I18n.PtBR/AtomUI.Desktop.Controls.DataGrid.I18n.PtBR.csproj" },
    [PSCustomObject]@{ Group = "Language"; PackageId = "AtomUI.Desktop.Controls.ColorPicker.I18n.PtBR"; ProjectPath = "src/LanguagePacks/pt-BR/AtomUI.Desktop.Controls.ColorPicker.I18n.PtBR/AtomUI.Desktop.Controls.ColorPicker.I18n.PtBR.csproj" },
    [PSCustomObject]@{ Group = "Language"; PackageId = "AtomUI.I18n.PtBR"; ProjectPath = "src/LanguagePacks/pt-BR/AtomUI.I18n.PtBR/AtomUI.I18n.PtBR.csproj" }
)

$duplicatePackageIds = @(
    $AtomUIReleasePackages |
        Group-Object PackageId |
        Where-Object Count -gt 1 |
        Select-Object -ExpandProperty Name
)
if ($duplicatePackageIds.Count -gt 0) {
    throw "Duplicate NuGet package IDs: $($duplicatePackageIds -join ', ')"
}

foreach ($package in $AtomUIReleasePackages) {
    $package.ProjectPath = Join-Path $repositoryRoot $package.ProjectPath
    if (-not (Test-Path -LiteralPath $package.ProjectPath -PathType Leaf)) {
        throw "NuGet package project does not exist: $($package.ProjectPath)"
    }
}

$AtomUIBasePackageProjects = @(
    $AtomUIReleasePackages |
        Where-Object Group -eq "Base" |
        Select-Object -ExpandProperty ProjectPath
)
$AtomUIExtensionPackageProjects = @(
    $AtomUIReleasePackages |
        Where-Object Group -eq "Extension" |
        Select-Object -ExpandProperty ProjectPath
)
$AtomUILanguagePackageProjects = @(
    $AtomUIReleasePackages |
        Where-Object Group -eq "Language" |
        Select-Object -ExpandProperty ProjectPath
)
$AtomUIExpectedPackageIds = @($AtomUIReleasePackages | Select-Object -ExpandProperty PackageId)
