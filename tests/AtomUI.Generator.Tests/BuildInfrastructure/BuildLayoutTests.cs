using System.Xml.Linq;
using Shouldly;
using Xunit;

namespace AtomUI.Generator.Tests.BuildInfrastructure;

public sealed class BuildLayoutTests
{
    [Fact]
    public void Directory_Build_Files_Import_Only_Repository_Entry_Points()
    {
        GetImports("Directory.Build.props")
            .ShouldBe(["$(MSBuildThisFileDirectory)build/repository/AtomUI.Repository.props"]);
        GetImports("Directory.Build.targets")
            .ShouldBe(["$(MSBuildThisFileDirectory)build/repository/AtomUI.Repository.targets"]);
    }

    [Fact]
    public void Repository_Props_Import_Configuration_In_Deterministic_Order()
    {
        GetImports("build/repository/AtomUI.Repository.props").ShouldBe([
            "$(MSBuildThisFileDirectory)Versions.props",
            "$(MSBuildThisFileDirectory)ProjectDefaults.props",
            "$(MSBuildThisFileDirectory)PackageMetadata.props",
            "$(MSBuildThisFileDirectory)OutputPaths.props",
            "$(MSBuildThisFileDirectory)GeneratorBuildAssets.props",
            "$(MSBuildThisFileDirectory)../nuget/linked-registration/LinkedRegistration.props",
            "$(MSBuildThisFileDirectory)../nuget/localization/Localization.props"
        ]);
    }

    [Fact]
    public void Generator_Build_Assets_Have_One_Canonical_Manifest()
    {
        var manifest = XDocument.Load(GetRepoFile("build/repository/GeneratorBuildAssets.props"));
        var buildAssets = manifest.Descendants("AtomUINuGetBuildAsset").ShouldHaveSingleItem();
        var buildAssetIncludes = ((string?)buildAssets.Attribute("Include")).ShouldNotBeNull();
        buildAssetIncludes.ShouldContain("../nuget/**/*.props");
        buildAssetIncludes.ShouldContain("../nuget/**/*.targets");
        ((string?)buildAssets.Attribute("Exclude"))
            .ShouldNotBeNull()
            .ShouldContain("../nuget/consumer/**");
        buildAssets.Elements("PackagePath")
                   .ShouldHaveSingleItem()
                   .Value.ShouldBe("buildTransitive/%(RecursiveDir)%(Filename)%(Extension)");

        var toolAssets = manifest.Descendants("AtomUIGeneratorToolAsset").ShouldHaveSingleItem();
        var toolIncludes = ((string?)toolAssets.Attribute("Include")).ShouldNotBeNull();
        toolIncludes.ShouldContain("AtomUI.Generator.dll");
        toolIncludes.ShouldContain("AtomUI.Build.Tasks.dll");
        toolAssets.Elements("PackagePath")
                  .ShouldHaveSingleItem()
                  .Value.ShouldBe("tools/netstandard2.0/%(Filename)%(Extension)");

        var generatorProject = XDocument.Load(GetRepoFile("src/AtomUI.Generator/AtomUI.Generator.csproj"));
        generatorProject.Descendants("None")
                        .ShouldContain(element =>
                            (string?)element.Attribute("Include") == "@(AtomUINuGetBuildAsset)");
        generatorProject.Descendants("None")
                        .ShouldContain(element =>
                            (string?)element.Attribute("Include") == "@(AtomUIGeneratorToolAsset)");

        var packageTargets = XDocument.Load(GetRepoFile(
            "build/repository/PackageGeneratorAssets.targets"));
        packageTargets.Descendants("None")
                      .ShouldContain(element =>
                          (string?)element.Attribute("Include") == "@(AtomUINuGetBuildAsset)");
        packageTargets.Descendants("None")
                      .ShouldContain(element =>
                          (string?)element.Attribute("Include") == "@(AtomUIGeneratorToolAsset)");
    }

    [Fact]
    public void NuGet_Features_Use_The_Shared_Build_Tasks_Assembly()
    {
        var infrastructure = XDocument.Load(GetRepoFile("build/nuget/infrastructure/BuildTasks.props"));
        var fallback = infrastructure.Descendants("AtomUIBuildTasksAssembly").ShouldHaveSingleItem();
        ((string?)fallback.Attribute("Condition"))
            .ShouldBe("'$(AtomUIBuildTasksAssembly)' == ''");
        fallback.Value.Trim().ShouldBe(
            "$(MSBuildThisFileDirectory)../../tools/netstandard2.0/AtomUI.Build.Tasks.dll");

        var buildRoot = Path.Combine(GetRepositoryRoot(), "build");
        var featureFiles = Directory.EnumerateFiles(
            Path.Combine(buildRoot, "nuget"),
            "*.targets",
            SearchOption.AllDirectories);
        var usingTasks = featureFiles.SelectMany(file => XDocument.Load(file).Descendants("UsingTask"))
                                     .ToArray();
        usingTasks.ShouldNotBeEmpty();
        usingTasks.ShouldAllBe(element =>
            (string?)element.Attribute("AssemblyFile") == "$(AtomUIBuildTasksAssembly)");

        var buildFiles = Directory.EnumerateFiles(buildRoot, "*.*", SearchOption.AllDirectories)
                                  .Where(file =>
                                      Path.GetExtension(file) is ".props" or ".targets");
        var buildText = string.Join('\n', buildFiles.Select(File.ReadAllText));
        buildText.ShouldNotContain("AtomUILocalizationBuildTasksAssembly");
        buildText.ShouldNotContain("AtomUILinkedRegistrationBuildTasksAssembly");
    }

