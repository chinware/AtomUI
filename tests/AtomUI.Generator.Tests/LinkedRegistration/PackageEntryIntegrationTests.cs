using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Xunit;

namespace AtomUI.Generator.Tests.LinkedRegistration;

public sealed class PackageEntryIntegrationTests
{
    [Theory]
    [InlineData(
        "src/AtomUI.Desktop.Controls/ThemeManagerBuilderExtensions.cs",
        "ThemeManagerBuilderExtensions",
        "UseAllDesktopControls|UseDesktopControls")]
    [InlineData(
        "src/AtomUI.Desktop.Controls.DataGrid/ThemeManagerBuilderExtensions.cs",
        "DataGridThemeManagerBuilderExtensions",
        "UseDesktopDataGrid")]
    [InlineData(
        "src/AtomUI.Desktop.Controls.ColorPicker/ThemeManagerBuilderExtensions.cs",
        "ColorPickerThemeManagerBuilderExtensions",
        "UseDesktopColorPicker")]
    [InlineData(
        "src/AtomUI.Desktop.Controls.Extras/ThemeManagerBuilderExtensions.cs",
        "ExtrasThemeManagerBuilderExtensions",
        "UseDesktopExtras")]
    [InlineData(
        "src/AtomUI.Toolkits.GalleryBase/ThemeManagerBuilderExtensions.cs",
        "ThemeManagerBuilderExtensions",
        "UseGalleryBase")]
    public void Registration_packages_declare_entries_on_the_real_methods(
        string sourcePath,
        string typeName,
        string expectedMethodNames)
    {
        var root = CSharpSyntaxTree.ParseText(
            File.ReadAllText(GetRepoFile(sourcePath)),
            cancellationToken: TestContext.Current.CancellationToken).GetRoot(
            TestContext.Current.CancellationToken);
        var type = root.DescendantNodes()
                       .OfType<ClassDeclarationSyntax>()
                       .Single(declaration => declaration.Identifier.ValueText == typeName);
        var actualMethodNames = type.Members
                                    .OfType<MethodDeclarationSyntax>()
                                    .Where(HasRegistrationEntryAttribute)
                                    .Select(static method => method.Identifier.ValueText)
                                    .OrderBy(static name => name, StringComparer.Ordinal)
                                    .ToArray();

        actualMethodNames.ShouldBe(
            expectedMethodNames.Split('|').OrderBy(static name => name, StringComparer.Ordinal));
    }

    [Fact]
    public void Common_is_not_declared_as_a_linked_registration_package()
    {
        var projectSource = File.ReadAllText(GetRepoFile(
            "src/AtomUI.Controls/AtomUI.Controls.csproj"));
        var legacyEntryProperty = "AtomUIRegistration" + "Entries";
        projectSource.ShouldNotContain("AtomUIRegistrationPackageId");
        projectSource.ShouldNotContain(legacyEntryProperty);

        var root = CSharpSyntaxTree.ParseText(
            File.ReadAllText(GetRepoFile(
                "src/AtomUI.Controls/ThemeManagerBuildExtensions.cs")),
            cancellationToken: TestContext.Current.CancellationToken).GetRoot(
            TestContext.Current.CancellationToken);
        var method = root.DescendantNodes()
                         .OfType<MethodDeclarationSyntax>()
                         .Single(declaration => declaration.Identifier.ValueText == "UseCommonControls");
        HasRegistrationEntryAttribute(method).ShouldBeFalse();
    }

