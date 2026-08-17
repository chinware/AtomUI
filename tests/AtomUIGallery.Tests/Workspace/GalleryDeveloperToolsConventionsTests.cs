using System.Xml.Linq;
using Shouldly;
using Xunit;

namespace AtomUIGallery.Tests.Workspace;

public class GalleryDeveloperToolsConventionsTests
{
    [Theory]
    [InlineData("controlgallery/AtomUIGallery.Desktop/Program.cs",
                "controlgallery/AtomUIGallery.Desktop/AtomUIGallery.Desktop.csproj")]
    [InlineData("controlgallery/AtomUIGallery.Browser/Program.cs",
                "controlgallery/AtomUIGallery.Browser/AtomUIGallery.Browser.csproj")]
    public void Gallery_Hosts_Keep_Developer_Tools_Restore_Graph_Stable_And_Release_Assets_Clean(
        string programPath,
        string projectPath)
    {
        var program = ReadRepoFile(programPath);
        program.ShouldContain("#if DEBUG");
        program.ShouldContain(".WithDeveloperTools()");

        var project = XDocument.Load(GetRepoFile(projectPath));
        var diagnosticsReference = project.Descendants("PackageReference")
                                          .SingleOrDefault(element =>
                                              string.Equals(
                                                  (string?)element.Attribute("Include"),
                                                  "AvaloniaUI.DiagnosticsSupport",
                                                  StringComparison.Ordinal));

        diagnosticsReference.ShouldNotBeNull();
        diagnosticsReference.Attribute("Condition").ShouldBeNull(
            $"{projectPath} must restore the developer-tools package path in every configuration so " +
            "the Debug-only assembly reference remains valid after a Release restore.");
        (diagnosticsReference.Attribute("GeneratePathProperty")?.Value).ShouldBe("true");
        (diagnosticsReference.Attribute("ExcludeAssets")?.Value).ShouldBe("all");

        var diagnosticsAssemblyReference = project.Descendants("Reference")
                                                  .SingleOrDefault(element =>
                                                      string.Equals(
                                                          (string?)element.Attribute("Include"),
                                                          "AvaloniaUI.DiagnosticsSupport.Avalonia",
                                                          StringComparison.Ordinal));

        diagnosticsAssemblyReference.ShouldNotBeNull();
        (diagnosticsAssemblyReference.Attribute("Condition")?.Value).ShouldBe("'$(Configuration)' == 'Debug'");
        (diagnosticsAssemblyReference.Element("HintPath")?.Value).ShouldBe(
            "$(PkgAvaloniaUI_DiagnosticsSupport)/lib/net10.0/AvaloniaUI.DiagnosticsSupport.Avalonia.dll");
        (diagnosticsAssemblyReference.Element("Private")?.Value).ShouldBe("true");

        var runtimeDependencies = new[]
        {
            (
                Package: "Microsoft.Extensions.Logging.Abstractions",
                Assembly: "Microsoft.Extensions.Logging.Abstractions",
                HintPath:
                "$(PkgMicrosoft_Extensions_Logging_Abstractions)/lib/net8.0/Microsoft.Extensions.Logging.Abstractions.dll"),
            (
                Package: "Microsoft.Extensions.DependencyInjection.Abstractions",
                Assembly: "Microsoft.Extensions.DependencyInjection.Abstractions",
                HintPath:
                "$(PkgMicrosoft_Extensions_DependencyInjection_Abstractions)/lib/net8.0/Microsoft.Extensions.DependencyInjection.Abstractions.dll"),
            (
                Package: "Microsoft.IO.RecyclableMemoryStream",
                Assembly: "Microsoft.IO.RecyclableMemoryStream",
                HintPath:
                "$(PkgMicrosoft_IO_RecyclableMemoryStream)/lib/net6.0/Microsoft.IO.RecyclableMemoryStream.dll")
        };

        foreach (var runtimeDependency in runtimeDependencies)
        {
            var packageReference = project.Descendants("PackageReference")
                                          .SingleOrDefault(element =>
                                              string.Equals(
                                                  (string?)element.Attribute("Include"),
                                                  runtimeDependency.Package,
                                                  StringComparison.Ordinal));
            packageReference.ShouldNotBeNull(
                $"{projectPath} must restore {runtimeDependency.Package} for the Debug-only diagnostics reference.");
            packageReference.Attribute("Condition").ShouldBeNull();
            (packageReference.Attribute("GeneratePathProperty")?.Value).ShouldBe("true");
            (packageReference.Attribute("ExcludeAssets")?.Value).ShouldBe("all");

            var assemblyReference = project.Descendants("Reference")
                                           .SingleOrDefault(element =>
                                               string.Equals(
                                                   (string?)element.Attribute("Include"),
                                                   runtimeDependency.Assembly,
                                                   StringComparison.Ordinal));
            assemblyReference.ShouldNotBeNull(
                $"{projectPath} must include {runtimeDependency.Assembly} in Debug output and deps metadata.");
            (assemblyReference.Attribute("Condition")?.Value).ShouldBe("'$(Configuration)' == 'Debug'");
            (assemblyReference.Element("HintPath")?.Value).ShouldBe(runtimeDependency.HintPath);
            (assemblyReference.Element("Private")?.Value).ShouldBe("true");
        }
    }

