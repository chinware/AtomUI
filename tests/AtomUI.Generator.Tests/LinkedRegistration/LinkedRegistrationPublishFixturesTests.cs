using System.Xml.Linq;
using Shouldly;
using Xunit;

namespace AtomUI.Generator.Tests.LinkedRegistration;

public sealed class LinkedRegistrationPublishFixturesTests
{
    private static readonly string[] FixtureNames =
    [
        "Minimal",
        "TwoUnits",
        "DynamicFallback",
        "Full"
    ];

    [Fact]
    public void Publish_fixtures_share_one_recorder_and_reference_the_real_packages()
    {
        foreach (var fixtureName in FixtureNames)
        {
            var project = XDocument.Load(GetRepoFile(
                $"tests/AtomUI.LinkedRegistration.Fixtures/{fixtureName}/{fixtureName}.csproj"));

            project.Descendants().ShouldContain(element =>
                element.Name.LocalName == "OutputType" && element.Value.Trim() == "Exe");
            project.Descendants().ShouldContain(element =>
                element.Name.LocalName == "ProjectReference" &&
                ((string?)element.Attribute("Include") ?? string.Empty)
                .EndsWith("src/AtomUI.Desktop.Controls/AtomUI.Desktop.Controls.csproj", StringComparison.Ordinal));
            project.Descendants().ShouldContain(element =>
                element.Name.LocalName == "ProjectReference" &&
                (string?)element.Attribute("OutputItemType") == "Analyzer" &&
                (string?)element.Attribute("ReferenceOutputAssembly") == "false");
            project.Descendants().ShouldContain(element =>
                element.Name.LocalName == "Compile" &&
                ((string?)element.Attribute("Include") ?? string.Empty)
                .EndsWith("Shared/FixtureHost.cs", StringComparison.Ordinal));
        }

        var minimalProject = XDocument.Load(GetRepoFile(
            "tests/AtomUI.LinkedRegistration.Fixtures/Minimal/Minimal.csproj"));
        minimalProject.Descendants().ShouldContain(element =>
            element.Name.LocalName == "ProjectReference" &&
            ((string?)element.Attribute("Include") ?? string.Empty)
            .EndsWith("UnusedUnit/UnusedUnit.csproj", StringComparison.Ordinal) &&
            ((string?)element.Attribute("Condition") ?? string.Empty)
            .Contains("AtomUIIncludeUnusedFixtureUnit", StringComparison.Ordinal));

        var unusedUnitProject = XDocument.Load(GetRepoFile(
            "tests/AtomUI.LinkedRegistration.Fixtures/UnusedUnit/UnusedUnit.csproj"));
        unusedUnitProject.Descendants().ShouldContain(element =>
            element.Name.LocalName == "ProjectReference" &&
            ((string?)element.Attribute("Include") ?? string.Empty)
            .EndsWith("src/AtomUI.Core/AtomUI.Core.csproj", StringComparison.Ordinal));

        var unusedUnitSource = ReadRepoFile(
            "tests/AtomUI.LinkedRegistration.Fixtures/UnusedUnit/UnusedUnitFragment.cs");
        unusedUnitSource.ShouldContain("AtomUI.Linked.Package.v1");
        unusedUnitSource.ShouldContain("AtomUI.Linked.Unit.v1");
        unusedUnitSource.ShouldContain(
            "AtomUI.LinkedRegistration.Fixtures.UnusedUnit%2FFixtureUnused");
        unusedUnitSource.ShouldNotContain("AtomUI.Desktop.Controls%2FFixtureUnused");
        unusedUnitSource.ShouldContain("public static class UnusedUnitRegistration");
        unusedUnitSource.ShouldContain("[ControlPackageRegistrationEntry]");
        unusedUnitSource.ShouldContain("public static class UnusedUnitFragment");
        unusedUnitSource.ShouldContain("AotTrimControlPackageRegistrationBuilder builder");
        unusedUnitSource.ShouldNotContain("TryEnterUnit");
    }