    [Fact]
    public void Only_Desktop_Controls_Opts_Into_Directory_Registration_Granularity()
    {
        var desktopProject = File.ReadAllText(GetRepoFile(
            "src/AtomUI.Desktop.Controls/AtomUI.Desktop.Controls.csproj"));
        desktopProject.ShouldContain(
            "<AtomUIRegistrationGranularity>Directory</AtomUIRegistrationGranularity>");

        foreach (var projectPath in new[]
                 {
                     "src/AtomUI.Desktop.Controls.DataGrid/AtomUI.Desktop.Controls.DataGrid.csproj",
                     "src/AtomUI.Desktop.Controls.ColorPicker/AtomUI.Desktop.Controls.ColorPicker.csproj",
                     "src/AtomUI.Desktop.Controls.Extras/AtomUI.Desktop.Controls.Extras.csproj",
                     "src/AtomUI.Toolkits.GalleryBase/AtomUI.Toolkits.GalleryBase.csproj"
                 })
        {
            File.ReadAllText(GetRepoFile(projectPath))
                .ShouldNotContain("AtomUIRegistrationGranularity");
        }
    }

    [Theory]
    [InlineData("src/AtomUI.Desktop.Controls.DataGrid/AtomUI.Desktop.Controls.DataGrid.csproj")]
    [InlineData("src/AtomUI.Desktop.Controls.ColorPicker/AtomUI.Desktop.Controls.ColorPicker.csproj")]
    public void Package_Granularity_Projects_Do_Not_Declare_Unit_Ownership_Patches(
        string projectPath)
    {
        var projectSource = File.ReadAllText(GetRepoFile(projectPath));

        projectSource.ShouldNotContain("AtomUIRegistrationUnit");
    }

    [Theory]
    [InlineData(
        "src/AtomUI.Controls/ThemeManagerBuildExtensions.cs",
        "ThemeManagerBuilderExtensions",
        "AtomUI.Controls.Common",
        "src/AtomUI.Controls/CommonControlThemesProvider.cs|" +
        "src/AtomUI.Controls/BrowserCommonControlThemesProvider.cs")]
    [InlineData(
        "src/AtomUI.Desktop.Controls/ThemeManagerBuilderExtensions.cs",
        "ThemeManagerBuilderExtensions",
        "AtomUI.Desktop.Controls",
        "src/AtomUI.Desktop.Controls/DesktopControlThemesProvider.cs|" +
        "src/AtomUI.Desktop.Controls/BrowserDesktopControlThemesProvider.cs")]
    [InlineData(
        "src/AtomUI.Desktop.Controls.DataGrid/ThemeManagerBuilderExtensions.cs",
        "DataGridThemeManagerBuilderExtensions",
        "AtomUI.Desktop.Controls.DataGrid",
        "src/AtomUI.Desktop.Controls.DataGrid/AtomUIDataGridThemesProvider.cs")]
    [InlineData(
        "src/AtomUI.Desktop.Controls.ColorPicker/ThemeManagerBuilderExtensions.cs",
        "ColorPickerThemeManagerBuilderExtensions",
        "AtomUI.Desktop.Controls.ColorPicker",
        "src/AtomUI.Desktop.Controls.ColorPicker/AtomUIColorPickerThemesProvider.cs")]
    [InlineData(
        "src/AtomUI.Desktop.Controls.Extras/ThemeManagerBuilderExtensions.cs",
        "ExtrasThemeManagerBuilderExtensions",
        "AtomUI.Desktop.Controls.Extras",
        "src/AtomUI.Desktop.Controls.Extras/AtomUIExtrasThemesProvider.cs")]
    public void Runtime_package_ids_are_declared_once_and_reused_by_theme_providers(
        string entrySourcePath,
        string entryTypeName,
        string packageId,
        string providerSourcePaths)
    {
        var entrySource = File.ReadAllText(GetRepoFile(entrySourcePath));
        entrySource.ShouldContain($"internal const string PackageId = \"{packageId}\";");
        entrySource.Split($"\"{packageId}\"", StringSplitOptions.None).Length.ShouldBe(2);

        foreach (var providerSourcePath in providerSourcePaths.Split('|'))
        {
            var providerSource = File.ReadAllText(GetRepoFile(providerSourcePath));
            providerSource.ShouldContain($"Id = {entryTypeName}.PackageId;");
            providerSource.ShouldNotContain($"\"{packageId}\"");
        }
    }

