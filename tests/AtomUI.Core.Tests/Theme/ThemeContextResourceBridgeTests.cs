using AtomUI.Theme;
using AtomUI.Theme.Resources;
using Avalonia.Controls;
using Shouldly;
using Xunit;

namespace AtomUI.Core.Tests.Theme;

[Collection(ThemeConfigProviderTestCollection.Name)]
public class ThemeContextResourceBridgeTests
{
    [Fact]
    public void Bridge_Delegates_To_The_Owner_Context_Without_Copying_Resources()
    {
        var snapshot = ThemeTokenResourceProviderTests.Compile();
        var context = new ThemeContext(new ThemeManager(static () => true), snapshot, 0);
        using var bridge = new ThemeContextResourceBridge(context);

        bridge.TryGetResource(SharedTokenKind.ColorPrimary, null, out var actual).ShouldBeTrue();
        context.ResourceProvider.TryGetResource(
            SharedTokenKind.ColorPrimary,
            null,
            out var expected).ShouldBeTrue();

        actual.ShouldBeSameAs(expected);
    }

    [Fact]
    public void Bridge_Forwards_One_Resource_Notification_Per_Context_Publish()
    {
        var context = new ThemeContext(
            new ThemeManager(static () => true),
            ThemeTokenResourceProviderTests.Compile(),
            0);
        using var bridge = new ThemeContextResourceBridge(context);
        var host = new Border();
        host.Resources.MergedDictionaries.Add(bridge);
        var notifications = 0;
        ((IResourceHost)host).ResourcesChanged += (_, _) => notifications++;

        context.Publish();

        notifications.ShouldBe(1);
    }
}
