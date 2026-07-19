using AtomUI.Theme;
using AtomUI.Theme.Configuration;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using Shouldly;
using Xunit;
using AtomUIWindow = AtomUI.Desktop.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Theme;

public class ThemePopupInheritanceTests
{
    static ThemePopupInheritanceTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Popup_And_Flyout_Inherit_The_Local_Context_Without_An_Independent_Bridge()
    {
        var target = new Border
        {
            Width = 80,
            Height = 24
        };
        var popupContent = new Border
        {
            Width = 120,
            Height = 40
        };
        var flyoutContent = new Border
        {
            Width = 120,
            Height = 40
        };
        var popup = new Popup
        {
            PlacementTarget = target,
            ShouldUseOverlayLayer = true,
            Child = popupContent
        };
        var canvas = new Canvas();
        canvas.Children.Add(target);
        canvas.Children.Add(popup);
        var provider = new ThemeConfigProvider
        {
            Config = new ThemeConfigBuilder().Build(),
            Child = canvas
        };
        var window = new AtomUIWindow
        {
            Width = 320,
            Height = 240,
            Content = provider
        };
        var flyout = new Flyout
        {
            Content = flyoutContent
        };

        try
        {
            window.Show();
            Dispatcher.UIThread.RunJobs();
            var localContext = target.GetValue(ThemeScope.ContextProperty).ShouldNotBeNull();
            localContext.ShouldNotBeSameAs(window.GetValue(ThemeScope.ContextProperty));

            popup.IsOpen = true;
            Dispatcher.UIThread.RunJobs();
            AssertNaturalInheritance(popupContent, window, localContext);

            flyout.ShowAt(target);
            Dispatcher.UIThread.RunJobs();
            AssertNaturalInheritance(flyoutContent, window, localContext);
        }
        finally
        {
            flyout.Hide();
            popup.IsOpen = false;
            window.Close();
        }
    }

    private static void AssertNaturalInheritance(
        Control content,
        AtomUIWindow owner,
        ThemeContext ownerContext)
    {
        content.GetValue(ThemeScope.ContextProperty).ShouldBeSameAs(ownerContext);
        var contentTopLevel = TopLevel.GetTopLevel(content);
        if (contentTopLevel is not null && !ReferenceEquals(contentTopLevel, owner))
        {
            contentTopLevel.Resources.MergedDictionaries
                           .OfType<ThemeContextResourceBridge>()
                           .ShouldBeEmpty();
        }
    }
}
