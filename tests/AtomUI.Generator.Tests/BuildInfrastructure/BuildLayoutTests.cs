using System.Diagnostics;
using System.Xml.Linq;
using Shouldly;
using Xunit;

namespace AtomUI.Generator.Tests.BuildInfrastructure;

public sealed class BuildLayoutTests
{
    private static readonly string[] s_expectedBuildFiles =
    [
        "AtomUI.Generator.props",
        "AtomUI.Generator.targets",
        "AtomUI.GeneratorConsumer.targets",
        "AtomUI.LinkedRegistration.props",
        "AtomUI.LinkedRegistration.targets",
        "AtomUI.LinkedRegistration.SidecarConsumer.targets",
        "AtomUI.Localization.props",
        "AtomUI.Localization.targets",
        "AtomUI.Repository.props",
        "AtomUI.Repository.targets",
        "AtomUI.ThemeAssets.targets",
        "MacOSHomebrewNativeAot.targets",
        "OutputPaths.props",
        "PackageMetadata.props",
        "ProjectDefaults.props",
        "Versions.props"
    ];

    private static readonly string[] s_expectedNuGetBuildAssets =
    [
        "AtomUI.Generator.props",
        "AtomUI.Generator.targets",
        "AtomUI.LinkedRegistration.props",
        "AtomUI.LinkedRegistration.targets",
        "AtomUI.LinkedRegistration.SidecarConsumer.targets",
        "AtomUI.Localization.props",
        "AtomUI.Localization.targets",
        "AtomUI.ThemeAssets.targets"
    ];

    [Fact]
    public void Directory_Build_Files_Import_Only_Repository_Entry_Points()
    {
        GetImports("Directory.Build.props")
            .ShouldBe(["$(MSBuildThisFileDirectory)build/AtomUI.Repository.props"]);
        GetImports("Directory.Build.targets")
            .ShouldBe(["$(MSBuildThisFileDirectory)build/AtomUI.Repository.targets"]);
    }

    [Fact]
    public void Repository_Props_Import_Configuration_And_Generator_Entry_Point()
    {
        const string repositoryPropsPath = "build/AtomUI.Repository.props";
        GetImports(repositoryPropsPath).ShouldBe([
            "$(MSBuildThisFileDirectory)Versions.props",
            "$(MSBuildThisFileDirectory)ProjectDefaults.props",
            "$(MSBuildThisFileDirectory)PackageMetadata.props",
            "$(MSBuildThisFileDirectory)OutputPaths.props",
            "$(OutputPathWithoutFramework)/netstandard2.0/AtomUI.BuildTasks.ShadowKey.props",
            "$(MSBuildThisFileDirectory)AtomUI.Generator.props"
        ]);
    }

    [Fact]
    public void Repository_Targets_Reuse_The_Generator_Entry_Point()
    {
        var repositoryTargets = XDocument.Load(GetRepoFile("build/AtomUI.Repository.targets"));

        GetImports("build/AtomUI.Repository.targets")
            .ShouldBe(["$(MSBuildThisFileDirectory)AtomUI.Generator.targets"]);
        GetTargetNames(repositoryTargets)
            .ShouldContain("AtomUIPrepareGeneratorConsumerPackageAssets");
    }

    [Fact]
    public void Build_Task_Shadow_Copies_Are_Not_Removed_During_Consumer_Builds()
    {
        var repositoryTargets = XDocument.Load(GetRepoFile("build/AtomUI.Repository.targets"));
        var stagingTarget = repositoryTargets.Descendants("Target")
                                              .Single(element =>
                                                  (string?)element.Attribute("Name") ==
                                                  "_AtomUIStageBuildTasksToolset");

        stagingTarget.Descendants()
                     .Where(element => element.Name.LocalName == "RemoveDir")
                     .ShouldBeEmpty();
    }

    [Fact]
    public void Build_Task_Shadow_Copies_Are_Staged_For_Cross_Targeting_Pack_Targets()
    {
        var repositoryTargets = XDocument.Load(GetRepoFile("build/AtomUI.Repository.targets"));
        var stagingTarget = repositoryTargets.Descendants("Target")
                                              .Single(element =>
                                                  (string?)element.Attribute("Name") ==
                                                  "_AtomUIStageBuildTasksToolset");

        ((string?)stagingTarget.Attribute("BeforeTargets")).ShouldNotBeNull()
            .ShouldContain("AtomUIPrepareLanguageModuleAssets");
        ((string?)stagingTarget.Attribute("Condition")).ShouldNotBeNull()
            .ShouldNotContain("IsCrossTargetingBuild");
    }

