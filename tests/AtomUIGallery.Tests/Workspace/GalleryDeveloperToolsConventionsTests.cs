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