    [Fact]
    public void Common_dependency_remains_on_the_full_registration_path()
    {
        var source = File.ReadAllText(GetRepoFile(
            "src/AtomUI.Controls/ThemeManagerBuildExtensions.cs"));
        var body = GetMethodBody(source, "UseCommonControls");

        body.ShouldContain("GeneratedControlPackageRegistration.Register");
        body.ShouldContain("GeneratedLanguageModuleRegistration.Register");
        body.ShouldNotContain("AotTrimRegistration");
        body.ShouldNotContain("AotTrimRegistrationPlanRegistry");
    }

    [Fact]
    public void Desktop_generated_entry_preserves_package_core_order()
    {
        var source = File.ReadAllText(GetRepoFile(
            "src/AtomUI.Desktop.Controls/ThemeManagerBuilderExtensions.cs"));
        source.ShouldContain("using AtomUI.Generated.AtomUIDesktopControls;");
        source.ShouldNotContain("using AtomUI.Generated.AtomUIControls;");
        var entryBody = GetMethodBody(source, "UseDesktopControls");

        AssertOrder(
            entryBody,
            "PrepareDesktopPackageCore(builder)",
            "if (AotTrimRegistration.IsEnabled)",
            "RegisterGeneratedDesktopPackage(builder)",
            "RegisterFullDesktopPackage(builder)",
            "CompleteDesktopPackageCore(builder)");
        entryBody.ShouldNotContain("useGeneratedRegistration");

        var prepareBody = GetMethodBody(source, "PrepareDesktopPackageCore");
        AssertOrder(
            prepareBody,
            "builder.UseCommonControls()",
            "DialogInputCaptureTracker.Initialize()");
        var completeBody = GetMethodBody(source, "CompleteDesktopPackageCore");
        AssertOrder(
            completeBody,
            "GeneratedLanguageModuleRegistration.Register(builder.Localization)",
            "builder.Theme.AddInitializer(InitializeDesktopRuntime)");

        source.ShouldContain("public static IAtomUIBuilder UseAllDesktopControls");
        var fullBody = GetMethodBody(source, "UseAllDesktopControls");
        AssertOrder(
            fullBody,
            "PrepareDesktopPackageCore(builder)",
            "RegisterFullDesktopPackage(builder)",
            "CompleteDesktopPackageCore(builder)");
        fullBody.ShouldNotContain("AotTrimRegistration");
        fullBody.ShouldNotContain("RegisterGeneratedDesktopPackage");
        source.ShouldContain("AotTrimRegistrationPlanRegistry.ApplyPackage(");
        source.ShouldContain("\"AtomUI.Desktop.Controls\"");
        source.ShouldContain("DesktopControlThemeAssetSelector.IsBrowserControlSupported");
        source.ShouldContain("DesktopControlThemeAssetSelector.SelectBrowser");
        source.ShouldContain("DesktopControlThemeAssetSelector.SelectNative");
    }