    [Fact]
    public void Generator_Build_Assets_Have_One_Explicit_Manifest()
    {
        var repositoryProps = XDocument.Load(GetRepoFile("build/AtomUI.Repository.props"));
        var buildAssets = repositoryProps.Descendants("AtomUINuGetBuildAsset").ShouldHaveSingleItem();
        var includes = ((string?)buildAssets.Attribute("Include"))
            .ShouldNotBeNull()
            .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(include => include.Replace("$(MSBuildThisFileDirectory)", string.Empty,
                                               StringComparison.Ordinal))
            .ToArray();
        includes.ShouldBe(s_expectedNuGetBuildAssets);
        buildAssets.Attribute("Exclude").ShouldBeNull();
        buildAssets.Elements("PackagePath")
                   .ShouldHaveSingleItem()
                   .Value.ShouldBe("buildTransitive/%(Filename)%(Extension)");

        var toolAssets = repositoryProps.Descendants("AtomUIGeneratorToolAsset").ShouldHaveSingleItem();
        var toolIncludes = ((string?)toolAssets.Attribute("Include")).ShouldNotBeNull();
        toolIncludes.ShouldContain("AtomUI.Generator.dll");
        toolIncludes.ShouldContain("AtomUI.Build.Tasks.dll");
        toolAssets.Elements("PackagePath")
                  .ShouldHaveSingleItem()
                  .Value.ShouldBe("tools/netstandard2.0/%(Filename)%(Extension)");

        var generatorProject = XDocument.Load(GetRepoFile("src/AtomUI.Generator/AtomUI.Generator.csproj"));
        generatorProject.Descendants("ProjectReference")
                        .ShouldNotContain(element =>
                            ((string?)element.Attribute("Include") ?? string.Empty)
                            .Contains("AtomUI.Generator.LinkedPublish", StringComparison.Ordinal));
        generatorProject.Descendants("None")
                        .ShouldContain(element =>
                            (string?)element.Attribute("Include") == "@(AtomUINuGetBuildAsset)");
        generatorProject.Descendants("None")
                        .ShouldContain(element =>
                            (string?)element.Attribute("Include") == "@(AtomUIGeneratorToolAsset)");

        var repositoryTargets = XDocument.Load(GetRepoFile("build/AtomUI.Repository.targets"));
        repositoryTargets.Descendants("None")
                         .ShouldContain(element =>
                             (string?)element.Attribute("Include") == "@(AtomUINuGetBuildAsset)");
        repositoryTargets.Descendants("None")
                         .ShouldContain(element =>
                             (string?)element.Attribute("Include") == "@(AtomUIGeneratorToolAsset)");

        var linkedPackTarget = generatorProject.Descendants("Target")
                                               .Single(element =>
                                                   (string?)element.Attribute("Name") ==
                                                   "AtomUIPrepareLinkedPublishGeneratorForPack");
        ((string?)linkedPackTarget.Attribute("BeforeTargets")).ShouldBe("_GetPackageFiles");
        linkedPackTarget.Descendants("MSBuild")
                        .Single()
                        .Attribute("Projects")!
                        .Value.ShouldContain("AtomUI.Generator.LinkedPublish.csproj");
        ((string?)linkedPackTarget.Descendants("MSBuild").Single().Attribute("Condition"))
            .ShouldBe("'$(NoBuild)' != 'true'");
        linkedPackTarget.Descendants("Error")
                        .Single()
                        .Attribute("Condition")!
                        .Value.ShouldContain("AtomUILinkedPublishGeneratorAssembly");
    }

    [Fact]
    public void NuGet_Features_Use_The_Shared_Build_Tasks_Assembly()
    {
        var repositoryProps = XDocument.Load(GetRepoFile("build/AtomUI.Repository.props"));
        repositoryProps.Descendants("AtomUIBuildTasksAssembly")
                       .ShouldHaveSingleItem()
                       .Value.ShouldContain("AtomUI.Build.Tasks.dll");

        var generatorProps = XDocument.Load(GetRepoFile("build/AtomUI.Generator.props"));
        var fallback = generatorProps.Descendants("AtomUIBuildTasksAssembly").ShouldHaveSingleItem();
        ((string?)fallback.Attribute("Condition"))
            .ShouldBe("'$(AtomUIBuildTasksAssembly)' == ''");
        fallback.Value.Trim().ShouldBe(
            "$(MSBuildThisFileDirectory)../tools/netstandard2.0/AtomUI.Build.Tasks.dll");

        var buildRoot = Path.Combine(GetRepositoryRoot(), "build");
        var featureFiles = s_expectedNuGetBuildAssets
            .Where(file => Path.GetExtension(file) == ".targets")
            .Select(file => Path.Combine(buildRoot, file));
        var usingTasks = featureFiles.SelectMany(file => XDocument.Load(file).Descendants("UsingTask"))
                                     .ToArray();
        usingTasks.ShouldNotBeEmpty();
        usingTasks.ShouldAllBe(element =>
            (string?)element.Attribute("AssemblyFile") == "$(AtomUIBuildTasksAssembly)");

        File.Exists(Path.Combine(buildRoot, "BuildTasks.props")).ShouldBeFalse();
        var buildText = string.Join('\n', Directory.EnumerateFiles(buildRoot).Select(File.ReadAllText));
        buildText.ShouldNotContain("AtomUILocalizationBuildTasksAssembly");
        buildText.ShouldNotContain("AtomUILinkedRegistrationBuildTasksAssembly");
    }

