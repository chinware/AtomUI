using System.Diagnostics;
using System.IO.Compression;
using System.Xml.Linq;
using Shouldly;
using Xunit;

namespace AtomUI.Localization.IntegrationTests;

public sealed partial class LanguagePackEndToEndTests
{
    private static readonly TimeSpan s_stageTimeout = TimeSpan.FromSeconds(120);

    [Fact]
    public void Language_Pack_Fixtures_Cover_Verified_And_Deferred_Authoring()
    {
        var repositoryRoot = FindRepositoryRoot();
        var fixtureRoot = Path.Combine(repositoryRoot, "tests", "fixtures", "LanguagePackEndToEnd");
        var verifiedProject = XDocument.Load(
            Path.Combine(fixtureRoot, "LanguagePack", "LanguagePack.csproj"));
        verifiedProject.Descendants("AtomUILanguage").ShouldBeEmpty();
        verifiedProject.Descendants("AtomUIRequireVerifiedLanguageContract")
                       .ShouldHaveSingleItem()
                       .Value.ShouldBe("true");
        verifiedProject.Descendants("ProjectReference")
                       .Any(reference =>
                           ((string?)reference.Attribute("Include"))?.EndsWith(
                               "Module/Module.csproj",
                               StringComparison.Ordinal) == true)
                       .ShouldBeTrue();

        var deferredProject = XDocument.Load(
            Path.Combine(fixtureRoot, "OptionalLanguagePack", "OptionalLanguagePack.csproj"));
        deferredProject.Descendants("AtomUILanguage").ShouldBeEmpty();
        deferredProject.Descendants("AtomUIRequireVerifiedLanguageContract").ShouldBeEmpty();
        deferredProject.Descendants("ProjectReference")
                       .Any(reference =>
                           ((string?)reference.Attribute("Include"))?.EndsWith(
                               "OptionalModule/OptionalModule.csproj",
                               StringComparison.Ordinal) == true)
                       .ShouldBeFalse();

        var templateProject = XDocument.Load(
            Path.Combine(fixtureRoot, "TemplateExport", "TemplateExport.csproj"));
        var componentPackage = templateProject.Descendants("PackageReference")
                                              .Single(reference =>
                                                  (string?)reference.Attribute("Include") ==
                                                  "Acme.LocalizationComponent");
        ((string?)componentPackage.Attribute("PrivateAssets")).ShouldBe("all");
    }

    [Fact(Timeout = 360_000)]
    public async Task Aggregate_Language_Pack_Dormantly_Consumes_Unreferenced_Module_Through_NuGet()
    {
        var repositoryRoot = FindRepositoryRoot();
        var fixtureRoot = Path.Combine(repositoryRoot, "tests", "fixtures", "LanguagePackEndToEnd");
        var temporaryRoot = Path.Combine(
            Path.GetTempPath(),
            "AtomUI.Localization.IntegrationTests",
            Guid.NewGuid().ToString("N"));
        var feed = Path.Combine(temporaryRoot, "feed");
        var packages = Path.Combine(temporaryRoot, "packages");
        var packageVersion = "1.0.0-local." + Guid.NewGuid().ToString("N");
        var globalPackages = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".nuget",
            "packages");
        Directory.CreateDirectory(feed);
        Directory.CreateDirectory(packages);

