using AtomUI.Theme;
using AtomUI.Theme.Resources;
using Avalonia.Controls;
using Avalonia.Styling;
using Shouldly;
using Xunit;

namespace AtomUI.Core.Tests.Theme;

[Collection(ThemeConfigProviderTestCollection.Name)]
public class ThemeContextLeaseTests
{
    [Fact]
    public void Attach_Installs_Context_Bridge_And_Explicit_Variant()
    {
        HeadlessTestApp.Run(() =>
        {
            var context = CreateContext();
            var host = new Window();

            using var lease = ThemeContextLease.Attach(host, context);

            host.GetValue(ThemeScope.ContextProperty).ShouldBeSameAs(context);
            host.RequestedThemeVariant.ShouldBe(ThemeVariant.Light);
            host.Resources.MergedDictionaries
                .OfType<ThemeContextResourceBridge>()
                .ShouldHaveSingleItem();
            host.Close();
        });
    }

    [Fact]
    public void Owner_Replacement_Releases_The_Old_Bridge_And_Keeps_The_New_Local_State()
    {
        HeadlessTestApp.Run(() =>
        {
            var first = CreateContext();
            var second = CreateContext(globalPrimary: "#00b96b", registrationId: 1);
            var host = new Window();
            var firstLease = ThemeContextLease.Attach(host, first);

            using var secondLease = ThemeContextLease.Attach(host, second);

            host.GetValue(ThemeScope.ContextProperty).ShouldBeSameAs(second);
            host.Resources.MergedDictionaries
                .OfType<ThemeContextResourceBridge>()
                .ShouldHaveSingleItem()
                .OwnerContext.ShouldBeSameAs(second);
            firstLease.Dispose();
            host.GetValue(ThemeScope.ContextProperty).ShouldBeSameAs(second);
            host.Close();
        });
    }

    [Fact]
    public void Dispose_Removes_Only_State_Owned_By_The_Lease()
    {
        HeadlessTestApp.Run(() =>
        {
            var context = CreateContext();
            var host = new Window();
            var lease = ThemeContextLease.Attach(host, context);

            lease.Dispose();

            host.IsSet(ThemeScope.ContextProperty).ShouldBeFalse();
            host.Resources.MergedDictionaries
                .OfType<ThemeContextResourceBridge>()
                .ShouldBeEmpty();
            host.Close();
        });
    }

    private static ThemeContext CreateContext(
        string? globalPrimary = null,
        long registrationId = 0)
    {
        return new ThemeContext(
            new ThemeManager(static () => true),
            ThemeTokenResourceProviderTests.Compile(globalPrimary),
            registrationId);
    }
}