    [Fact]
    public void NuGet_Build_Tasks_Load_From_Shadow_Copy_In_Process()
    {
        var buildRoot = Path.Combine(GetRepositoryRoot(), "build");
        var usingTasks = s_expectedNuGetBuildAssets
            .Where(file => Path.GetExtension(file) == ".targets")
            .Select(file => Path.Combine(buildRoot, file))
            .SelectMany(file => XDocument.Load(file)
                .Descendants()
                .Where(element => element.Name.LocalName == "UsingTask"))
            .Where(element =>
                (string?)element.Attribute("AssemblyFile") == "$(AtomUIBuildTasksAssembly)")
            .ToArray();

        usingTasks.ShouldNotBeEmpty();
        usingTasks.ShouldAllBe(element =>
            element.Attribute("Runtime") == null);
        usingTasks.ShouldAllBe(element =>
            element.Attribute("TaskFactory") == null);
    }

    [Fact]
    public void Repository_Test_Project_Flag_Has_A_Canonical_Boolean_Value()
    {
        var defaults = XDocument.Load(GetRepoFile("build/ProjectDefaults.props"));
        var isTestProject = defaults.Descendants("IsTestProject").ShouldHaveSingleItem();

        isTestProject.Value.ShouldBe("true");
    }

    [Fact]
    public void Repository_Defaults_Exclude_Stale_Project_Local_Obj_Files()
    {
        var defaults = XDocument.Load(GetRepoFile("build/ProjectDefaults.props"));
        var excludes = defaults.Descendants("DefaultItemExcludes").ShouldHaveSingleItem();

        excludes.Value.ShouldContain("$(MSBuildProjectDirectory)/obj/**");
    }

    [Fact]
    public void Repository_Output_Paths_Centralize_Binaries_And_Intermediate_Files()
    {
        var outputPaths = XDocument.Load(GetRepoFile("build/OutputPaths.props"));

        outputPaths.Descendants("PackageOutputPath")
                   .ShouldHaveSingleItem()
                   .Value.ShouldBe("$(MSBuildThisFileDirectory)../.artifacts/Nuget/$(Configuration)");
        outputPaths.Descendants("OutputPathWithoutFramework")
                   .ShouldHaveSingleItem()
                   .Value.ShouldBe("$(MSBuildThisFileDirectory)../.artifacts/bin/$(Configuration)");
        outputPaths.Descendants("OutputPath")
                   .ShouldHaveSingleItem()
                   .Value.ShouldBe("$(OutputPathWithoutFramework)");
        outputPaths.Descendants("BaseIntermediateOutputPath")
                   .ShouldHaveSingleItem()
                   .Value.ShouldBe(
                       "$(MSBuildThisFileDirectory)../.artifacts/$(MSBuildProjectName)/obj");
    }

    [Fact]
    public void Tool_Source_Files_Stay_Trackable_While_Tool_Build_Outputs_Are_Ignored()
    {
        IsIgnoredByGit("tools/AtomUI.Docs.LLMsGenerator/Program.cs").ShouldBeFalse();
        IsIgnoredByGit("tools/performances/AtomUI.Performance/Program.cs").ShouldBeFalse();
        IsIgnoredByGit("tools/AtomUI.Docs.LLMsGenerator/bin/Debug/tool.dll").ShouldBeTrue();
        IsIgnoredByGit("tools/performances/AtomUI.Performance/obj/project.assets.json").ShouldBeTrue();
    }

