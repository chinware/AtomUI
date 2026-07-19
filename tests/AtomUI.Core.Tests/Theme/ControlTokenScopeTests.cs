using AtomUI.Theme;
using AtomUI.Theme.Compilation;
using AtomUI.Theme.Resources;
using AtomUI.Theme.Schema;
using AtomUI.Theme.DesignTokens;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Markup.Xaml.XamlIl.Runtime;
using Shouldly;
using Xunit;
using ControlTokenIdentity = AtomUI.Theme.Schema.ControlTokenIdentity;

namespace AtomUI.Core.Tests.Theme;

[Collection(ThemeConfigProviderTestCollection.Name)]
public class ControlTokenScopeTests
{
    [Fact]
    public void Identity_Is_Stored_Once_On_The_Asset_Boundary()
    {
        var dictionary = new ResourceDictionary();
        var identity = new ControlTokenIdentity("AtomUI", CompilerButtonToken.ID);

        ControlTokenScope.SetIdentity(dictionary, identity);

        ControlTokenScope.GetIdentity(dictionary).ShouldBe(identity);
    }

    [Fact]
    public void Shared_Extension_Uses_The_Ambient_Control_Slot_And_Reuses_The_Boxed_Key()
    {
        var manager = new ThemeManager(static () => true);
        var snapshot = ThemeTokenResourceProviderTests.Compile();
        var context = new ThemeContext(manager, snapshot, 0);
        var owner = new Border();
        owner.SetValue(ThemeScope.ContextProperty, context);
        var asset = new ResourceDictionary();
        var identity = new ControlTokenIdentity("AtomUI", CompilerButtonToken.ID);
        ControlTokenScope.SetIdentity(asset, identity);
        var services = new TestServiceProvider([asset, owner]);

        var first = new SharedTokenResourceExtension(SharedTokenKind.ColorPrimary)
                    .ProvideValue(services)
                    .ShouldBeOfType<DynamicResourceExtension>()
                    .ResourceKey;
        var second = new SharedTokenResourceExtension(SharedTokenKind.ColorPrimary)
                     .ProvideValue(services)
                     .ShouldBeOfType<DynamicResourceExtension>()
                     .ResourceKey;

        first.ShouldBeSameAs(second);
        var key = first.ShouldBeOfType<ControlSharedTokenResourceKey>();
        snapshot.Registry.TryGetControl(identity, out var descriptor).ShouldBeTrue();
        key.ControlSlot.ShouldBe(descriptor!.Slot);
        key.Kind.ShouldBe(SharedTokenKind.ColorPrimary);
    }

    [Fact]
    public void Shared_Extension_Without_A_Control_Scope_Remains_A_Global_Key()
    {
        var key = new SharedTokenResourceExtension(SharedTokenKind.ColorPrimary)
                  .ProvideValue(new TestServiceProvider(Array.Empty<object>()))
                  .ShouldBeOfType<DynamicResourceExtension>()
                  .ResourceKey;

        key.ShouldBe(SharedTokenKind.ColorPrimary);
    }

    private sealed class TestServiceProvider : IServiceProvider, IAvaloniaXamlIlParentStackProvider
    {
        internal TestServiceProvider(IEnumerable<object> parents)
        {
            Parents = parents;
        }

        public IEnumerable<object> Parents { get; }

        public object? GetService(Type serviceType)
        {
            return serviceType == typeof(IAvaloniaXamlIlParentStackProvider) ? this : null;
        }
    }
}