    [Fact]
    public void Repository_Targets_Import_Defaults_And_Features_In_Deterministic_Order()
    {
        GetImports("build/repository/AtomUI.Repository.targets").ShouldBe([
            "$(MSBuildThisFileDirectory)ProjectDefaults.targets",
            "$(MSBuildThisFileDirectory)../nuget/linked-registration/LinkedRegistration.targets",
            "$(MSBuildThisFileDirectory)PackageGeneratorAssets.targets",
            "$(MSBuildThisFileDirectory)../nuget/localization/Localization.targets",
            "$(MSBuildThisFileDirectory)../nuget/theme/ThemeAssets.targets"
        ]);
    }

    [Fact]
    public void NuGet_Generator_Entry_Points_Import_Features_In_Deterministic_Order()
    {
        GetImports("build/nuget/AtomUI.Generator.props").ShouldBe([
            "$(MSBuildThisFileDirectory)linked-registration/LinkedRegistration.props",
            "$(MSBuildThisFileDirectory)localization/Localization.props"
        ]);
        GetImports("build/nuget/AtomUI.Generator.targets").ShouldBe([
            "$(MSBuildThisFileDirectory)linked-registration/LinkedRegistration.targets",
            "$(MSBuildThisFileDirectory)localization/Localization.targets",
            "$(MSBuildThisFileDirectory)theme/ThemeAssets.targets"
        ]);
    }

    [Fact]
    public void Localization_Targets_Imports_Focused_Owners_In_Deterministic_Order()
    {
        GetImports("build/nuget/localization/Localization.targets").ShouldBe([
            "$(MSBuildThisFileDirectory)Inputs.targets",
            "$(MSBuildThisFileDirectory)ProjectReferences.targets",
            "$(MSBuildThisFileDirectory)Export.targets",
            "$(MSBuildThisFileDirectory)Packaging.targets"
        ]);

        var inputs = XDocument.Load(GetRepoFile("build/nuget/localization/Inputs.targets"));
        inputs.Descendants("AdditionalFiles").ShouldNotBeEmpty();
        inputs.Descendants("CompilerVisibleItemMetadata").ShouldNotBeEmpty();
        GetTargetNames(inputs).ShouldBeEmpty();

        GetTargetNames(XDocument.Load(GetRepoFile(
            "build/nuget/localization/ProjectReferences.targets"))).ShouldBe([
            "AtomUIGetLanguageModuleSourceAssets",
            "AtomUIResolveLanguageContractProjectReferences",
            "AtomUIGetLanguagePackProjectAssets",
            "AtomUIResolveLanguagePackProjectReferences"
        ]);
        GetTargetNames(XDocument.Load(GetRepoFile(
            "build/nuget/localization/Export.targets"))).ShouldBe([
            "AtomUIExportLanguageTemplates"
        ]);
        GetTargetNames(XDocument.Load(GetRepoFile(
            "build/nuget/localization/Packaging.targets"))).ShouldBe([
            "AtomUIPrepareLanguagePackage",
            "AtomUIPrepareLanguageModuleAssets"
        ]);
    }

    [Fact]
    public void Legacy_NuGet_Assets_Do_Not_Remain_At_The_Build_Root()
    {
        var repositoryRoot = GetRepositoryRoot();
        foreach (var relativePath in new[]
                 {
                     "build/AtomUI.Generator.props",
                     "build/AtomUI.Generator.targets",
                     "build/AtomUI.GeneratorConsumer.targets",
                     "build/AtomUI.LinkedRegistration.props",
                     "build/AtomUI.LinkedRegistration.targets",
                     "build/AtomUI.Localization.props",
                     "build/AtomUI.Localization.targets",
                     "build/AtomUI.ThemeAssets.targets"
                 })
        {
            File.Exists(Path.Combine(repositoryRoot, relativePath)).ShouldBeFalse(relativePath);
        }
    }

    private static string[] GetImports(string relativePath)
    {
        return XDocument.Load(GetRepoFile(relativePath))
                        .Descendants()
                        .Where(element => element.Name.LocalName == "Import")
                        .Select(element => (string?)element.Attribute("Project"))
                        .Where(static path => path is not null)
                        .Cast<string>()
                        .ToArray();
    }

    private static string[] GetTargetNames(XDocument document)
    {
        return document.Descendants()
                       .Where(element => element.Name.LocalName == "Target")
                       .Select(element => (string?)element.Attribute("Name"))
                       .Where(static name => name is not null)
                       .Cast<string>()
                       .ToArray();
    }

    private static string GetRepoFile(string relativePath)
    {
        var repositoryRoot = GetRepositoryRoot();
        var candidate = Path.Combine(repositoryRoot, relativePath);
        if (File.Exists(candidate))
        {
            return candidate;
        }

        throw new FileNotFoundException(relativePath);
    }

    private static string GetRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "AtomUI.slnx")))
            {
                return directory.FullName;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException("Could not locate the AtomUI repository root.");
    }
}