        try
        {
            await RunProcess(
                "Pack module",
                repositoryRoot,
                temporaryRoot,
                "dotnet",
                "msbuild",
                Path.Combine(fixtureRoot, "Module", "Module.csproj"),
                "-restore",
                "-t:Pack",
                "-m:1",
                "-nr:false",
                "-p:Configuration=Debug",
                $"-p:Version={packageVersion}",
                $"-p:PackageOutputPath={feed}",
                $"-p:RestorePackagesPath={globalPackages}",
                "-p:NoPackageAnalysis=true");
            var verifiedPackResult = await RunProcess(
                "Pack language pack",
                repositoryRoot,
                temporaryRoot,
                "dotnet",
                "msbuild",
                Path.Combine(fixtureRoot, "LanguagePack", "LanguagePack.csproj"),
                "-restore",
                "-t:Pack",
                "-m:1",
                "-nr:false",
                "-p:Configuration=Debug",
                $"-p:PackageVersion={packageVersion}",
                $"-p:PackageOutputPath={feed}",
                $"-p:RestorePackagesPath={globalPackages}");
            verifiedPackResult.Output.ShouldNotContain("ATOMUILOC010");
            await RunProcess(
                "Pack optional module",
                repositoryRoot,
                temporaryRoot,
                "dotnet",
                "msbuild",
                Path.Combine(fixtureRoot, "OptionalModule", "OptionalModule.csproj"),
                "-restore",
                "-t:Pack",
                "-m:1",
                "-nr:false",
                "-p:Configuration=Debug",
                $"-p:Version={packageVersion}",
                $"-p:PackageOutputPath={feed}",
                $"-p:RestorePackagesPath={globalPackages}",
                "-p:NoPackageAnalysis=true");
            var deferredPackResult = await RunProcess(
                "Pack optional language pack",
                repositoryRoot,
                temporaryRoot,
                "dotnet",
                "msbuild",
                Path.Combine(fixtureRoot, "OptionalLanguagePack", "OptionalLanguagePack.csproj"),
                "-restore",
                "-t:Pack",
                "-m:1",
                "-nr:false",
                "-p:Configuration=Debug",
                $"-p:PackageVersion={packageVersion}",
                $"-p:PackageOutputPath={feed}",
                $"-p:RestorePackagesPath={globalPackages}");
            CountOccurrences(deferredPackResult.Output, "ATOMUILOC010").ShouldBe(1);
            await RunProcess(
                "Pack aggregate language pack",
                repositoryRoot,
                temporaryRoot,
                "dotnet",
                "msbuild",
                Path.Combine(fixtureRoot, "AggregateLanguagePack", "AggregateLanguagePack.csproj"),
                "-restore",
                "-t:Pack",
                "-m:1",
                "-nr:false",
                "-p:Configuration=Debug",
                $"-p:PackageVersion={packageVersion}",
                $"-p:PackageOutputPath={feed}",
                $"-p:RestorePackagesPath={globalPackages}");

            AssertPackageLayouts(feed, packageVersion);

            var consumerProject = Path.Combine(fixtureRoot, "Consumer", "Consumer.csproj");
            await RunProcess(
                "Restore consumer",
                repositoryRoot,
                temporaryRoot,
                "dotnet",
                "restore",
                consumerProject,
                "--disable-build-servers",
                "-m:1",
                "-nr:false",
                $"-p:FixturePackageVersion={packageVersion}",
                $"-p:RestoreAdditionalProjectSources={feed}",
                $"-p:RestorePackagesPath={packages}",
                $"-p:RestoreAdditionalProjectFallbackFolders={globalPackages}");
            var assetsFile = Path.Combine(
                repositoryRoot,
                "output",
                "Consumer",
                "obj",
                "project.assets.json");
            var assets = File.ReadAllText(assetsFile);
            assets.ShouldContain($"Acme.OptionalComponent.I18n.JaJP/{packageVersion}");
            assets.ShouldNotContain($"Acme.OptionalComponent/{packageVersion}");
            assets.ShouldNotContain("Acme.OptionalComponent.dll");
            var generatedPropsFile = Path.Combine(
                Path.GetDirectoryName(assetsFile)!,
                "Consumer.csproj.nuget.g.props");
            var generatedImports = XDocument.Load(generatedPropsFile)
                .Descendants()
                .Where(static element =>
                    string.Equals(element.Name.LocalName, "Import", StringComparison.Ordinal))
                .Select(static import => (string?)import.Attribute("Project"))
                .Where(static project => project is not null)
                .ToArray();
            generatedImports.ShouldContain(
                $"$(NuGetPackageRoot)/acme.optionalcomponent.i18n.jajp/{packageVersion}/" +
                "buildTransitive/Acme.OptionalComponent.I18n.JaJP.props");
            var consumerOutput = Path.Combine(temporaryRoot, "consumer-output");
            await RunProcess(
                "Build consumer",
                repositoryRoot,
                temporaryRoot,
                "dotnet",
                "build",
                consumerProject,
                "--no-restore",
                "--disable-build-servers",
                "-m:1",
                "-nr:false",
                "-p:Configuration=Debug",
                $"-p:OutputPath={consumerOutput}",
                $"-p:FixturePackageVersion={packageVersion}",
                $"-p:RestorePackagesPath={packages}",
                $"-p:RestoreAdditionalProjectFallbackFolders={globalPackages}");

            var consumerExecutable = Path.Combine(
                consumerOutput,
                OperatingSystem.IsWindows() ? "Consumer.exe" : "Consumer");
            Directory.EnumerateFiles(
                    Path.GetDirectoryName(consumerExecutable)!,
                    "Acme.OptionalComponent.dll",
                    SearchOption.TopDirectoryOnly)
                .ShouldBeEmpty();
            var runResult = await RunProcess(
                "Run consumer",
                repositoryRoot,
                temporaryRoot,
                consumerExecutable);
            runResult.Output.ShouldContain("ja-JP:Welcome=ようこそ;ItemCount=3 件の項目があります。");
        }
        finally
        {
            if (Directory.Exists(temporaryRoot))
            {
                Directory.Delete(temporaryRoot, recursive: true);
            }
        }
    }

    [Fact(Timeout = 360_000)]
    public async Task Language_Template_Is_Exported_From_A_Module_Package_Into_The_Current_Project()
    {
        var repositoryRoot = FindRepositoryRoot();
        var fixtureRoot = Path.Combine(repositoryRoot, "tests", "fixtures", "LanguagePackEndToEnd");
        var templateProjectDirectory = Path.Combine(fixtureRoot, "TemplateExport");
        var exportedTemplate = Path.Combine(
            templateProjectDirectory,
            "Localization",
            "Welcome",
            "ja-JP.xlf");
        var temporaryRoot = Path.Combine(
            Path.GetTempPath(),
            "AtomUI.Localization.IntegrationTests",
            Guid.NewGuid().ToString("N"));
        var feed = Path.Combine(temporaryRoot, "feed");
        var packages = Path.Combine(temporaryRoot, "packages");
        var packageVersion = "1.0.0-local." + Guid.NewGuid().ToString("N");
        Directory.CreateDirectory(feed);
        Directory.CreateDirectory(packages);

        try
        {
            File.Exists(exportedTemplate).ShouldBeFalse(
                $"The template export fixture must start empty: '{exportedTemplate}'.");
            await RunProcess(
                "Pack module for template export",
                repositoryRoot,
                temporaryRoot,
                "dotnet",
                "msbuild",
                Path.Combine(fixtureRoot, "Module", "Module.csproj"),
                "-t:Pack",
                "-m:1",
                "-nr:false",
                "-p:Configuration=Debug",
                $"-p:Version={packageVersion}",
                $"-p:PackageOutputPath={feed}",
                "-p:NoPackageAnalysis=true");

            var templateProject = Path.Combine(templateProjectDirectory, "TemplateExport.csproj");
            var globalPackages = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                ".nuget",
                "packages");
            await RunProcess(
                "Restore template export project",
                repositoryRoot,
                temporaryRoot,
                "dotnet",
                "restore",
                templateProject,
                "--disable-build-servers",
                $"-p:FixturePackageVersion={packageVersion}",
                $"-p:RestoreAdditionalProjectSources={feed}",
                $"-p:RestorePackagesPath={packages}",
                $"-p:RestoreAdditionalProjectFallbackFolders={globalPackages}");
            await RunProcess(
                "Export language template",
                repositoryRoot,
                temporaryRoot,
                "dotnet",
                "msbuild",
                templateProject,
                "-t:AtomUIExportLanguageTemplates",
                "-m:1",
                "-nr:false",
                "-p:Configuration=Debug",
                "-p:AtomUITargetLanguage=ja-JP",
                $"-p:FixturePackageVersion={packageVersion}",
                $"-p:RestorePackagesPath={packages}",
                $"-p:RestoreAdditionalProjectFallbackFolders={globalPackages}");

            File.Exists(exportedTemplate).ShouldBeTrue();
            var exported = XDocument.Load(exportedTemplate);
            XNamespace xliff = "urn:oasis:names:tc:xliff:document:2.0";
            var root = exported.Root.ShouldNotBeNull();
            ((string?)root.Attribute("version")).ShouldBe("2.1");
            ((string?)root.Attribute("srcLang")).ShouldBe("en-US");
            ((string?)root.Attribute("trgLang")).ShouldBe("ja-JP");

            var file = root.Element(xliff + "file").ShouldNotBeNull();
            ((string?)file.Attribute("id"))
                .ShouldBe("Acme.LocalizationComponent.Localization.WelcomeLangResourceKind");
            var units = file.Elements(xliff + "unit")
                            .ToDictionary(
                                static unit => (string)unit.Attribute("id")!,
                                StringComparer.Ordinal);
            AssertExportedUnit(units["Greeting"], xliff, "Welcome");
            AssertExportedUnit(units["ItemCount"], xliff, "You have {0} items.");

            var packageCacheTemplate = Path.Combine(
                packages,
                "acme.localizationcomponent",
                packageVersion,
                "contentFiles",
                "any",
                "any",
                "Localization",
                "Welcome",
                "ja-JP.xlf");
            File.Exists(packageCacheTemplate).ShouldBeFalse();
        }
        finally
        {
            if (File.Exists(exportedTemplate))
            {
                File.Delete(exportedTemplate);
            }
            if (Directory.Exists(temporaryRoot))
            {
                Directory.Delete(temporaryRoot, recursive: true);
            }
        }
    }

    private static void AssertPackageLayouts(string feed, string packageVersion)
    {
        var modulePackage = Path.Combine(
            feed,
            $"Acme.LocalizationComponent.{packageVersion}.nupkg");
        var languagePackage = Path.Combine(
            feed,
            $"Acme.LocalizationComponent.I18n.JaJP.{packageVersion}.nupkg");
        var optionalModulePackage = Path.Combine(
            feed,
            $"Acme.OptionalComponent.{packageVersion}.nupkg");

        var moduleEntries = PackageEntries(modulePackage);
        moduleEntries.ShouldContain("buildTransitive/Acme.LocalizationComponent.props");
        moduleEntries.ShouldContain("contentFiles/any/any/Localization/Welcome/en-US.xlf");
        moduleEntries.ShouldContain("lib/net10.0/Module.dll");
        moduleEntries.ShouldContain("lib/net10.0/Module.pdb");
        moduleEntries.Count(IsBuildAsset).ShouldBe(1);
        moduleEntries.Count(IsRuntimeAsset).ShouldBe(2);
        moduleEntries.ShouldNotContain(static path => IsUnexpectedBuildOrRuntimeAsset(path));

        var moduleFingerprint = AssertLanguageProps(
            modulePackage,
            "buildTransitive/Acme.LocalizationComponent.props",
            "ModuleBuiltIn",
            "Localization/Welcome/en-US.xlf",
            "Verified",
            "2");

        var languageEntries = PackageEntries(languagePackage);
        languageEntries.ShouldContain("buildTransitive/Acme.LocalizationComponent.I18n.JaJP.props");
        languageEntries.ShouldContain("contentFiles/any/any/AtomUI.LanguagePack.xml");
        languageEntries.ShouldContain("contentFiles/any/any/Welcome/ja-JP.xlf");
        languageEntries.Count(IsBuildAsset).ShouldBe(1);
        languageEntries.ShouldNotContain(static path => IsRuntimeAsset(path));
        languageEntries.ShouldNotContain(static path => IsUnexpectedBuildOrRuntimeAsset(path));

        var languageFingerprint = AssertLanguageProps(
            languagePackage,
            "buildTransitive/Acme.LocalizationComponent.I18n.JaJP.props",
            "StaticLanguagePack",
            "Welcome/ja-JP.xlf",
            "Verified",
            "2");
        languageFingerprint.ShouldBe(moduleFingerprint);

        var manifest = XDocument.Parse(PackageEntryText(
            languagePackage,
            "contentFiles/any/any/AtomUI.LanguagePack.xml"));
        var manifestCatalog = manifest.Descendants("catalog").ShouldHaveSingleItem();
        ((string?)manifestCatalog.Attribute("contractValidation")).ShouldBe("Verified");
        ((string?)manifestCatalog.Attribute("contractVersion")).ShouldBe("2");
        ((string?)manifestCatalog.Attribute("sourceFingerprint")).ShouldBe(languageFingerprint);

        var optionalModuleEntries = PackageEntries(optionalModulePackage);
        optionalModuleEntries.ShouldContain("lib/net10.0/Acme.OptionalComponent.dll");
        optionalModuleEntries.ShouldNotContain("lib/net10.0/OptionalModule.dll");

        AssertOptionalLanguagePackageLayout(feed, packageVersion);
        AssertAggregatePackageLayout(feed, packageVersion);
    }

    private static void AssertOptionalLanguagePackageLayout(string feed, string packageVersion)
    {
        var optionalLanguagePackage = Path.Combine(
            feed,
            $"Acme.OptionalComponent.I18n.JaJP.{packageVersion}.nupkg");
        var optionalLanguageEntries = PackageEntries(optionalLanguagePackage);
        optionalLanguageEntries.ShouldContain(
            "buildTransitive/Acme.OptionalComponent.I18n.JaJP.props");
        optionalLanguageEntries.ShouldContain("contentFiles/any/any/AtomUI.LanguagePack.xml");
        optionalLanguageEntries.ShouldContain("contentFiles/any/any/Optional/ja-JP.xlf");
        optionalLanguageEntries.Count(IsBuildAsset).ShouldBe(1);
        optionalLanguageEntries.ShouldNotContain(static path => IsRuntimeAsset(path));
        optionalLanguageEntries.ShouldNotContain(static path => IsUnexpectedBuildOrRuntimeAsset(path));

        var dependencies = PackageDependencies(
            optionalLanguagePackage,
            "Acme.OptionalComponent.I18n.JaJP.nuspec");
        dependencies.ShouldNotContainKey("Acme.OptionalComponent");

        var fingerprint = AssertLanguageProps(
            optionalLanguagePackage,
            "buildTransitive/Acme.OptionalComponent.I18n.JaJP.props",
            "StaticLanguagePack",
            "Optional/ja-JP.xlf",
            "Deferred",
            expectedContractVersion: null);
        var manifest = XDocument.Parse(PackageEntryText(
            optionalLanguagePackage,
            "contentFiles/any/any/AtomUI.LanguagePack.xml"));
        var catalog = manifest.Descendants("catalog").ShouldHaveSingleItem();
        ((string?)catalog.Attribute("contractValidation")).ShouldBe("Deferred");
        catalog.Attribute("contractVersion").ShouldBeNull();
        ((string?)catalog.Attribute("sourceFingerprint")).ShouldBe(fingerprint);
    }

    private static void AssertAggregatePackageLayout(string feed, string packageVersion)
    {
        var aggregatePackage = Path.Combine(
            feed,
            $"Acme.LocalizationAggregate.I18n.JaJP.{packageVersion}.nupkg");
        var aggregateEntries = PackageEntries(aggregatePackage);
        aggregateEntries.ShouldAllBe(static path => IsAggregatePackageMetadataEntry(path));

        var dependencies = PackageDependencies(
            aggregatePackage,
            "Acme.LocalizationAggregate.I18n.JaJP.nuspec");
        dependencies.Count.ShouldBe(2);
        dependencies["Acme.LocalizationComponent.I18n.JaJP"].ShouldBe($"[{packageVersion}]");
        dependencies["Acme.OptionalComponent.I18n.JaJP"].ShouldBe($"[{packageVersion}]");
    }

    private static void AssertExportedUnit(
        XElement unit,
        XNamespace xliff,
        string expectedSource)
    {
        unit.Attribute("name").ShouldBeNull();
        var segment = unit.Element(xliff + "segment").ShouldNotBeNull();
        segment.Element(xliff + "source").ShouldNotBeNull().Value.ShouldBe(expectedSource);
        var target = segment.Element(xliff + "target").ShouldNotBeNull();
        ((string?)target.Attribute("state")).ShouldBe("initial");
        target.Value.ShouldBeEmpty();
    }

    private static string AssertLanguageProps(
        string packagePath,
        string entryPath,
        string sourceKind,
        string packageLanguagePath,
        string contractValidation,
        string? expectedContractVersion)
    {
        var props = XDocument.Parse(PackageEntryText(packagePath, entryPath));
        var item = props.Descendants("AtomUILanguage").ShouldHaveSingleItem();
        ((string?)item.Attribute("AtomUILanguageSourceKind")).ShouldBe(sourceKind);
        ((string?)item.Attribute("AtomUILanguagePackagePath")).ShouldBe(packageLanguagePath);
        ((string?)item.Attribute("AtomUILanguageContractValidation")).ShouldBe(contractValidation);
        ((string?)item.Attribute("AtomUILanguageContractVersion")).ShouldBe(expectedContractVersion);
        var fingerprint = ((string?)item.Attribute("AtomUILanguageSourceFingerprint"))
            .ShouldNotBeNull();
        fingerprint.ShouldMatch("^[0-9a-f]{64}$");
        return fingerprint;
    }

    private static int CountOccurrences(string value, string fragment)
    {
        return value.Split(fragment, StringSplitOptions.None).Length - 1;
    }

    private static string[] PackageEntries(string packagePath)
    {
        File.Exists(packagePath).ShouldBeTrue($"Expected package '{packagePath}' was not created.");
        using var archive = ZipFile.OpenRead(packagePath);
        return archive.Entries.Select(static entry => entry.FullName).ToArray();
    }

    private static string PackageEntryText(string packagePath, string entryPath)
    {
        using var archive = ZipFile.OpenRead(packagePath);
        var entry = archive.GetEntry(entryPath).ShouldNotBeNull(
            $"Expected package '{packagePath}' to contain '{entryPath}'.");
        using var reader = new StreamReader(entry.Open());
        return reader.ReadToEnd();
    }

    private static IReadOnlyDictionary<string, string> PackageDependencies(
        string packagePath,
        string nuspecPath)
    {
        var nuspec = XDocument.Parse(PackageEntryText(packagePath, nuspecPath));
        return nuspec.Descendants().Where(static element =>
                string.Equals(element.Name.LocalName, "dependency", StringComparison.Ordinal))
            .ToDictionary(
            static dependency => ((string?)dependency.Attribute("id")).ShouldNotBeNull(),
            static dependency => ((string?)dependency.Attribute("version")).ShouldNotBeNull(),
            StringComparer.Ordinal);
    }

    private static bool IsBuildAsset(string path)
    {
        return path.StartsWith("build/", StringComparison.OrdinalIgnoreCase) ||
               path.StartsWith("buildMultiTargeting/", StringComparison.OrdinalIgnoreCase) ||
               path.StartsWith("buildTransitive/", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsRuntimeAsset(string path)
    {
        return path.StartsWith("lib/", StringComparison.OrdinalIgnoreCase) ||
               path.StartsWith("runtimes/", StringComparison.OrdinalIgnoreCase) ||
               path.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) ||
               path.EndsWith(".pdb", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsAggregatePackageMetadataEntry(string path)
    {
        const string corePropertiesPrefix = "package/services/metadata/core-properties/";

        if (path.Equals("_rels/.rels", StringComparison.Ordinal) ||
            path.Equals("[Content_Types].xml", StringComparison.Ordinal) ||
            path.Equals(
                "Acme.LocalizationAggregate.I18n.JaJP.nuspec",
                StringComparison.Ordinal))
        {
            return true;
        }

        if (!path.StartsWith(corePropertiesPrefix, StringComparison.Ordinal) ||
            !path.EndsWith(".psmdcp", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var fileName = path[corePropertiesPrefix.Length..];
        return fileName.IndexOf('/') < 0 &&
               Guid.TryParse(fileName[..^".psmdcp".Length], out _);
    }

    private static bool IsUnexpectedBuildOrRuntimeAsset(string path)
    {
        return path.StartsWith("analyzers/", StringComparison.OrdinalIgnoreCase) ||
               path.StartsWith("tools/", StringComparison.OrdinalIgnoreCase) ||
               path.EndsWith(".targets", StringComparison.OrdinalIgnoreCase) ||
               path.Contains("AtomUI.Build.Tasks", StringComparison.OrdinalIgnoreCase) ||
               path.Contains("AtomUI.Generator", StringComparison.OrdinalIgnoreCase);
    }

    private static async Task<ProcessResult> RunProcess(
        string stage,
        string workingDirectory,
        string temporaryRoot,
        string executable,
        params string[] arguments)
    {
        var result = await RunProcessUnchecked(
            stage,
            workingDirectory,
            temporaryRoot,
            executable,
            arguments);
        result.ExitCode.ShouldBe(0, FormatFailure(result, "failed"));
        return result;
    }

    private static async Task<ProcessResult> RunProcessUnchecked(
        string stage,
        string workingDirectory,
        string temporaryRoot,
        string executable,
        params string[] arguments)
    {
        var startInfo = new ProcessStartInfo(executable)
        {
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };
        startInfo.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1";
        startInfo.Environment["DOTNET_NOLOGO"] = "1";
        startInfo.Environment["DOTNET_CLI_HOME"] = Path.Combine(temporaryRoot, "cli");
        startInfo.Environment["NUGET_PACKAGES"] = Path.Combine(temporaryRoot, "packages");
        startInfo.Environment["DOTNET_CLI_USE_MSBUILD_SERVER"] = "0";
        startInfo.Environment["MSBUILDUSESERVER"] = "0";
        startInfo.Environment["UseSharedCompilation"] = "false";
        foreach (var argument in arguments)
        {
            startInfo.ArgumentList.Add(argument);
        }

        var command = FormatCommand(executable, arguments);
        using var process = Process.Start(startInfo) ?? throw new InvalidOperationException(
            $"The '{stage}' process could not be started.");
        var stopwatch = Stopwatch.StartNew();
        var standardOutput = process.StandardOutput.ReadToEndAsync();
        var standardError = process.StandardError.ReadToEndAsync();
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(
            TestContext.Current.CancellationToken);
        timeout.CancelAfter(s_stageTimeout);
        try
        {
            await process.WaitForExitAsync(timeout.Token);
        }
        catch (OperationCanceledException)
        {
            if (!process.HasExited)
            {
                process.Kill(entireProcessTree: true);
            }
            await process.WaitForExitAsync();
            var timedOutStandardOutput = await standardOutput;
            var timedOutStandardError = await standardError;
            var outcome = TestContext.Current.CancellationToken.IsCancellationRequested
                ? "was cancelled"
                : "timed out";
            throw new TimeoutException(
                FormatFailure(
                    stage,
                    command,
                    stopwatch.Elapsed,
                    exitCode: null,
                    timedOutStandardOutput,
                    timedOutStandardError,
                    outcome));
        }

        var result = new ProcessResult(
            stage,
            command,
            process.Id,
            stopwatch.Elapsed,
            process.ExitCode,
            await standardOutput,
            await standardError);
        return result;
    }

    private static string FormatCommand(string executable, IReadOnlyList<string> arguments)
    {
        return executable + " " + string.Join(' ', arguments);
    }

    private static string FormatFailure(ProcessResult result, string outcome)
    {
        return FormatFailure(
            result.Stage,
            result.Command,
            result.Elapsed,
            result.ExitCode,
            result.StandardOutput,
            result.StandardError,
            outcome);
    }

    private static string FormatFailure(
        string stage,
        string command,
        TimeSpan elapsed,
        int? exitCode,
        string standardOutput,
        string standardError,
        string outcome)
    {
        return $"{stage} {outcome} after {elapsed.TotalSeconds:F1}s.{Environment.NewLine}" +
               $"Command: {command}{Environment.NewLine}" +
               $"Exit code: {exitCode?.ToString() ?? "not available"}{Environment.NewLine}" +
               $"stdout:{Environment.NewLine}{standardOutput}{Environment.NewLine}" +
               $"stderr:{Environment.NewLine}{standardError}";
    }

    private static string FindRepositoryRoot()
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

        throw new DirectoryNotFoundException("The AtomUI repository root could not be located.");
    }

    private sealed record ProcessResult(
        string Stage,
        string Command,
        int ProcessId,
        TimeSpan Elapsed,
        int ExitCode,
        string StandardOutput,
        string StandardError)
    {
        public string Output => StandardOutput + StandardError;
    }
}