    [Theory]
    [InlineData(
        "src/AtomUI.Desktop.Controls.DataGrid/ThemeManagerBuilderExtensions.cs",
        "UseDesktopDataGrid",
        "AtomUI.Desktop.Controls.DataGrid",
        "AtomUI.Generated.AtomUIDesktopControlsDataGrid",
        true)]
    [InlineData(
        "src/AtomUI.Desktop.Controls.ColorPicker/ThemeManagerBuilderExtensions.cs",
        "UseDesktopColorPicker",
        "AtomUI.Desktop.Controls.ColorPicker",
        "AtomUI.Generated.AtomUIDesktopControlsColorPicker",
        true)]
    [InlineData(
        "src/AtomUI.Desktop.Controls.Extras/ThemeManagerBuilderExtensions.cs",
        "UseDesktopExtras",
        "AtomUI.Desktop.Controls.Extras",
        "AtomUI.Generated.AtomUIDesktopControlsExtras",
        false)]
    public void Optional_package_entries_branch_only_the_control_package_registration(
        string sourcePath,
        string methodName,
        string packageId,
        string generatedNamespace,
        bool hasLanguageModule)
    {
        var source = File.ReadAllText(GetRepoFile(sourcePath));
        source.ShouldContain($"using {generatedNamespace};");
        var body = GetMethodBody(source, methodName);

        body.ShouldContain("AotTrimRegistration.IsEnabled");
        body.ShouldContain("AotTrimRegistrationPlanRegistry.ApplyPackage(");
        body.ShouldContain("GeneratedControlPackageRegistration.Register(");
        body.ShouldContain("PackageId");
        body.ShouldNotContain($"\"{packageId}\"");
        if (hasLanguageModule)
        {
            body.IndexOf("AotTrimRegistrationPlanRegistry.ApplyPackage(", StringComparison.Ordinal)
                .ShouldBeLessThan(body.IndexOf(
                    "GeneratedLanguageModuleRegistration.Register(builder.Localization)",
                    StringComparison.Ordinal));
        }
        else
        {
            body.ShouldNotContain("GeneratedLanguageModuleRegistration");
        }
    }

    [Fact]
    public void GalleryBase_keeps_configuration_and_language_outside_the_registration_branch()
    {
        var source = File.ReadAllText(GetRepoFile(
            "src/AtomUI.Toolkits.GalleryBase/ThemeManagerBuilderExtensions.cs"));
        source.ShouldContain("using AtomUI.Generated.AtomUIToolkitsGalleryBase;");
        source.ShouldContain(
            "internal const string PackageId = \"AtomUI.Toolkits.GalleryBase\";");
        var body = GetMethodBody(source, "UseGalleryBase");

        AssertOrder(
            body,
            "GalleryBaseConfigurationProvider.SetCurrent",
            "AotTrimRegistrationPlanRegistry.ApplyPackage(",
            "GeneratedLanguageModuleRegistration.Register(builder.Localization)");
        body.ShouldContain("AotTrimRegistration.IsEnabled");
        body.ShouldContain("GeneratedControlPackageRegistration.Register(");
        body.ShouldContain("PackageId");
        body.ShouldNotContain("\"AtomUI.Toolkits.GalleryBase\"");

        var providerSource = File.ReadAllText(GetRepoFile(
            "src/AtomUI.Toolkits.GalleryBase/Controls/GalleryControlThemesProvider.cs"));
        providerSource.ShouldContain("Id = \"AtomUI.Toolkits.GalleryBase.Controls\";");
    }

    private static string GetMethodBody(string source, string methodName)
    {
        var root = CSharpSyntaxTree.ParseText(source).GetRoot();
        var method = root.DescendantNodes()
                         .OfType<MethodDeclarationSyntax>()
                         .Single(declaration => declaration.Identifier.ValueText == methodName);
        return method.Body.ShouldNotBeNull().ToFullString();
    }

    private static bool HasRegistrationEntryAttribute(MethodDeclarationSyntax method)
    {
        return method.AttributeLists
                     .SelectMany(static list => list.Attributes)
                     .Any(static attribute =>
                     {
                         var name = attribute.Name.ToString();
                         return name.EndsWith(
                                    "ControlPackageRegistrationEntry",
                                    StringComparison.Ordinal) ||
                                name.EndsWith(
                                    "ControlPackageRegistrationEntryAttribute",
                                    StringComparison.Ordinal);
                     });
    }

    private static void AssertOrder(string source, params string[] values)
    {
        var previous = -1;
        foreach (var value in values)
        {
            var current = source.IndexOf(value, StringComparison.Ordinal);
            current.ShouldBeGreaterThan(previous, $"'{value}' must follow the preceding Package Core step");
            previous = current;
        }
    }

    private static string GetRepoFile(string relativePath)
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            var candidate = Path.Combine(directory.FullName, relativePath);
            if (File.Exists(candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException(relativePath);
    }
}