    [Fact]
    public void Repository_Targets_Exclude_Compiler_Generated_Files_Once()
    {
        const string generatedFilesPattern = "$(CompilerGeneratedFilesOutputPath)/**/*.cs";
        var repositoryTargets = XDocument.Load(GetRepoFile("build/AtomUI.Repository.targets"));
        var removal = repositoryTargets.Descendants("Compile")
                                       .Where(element =>
                                           (string?)element.Attribute("Remove") == generatedFilesPattern)
                                       .ShouldHaveSingleItem();

        ((string?)removal.Parent?.Attribute("Condition"))
            .ShouldBe("'$(CompilerGeneratedFilesOutputPath)' != ''");

        var repositoryRoot = GetRepositoryRoot();
        var projectFiles = new[] { "src", "controlgallery", "tests", "tools" }
            .Select(directory => Path.Combine(repositoryRoot, directory))
            .Where(Directory.Exists)
            .SelectMany(directory => Directory.EnumerateFiles(
                directory,
                "*.csproj",
                SearchOption.AllDirectories));
        foreach (var projectFile in projectFiles)
        {
            var project = XDocument.Load(projectFile);
            if (!project.Descendants("CompilerGeneratedFilesOutputPath").Any())
            {
                continue;
            }

            project.Descendants("Compile")
                   .ShouldNotContain(element =>
                       (string?)element.Attribute("Remove") == generatedFilesPattern,
                       projectFile);
        }
    }

    [Fact]
    public void Repository_Default_Items_Always_Exclude_Stale_Generated_Files()
    {
        var projectDefaults = XDocument.Load(GetRepoFile("build/ProjectDefaults.props"));
        var excludes = projectDefaults.Descendants("DefaultItemExcludes")
                                      .ShouldHaveSingleItem()
                                      .Value;

        excludes.ShouldContain("$(MSBuildProjectDirectory)/obj/**");
        excludes.ShouldContain("$(MSBuildProjectDirectory)/GeneratedFiles/**");
    }

    [Fact]
    public void Desktop_Project_Does_Not_Keep_Stale_Window_Directory_Exclusions()
    {
        var project = File.ReadAllText(GetRepoFile(
            "src/AtomUI.Desktop.Controls/AtomUI.Desktop.Controls.csproj"));

        project.ShouldNotContain("Window\\Reflection");
        project.ShouldNotContain("Window\\Visuals");
    }

    [Fact]
    public void NuGet_Generator_Entry_Points_Import_Flat_Feature_Files()
    {
        GetImports("build/AtomUI.Generator.props").ShouldBe([
            "$(MSBuildThisFileDirectory)AtomUI.LinkedRegistration.props",
            "$(MSBuildThisFileDirectory)AtomUI.Localization.props"
        ]);
        GetImports("build/AtomUI.Generator.targets").ShouldBe([
            "$(MSBuildThisFileDirectory)AtomUI.LinkedRegistration.targets",
            "$(MSBuildThisFileDirectory)AtomUI.Localization.targets",
            "$(MSBuildThisFileDirectory)AtomUI.ThemeAssets.targets"
        ]);
    }

    [Fact]
    public void Localization_Targets_Own_The_Complete_Build_Integration()
    {
        var targets = XDocument.Load(GetRepoFile("build/AtomUI.Localization.targets"));

        GetImports("build/AtomUI.Localization.targets").ShouldBeEmpty();
        targets.Descendants("AdditionalFiles").ShouldNotBeEmpty();
        targets.Descendants("CompilerVisibleItemMetadata").ShouldNotBeEmpty();
        GetTargetNames(targets).ShouldBe([
            "AtomUIGetLanguageModuleSourceAssets",
            "AtomUIResolveLanguageContractProjectReferences",
            "AtomUIInferLanguagePackageContractPolicy",
            "AtomUIGetLanguagePackProjectAssets",
            "AtomUIResolveLanguagePackProjectReferences",
            "AtomUIExportLanguageTemplates",
            "AtomUIPrepareLanguagePackage",
            "AtomUIPrepareLanguageModuleAssets"
        ]);
    }

    [Fact]
    public void Build_Root_Is_A_Flat_Explicit_MSBuild_Surface()
    {
        var buildRoot = Path.Combine(GetRepositoryRoot(), "build");
        Directory.EnumerateDirectories(buildRoot).ShouldBeEmpty();
        Directory.EnumerateFiles(buildRoot)
                 .Select(Path.GetFileName)
                 .ShouldBe(s_expectedBuildFiles, ignoreOrder: true);
        Directory.EnumerateFiles(buildRoot)
                 .All(file => Path.GetExtension(file) is ".props" or ".targets")
                 .ShouldBeTrue();
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

    private static bool IsIgnoredByGit(string relativePath)
    {
        using var process = Process.Start(new ProcessStartInfo
        {
            FileName               = "git",
            WorkingDirectory       = GetRepositoryRoot(),
            RedirectStandardOutput = true,
            RedirectStandardError  = true,
            ArgumentList =
            {
                "check-ignore",
                "--quiet",
                "--no-index",
                "--",
                relativePath
            }
        });
        process.ShouldNotBeNull();
        process.WaitForExit();
        process.ExitCode.ShouldBeOneOf(0, 1);
        return process.ExitCode == 0;
    }
}