    [Fact]
    public void Desktop_Release_Build_Does_Not_Implicitly_Enable_Publish_Analyzers()
    {
        var project = XDocument.Load(GetRepoFile(
            "controlgallery/AtomUIGallery.Desktop/AtomUIGallery.Desktop.csproj"));
        var releaseGroup = project.Descendants("PropertyGroup")
                                  .Single(element =>
                                      (string?)element.Attribute("Condition") ==
                                      "'$(Configuration)' == 'Release'");

        releaseGroup.Elements().ShouldNotContain(element =>
            (element.Name.LocalName == "PublishTrimmed" ||
             element.Name.LocalName == "PublishAot") &&
            string.IsNullOrWhiteSpace((string?)element.Attribute("Condition")));
        releaseGroup.Elements("PublishTrimmed")
                    .ShouldContain(element =>
                        ((string?)element.Attribute("Condition") ?? string.Empty)
                        .Contains("GalleryPublishTrimmed", StringComparison.Ordinal));
        releaseGroup.Elements("PublishAot")
                    .ShouldContain(element =>
                        ((string?)element.Attribute("Condition") ?? string.Empty)
                        .Contains("GalleryPublishAot", StringComparison.Ordinal));
    }

    [Fact]
    public void Browser_Normal_Build_Skips_Native_Wasm_Link()
    {
        var project = XDocument.Load(GetRepoFile(
            "controlgallery/AtomUIGallery.Browser/AtomUIGallery.Browser.csproj"));
        var runAot = project.Descendants("RunAOTCompilation").ShouldHaveSingleItem();
        ((string?)runAot.Attribute("Condition")).ShouldBe("'$(WasmBuildingForNestedPublish)' == 'true'");
        runAot.Value.ShouldBe("false");

        var target = project.Descendants("Target")
                            .Single(element =>
                                (string?)element.Attribute("Name") ==
                                "AtomUIUseManagedBrowserBuild");

        ((string?)target.Attribute("BeforeTargets")).ShouldBe("_SetWasmBuildNativeDefaults");
        var condition = (string?)target.Attribute("Condition");
        condition.ShouldNotBeNull();
        condition.ShouldContain("WasmBuildingForNestedPublish");
        condition.ShouldContain("RunAOTCompilation");
        condition.ShouldContain("AtomUIBrowserNativeBuild");
        target.Descendants("WasmBuildNative").ShouldHaveSingleItem().Value.ShouldBe("false");
        target.Descendants("WasmEnableWebcil").ShouldHaveSingleItem().Value.ShouldBe("true");
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
            var path = Path.Combine(directory.FullName, relativePath);
            if (File.Exists(path))
            {
                return path;
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException($"Unable to locate repository file '{relativePath}'.");
    }
}
