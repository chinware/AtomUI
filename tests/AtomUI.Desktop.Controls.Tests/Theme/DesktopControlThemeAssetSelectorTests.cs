using AtomUI.Theme.Schema;
using Shouldly;
using Xunit;

namespace AtomUI.Desktop.Controls.Tests.Theme;

public class DesktopControlThemeAssetSelectorTests
{
    [Theory]
    [InlineData("AdornerLayer", false)]
    [InlineData("OtpLineEdit", false)]
    [InlineData("OtpLineEditCell", false)]
    [InlineData("SplitView", false)]
    [InlineData("TreeViewFlyoutPresenter", false)]
    [InlineData("Window", false)]
    [InlineData("WindowTitleBar", false)]
    [InlineData("TextBox", true)]
    [InlineData("IndicatorScrollViewer", true)]
    public void Browser_Control_Support_Uses_Exact_Registered_Identity(string controlId, bool expected)
    {
        DesktopControlThemeAssetSelector.IsBrowserControlSupported(
            new ControlTokenIdentity("AtomUI", controlId)).ShouldBe(expected);
    }

    [Fact]
    public void Native_Selection_Excludes_Browser_Theme_Assets()
    {
        var assets = new[]
        {
            Asset("Buttons/Themes/ButtonTheme.axaml", "Button"),
            Asset("Buttons/Themes/Browser/ButtonTheme.axaml", "Button"),
            Asset("Alert/Themes/AlertTheme.axaml", "Alert")
        };

        var selected = DesktopControlThemeAssetSelector.SelectNative(assets);

        selected.Select(static asset => asset.AssetUri.AbsolutePath).ShouldBe(
        [
            "/Buttons/Themes/ButtonTheme.axaml",
            "/Alert/Themes/AlertTheme.axaml"
        ]);
    }

    [Fact]
    public void Browser_Selection_Prefers_Browser_Override_And_Keeps_Unrelated_Common_Assets()
    {
        var assets = new[]
        {
            Asset("Buttons/Themes/ButtonTheme.axaml", "Button"),
            Asset("Buttons/Themes/Browser/ButtonTheme.axaml", "Button"),
            Asset("Buttons/Themes/DropdownButtonTheme.axaml", "DropdownButton"),
            Asset("Alert/Themes/AlertTheme.axaml", "Alert")
        };

        var selected = DesktopControlThemeAssetSelector.SelectBrowser(assets);

        selected.Select(static asset => asset.AssetUri.AbsolutePath).ShouldBe(
        [
            "/Buttons/Themes/Browser/ButtonTheme.axaml",
            "/Buttons/Themes/DropdownButtonTheme.axaml",
            "/Alert/Themes/AlertTheme.axaml"
        ]);
    }

    private static ControlThemeAssetDescriptor Asset(string path, string owner)
    {
        var identity = new ControlTokenIdentity("AtomUI", owner);
        return new ControlThemeAssetDescriptor(
            new Uri($"avares://AtomUI.Desktop.Controls/{path}"),
            identity,
            [identity],
            null,
            1);
    }
}