    [Fact]
    public void Fixture_programs_define_the_four_registration_modes()
    {
        var minimal = ReadRepoFile(
            "tests/AtomUI.LinkedRegistration.Fixtures/Minimal/Program.cs");
        minimal.ShouldContain("typeof(AtomUI.Desktop.Controls.Button)");
        minimal.ShouldContain("builder.UseDesktopControls()");
        minimal.ShouldNotContain("DatePicker");
        minimal.ShouldNotContain("UseAllDesktopControls");

        var twoUnits = ReadRepoFile(
            "tests/AtomUI.LinkedRegistration.Fixtures/TwoUnits/Program.cs");
        twoUnits.ShouldContain("typeof(AtomUI.Desktop.Controls.Button)");
        twoUnits.ShouldContain("typeof(AtomUI.Desktop.Controls.DatePicker)");
        twoUnits.ShouldContain("builder.UseDesktopControls()");

        var dynamicFallback = ReadRepoFile(
            "tests/AtomUI.LinkedRegistration.Fixtures/DynamicFallback/Program.cs");
        dynamicFallback.ShouldContain("builder.UseDesktopControls()");
        dynamicFallback.ShouldNotContain("UseAllDesktopControls");
        var dynamicProject = XDocument.Load(GetRepoFile(
            "tests/AtomUI.LinkedRegistration.Fixtures/DynamicFallback/DynamicFallback.csproj"));
        dynamicProject.Descendants().ShouldContain(element =>
            element.Name.LocalName == "AtomUIPackageRoot" &&
            (string?)element.Attribute("Include") == "AtomUI.Desktop.Controls");

        var full = ReadRepoFile(
            "tests/AtomUI.LinkedRegistration.Fixtures/Full/Program.cs");
        full.ShouldContain("builder.UseAllDesktopControls()");
        full.ShouldNotContain("builder.UseDesktopControls()");
    }

    [Fact]
    public void Fixture_host_emits_stable_package_core_and_registration_observations()
    {
        var source = ReadRepoFile(
            "tests/AtomUI.LinkedRegistration.Fixtures/Shared/FixtureHost.cs");

        source.ShouldContain("ControlIdentities");
        source.ShouldContain("ThemeAssetUris");
        source.ShouldContain("ProviderId");
        source.ShouldContain("ProviderResourceCount");
        source.ShouldContain("CatalogIds");
        source.ShouldContain("TranslationBundleIds");
        source.ShouldContain("InitializerCount");
        source.ShouldContain("Utf8JsonWriter");
        source.ShouldNotContain("JsonSerializer.Serialize");
        source.ShouldNotContain("Assembly.GetTypes");
    }

