using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Tooltip;

public class ToolTipPopupModeTests
{
    [Fact]
    public void ToolTip_Keeps_Window_And_Overlay_Popup_Mode_Binding()
    {
        var source = File.ReadAllText(GetRepoFile("src/AtomUI.Desktop.Controls/Tooltip/ToolTip.cs"));

        source.ShouldContain("Bind(ShouldUseOverlayPopupProperty, control.GetBindingObservable(IsUseOverlayHostProperty))");
        source.ShouldContain("_popup.Bind(Popup.ShouldUseOverlayLayerProperty, this.GetObservable(ShouldUseOverlayPopupProperty))");
    }

    [Fact]
    public void Native_PopupRoot_Uses_Null_Background_For_Transparent_Window_Composition()
    {
        var document = XDocument.Load(GetRepoFile("src/AtomUI.Desktop.Controls/Popup/Themes/PopupRootTheme.axaml"));
        XNamespace av = "https://github.com/avaloniaui";

        var theme = document.Descendants(av + "ControlTheme")
                            .Single(element => (string?)element.Attribute("TargetType") == "PopupRoot");

        theme.Elements(av + "Setter").ShouldContain(setter =>
            (string?)setter.Attribute("Property") == "Background" &&
            (string?)setter.Attribute("Value") == "{x:Null}");
        theme.Elements(av + "Setter").ShouldContain(setter =>
            (string?)setter.Attribute("Property") == "TransparencyLevelHint" &&
            (string?)setter.Attribute("Value") == "Transparent");
        theme.Descendants(av + "Border").ShouldContain(border =>
            (string?)border.Attribute("Name") == "PART_TransparencyFallback");
    }

    [Fact]
    public void Overlay_PopupHost_Still_Uses_Overlay_Shadow_Layout_Mode()
    {
        var document = XDocument.Load(GetRepoFile("src/AtomUI.Desktop.Controls/Popup/Themes/OverlayPopupHostTheme.axaml"));
        XNamespace av = "https://github.com/avaloniaui";

        var shadowContainer = document.Descendants()
                                      .Single(element => element.Name.LocalName == "ShadowsAwareContainer");

        shadowContainer.Attribute("IsOverlayMode").ShouldNotBeNull().Value.ShouldBe("True");
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
