using AtomUI.Theme;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;
using Shouldly;
using Xunit;
using AtomUIWindow = AtomUI.Desktop.Controls.Window;
using AvaloniaWindow = Avalonia.Controls.Window;

namespace AtomUI.Desktop.Controls.Tests.Window;

public class WindowThemeContextLeaseTests
{
    static WindowThemeContextLeaseTests()
    {
        AvaloniaTestApp.EnsureInitialized();
    }

    [Fact]
    public void Show_Attaches_Theme_Context_Lease_Before_The_Window_Opened_Event()
    {
        var window = new AtomUIWindow();
        ThemeContext? contextAtOpening = null;
        ThemeVariant? variantAtOpening = null;
        var bridgeCountAtOpening = 0;
        window.AddHandler(
            AvaloniaWindow.WindowOpenedEvent,
            (_, _) =>
            {
                contextAtOpening = window.GetValue(ThemeScope.ContextProperty);
                variantAtOpening = window.RequestedThemeVariant;
                bridgeCountAtOpening = window.Resources.MergedDictionaries
                                             .OfType<ThemeContextResourceBridge>()
                                             .Count();
            });

        try
        {
            window.Show();

            contextAtOpening.ShouldNotBeNull();
            variantAtOpening.ShouldBe(
                contextAtOpening.Appearance == ThemeAppearance.Dark
                    ? ThemeVariant.Dark
                    : ThemeVariant.Light);
            bridgeCountAtOpening.ShouldBe(1);
        }
        finally
        {
            window.Close();
        }

        window.Resources.MergedDictionaries
              .OfType<ThemeContextResourceBridge>()
              .ShouldBeEmpty();
        window.GetValue(ThemeScope.ContextProperty).ShouldBeNull();
    }

    [Fact]
    public void Owned_Window_Uses_The_Owner_Context_Before_The_Window_Opened_Event()
    {
        var owner = new AtomUIWindow();
        var child = new AtomUIWindow();
        ThemeContext? contextAtOpening = null;
        child.AddHandler(
            AvaloniaWindow.WindowOpenedEvent,
            (_, _) => contextAtOpening = child.GetValue(ThemeScope.ContextProperty));

        try
        {
            owner.Show();
            child.Show(owner);

            contextAtOpening.ShouldBeSameAs(owner.GetValue(ThemeScope.ContextProperty));
            child.Resources.MergedDictionaries
                 .OfType<ThemeContextResourceBridge>()
                 .ShouldHaveSingleItem()
                 .OwnerContext.ShouldBeSameAs(contextAtOpening);
        }
        finally
        {
            child.Close();
            owner.Close();
        }
    }

    [Fact]
    public void Failed_Show_Releases_The_New_Context_Lease()
    {
        var window = new AtomUIWindow();
        window.Close();

        Should.Throw<InvalidOperationException>(() => window.Show());

        window.Resources.MergedDictionaries
              .OfType<ThemeContextResourceBridge>()
              .ShouldBeEmpty();
        window.GetValue(ThemeScope.ContextProperty).ShouldBeNull();
    }
}