    [Fact]
    public void Verification_script_enforces_publish_modes_and_size_gates()
    {
        var source = ReadRepoFile("scripts/verification/verify-aot-trim-registration.sh");

        source.ShouldContain("local build_args=(");
        source.ShouldContain("build \"$project\"");
        source.ShouldContain("restore_fixture()");
        source.ShouldContain("local restore_args=(");
        source.ShouldContain("restore \"$(fixture_project \"$fixture\")\"");
        source.ShouldContain("restore_args+=(-p:AtomUILinkedPublish=true)");
        source.ShouldContain("restore_fixture \"$fixture\" false");
        source.ShouldContain("restore_fixture \"$fixture\" true");
        source.ShouldContain("--no-restore");
        source.ShouldContain("local fixture_assembly=");
        source.ShouldContain(".artifacts/bin/$configuration/net10.0");
        source.ShouldContain("dotnet \"$fixture_assembly\"");
        source.ShouldNotContain("local run_args=(run");
        source.ShouldNotContain("dotnet run");
        source.ShouldNotContain("properties[@]");
        source.ShouldNotContain("-flp:logfile=");
        source.ShouldContain("--disable-build-servers");
        source.ShouldContain("-m:1");
        source.ShouldContain("PublishTrimmed=true");
        source.ShouldContain("PublishAot=true");
        source.ShouldContain("build/MacOSHomebrewNativeAot.targets");
        source.ShouldContain("CustomAfterMicrosoftCommonTargets");
        source.ShouldContain("RunAOTCompilation=true");
        source.ShouldContain("AtomUILinkedPublish=true");
        source.ShouldContain("AtomUIEmitVerificationPlan=true");
        source.ShouldContain("verify_minimal_plan");
        source.ShouldContain("GeneratedRegistrationUnit_Buttons_");
        source.ShouldContain("GeneratedRegistrationUnit_Space_");
        source.ShouldContain("for forbidden_unit in DropdownButton SplitButton Flyouts Menu TreeView Dialog Tooltip DatePicker");
        source.ShouldContain("GeneratedRegistrationUnit_${forbidden_unit}_");
        source.ShouldContain("AtomUIUseGeneratedRegistration must remain inert");
        source.ShouldContain("MINIMUM_DESKTOP_REDUCTION_PERCENT=40");
        source.ShouldContain("MAX_SECOND_UNIT_GROWTH_BYTES=262144");
        source.ShouldContain("MAX_MINIMAL_MAIN_BYTES_OSX_ARM64=18874368");
        source.ShouldContain("! -path '*/.dSYM/*'");
        source.ShouldContain("! -name '*.pdb'");
        source.ShouldContain("! -name '*.dbg'");
        source.ShouldContain("{ print $8 }");
        source.ShouldContain("if (( reduction_percent < MINIMUM_DESKTOP_REDUCTION_PERCENT )); then");
        source.ShouldContain("if (( second_unit_growth > MAX_SECOND_UNIT_GROWTH_BYTES )); then");
        source.ShouldContain("minimal_main_size > MAX_MINIMAL_MAIN_BYTES_OSX_ARM64");
        source.ShouldContain("NativeAOT Minimal main executable reduction");
        source.ShouldContain("Unused Unit NativeAOT main executable growth");
        source.ShouldContain("NativeAOT Minimal main executable is");
        source.ShouldNotContain("(( reduction_percent >= MINIMUM_DESKTOP_REDUCTION_PERCENT ))");
        source.ShouldNotContain("(( second_unit_growth <= MAX_SECOND_UNIT_GROWTH_BYTES ))");
        source.ShouldContain("MinimalWithUnusedUnit");
        source.ShouldContain("AtomUIIncludeUnusedFixtureUnit=true");
        source.ShouldNotContain("two_units_size=");
        source.ShouldNotContain("two_units_size - minimal_size");
        source.ShouldContain("AtomUI.AotTrimRegistration.Enabled");
        source.ShouldContain("AtomUI.Linked.Plan.v1");
        source.ShouldContain("size-report.tsv");
        source.ShouldContain("package_core_fingerprint");
        source.ShouldContain("cmp -s \"$output_root/trimmed.Minimal.json\" \"$output_root/Minimal.generated.json\"");
        source.ShouldContain("DynamicFallback|Full)");
        source.ShouldContain("cmp -s \"$output_root/aot.$fixture.json\" \"$output_root/$fixture.ordinary.json\"");
    }

    [Fact]
    public void Shared_macos_native_aot_settings_supply_homebrew_library_paths()
    {
        var project = XDocument.Load(GetRepoFile("build/MacOSHomebrewNativeAot.targets"));
        var linkerArgs = project.Descendants()
            .Where(element => element.Name.LocalName == "LinkerArg")
            .Select(element => (string?)element.Attribute("Include"))
            .Where(value => value is not null)
            .ToArray();

        linkerArgs.ShouldContain("-L/opt/homebrew/lib");
        linkerArgs.ShouldContain("-L/opt/homebrew/opt/openssl@3/lib");
        linkerArgs.ShouldContain("-L/usr/local/lib");
        linkerArgs.ShouldContain("-L/usr/local/opt/openssl@3/lib");

        var galleryProject = XDocument.Load(GetRepoFile(
            "controlgallery/AtomUIGallery.Desktop/AtomUIGallery.Desktop.csproj"));
        galleryProject.Descendants("Import")
                      .Single(element =>
                          (string?)element.Attribute("Project") ==
                          "../../build/MacOSHomebrewNativeAot.targets")
                      .ShouldNotBeNull();

        var languagePackConsumer = XDocument.Load(GetRepoFile(
            "tests/fixtures/LanguagePackEndToEnd/Consumer/Consumer.csproj"));
        languagePackConsumer.Descendants("Import")
                            .Single(element =>
                                (string?)element.Attribute("Project") ==
                                "../../../../build/MacOSHomebrewNativeAot.targets")
                            .ShouldNotBeNull();
    }

    private static string ReadRepoFile(string relativePath)
    {
        return File.ReadAllText(GetRepoFile(relativePath));
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
