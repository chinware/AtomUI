using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Window;

public class WindowResizeArtifactTests
{
    [Fact]
    public void Windows_Window_Template_Paints_Root_Background_And_Uses_Opaque_Transparency()
    {
        var document = XDocument.Load(GetRepoFile("src/AtomUI.Desktop.Controls/Window/Themes/WindowTheme.axaml"));
        XNamespace av = "https://github.com/avaloniaui";

        var windowsTemplateStyle = document.Descendants(av + "Style")
                                           .Single(element =>
                                               (string?)element.Attribute("Selector") ==
                                               "^[OsType=Windows][IsCsdEnabled=False]");
        var templateRoot = windowsTemplateStyle.Descendants(av + "ControlTemplate")
                                               .Single()
                                               .Elements(av + "Panel")
                                               .Single();

        templateRoot.Attribute("Background").ShouldNotBeNull().Value.ShouldBe("{TemplateBinding Background}");

        var windowsStyle = document.Descendants(av + "Style")
                                   .Single(element =>
                                       (string?)element.Attribute("Selector") == "^[OsType=Windows]");

        windowsStyle.Elements(av + "Setter").ShouldContain(setter =>
            (string?)setter.Attribute("Property") == "TransparencyLevelHint" &&
            (string?)setter.Attribute("Value") == "None");
    }

    [Fact]
    public void AtomUI_Defaults_Use_Opaque_Friendly_Windows_Composition()
    {
        var source = File.ReadAllText(GetRepoFile("src/AtomUI.Core/AppBuilderExtensions.cs"));

        source.ShouldContain("WithWin32OpaqueFriendlyCompositionOptions");
        source.ShouldContain("Avalonia.Win32PlatformOptions, Avalonia.Win32");
        source.ShouldContain("Avalonia.Win32RenderingMode, Avalonia.Win32");
        source.ShouldContain("Avalonia.Win32CompositionMode, Avalonia.Win32");
        source.ShouldContain("AngleEgl");
        source.ShouldContain("Software");
        source.ShouldContain("LowLatencyDxgiSwapChain");
        source.ShouldContain("RedirectionSurface");
    }

    [Fact]
    public void AtomUI_Core_Does_Not_Directly_Reference_Avalonia_Win32()
    {
        var projectFile = File.ReadAllText(GetRepoFile("src/AtomUI.Core/AtomUI.Core.csproj"));

        projectFile.ShouldNotContain("Avalonia.Win32");
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

        throw new FileNotFoundException($"Could not find repository file: {relativePath}");
    }
}
