using System.Runtime.CompilerServices;
using AtomUI.Theme;
using AtomUI.Theme.Compilation;
using AtomUI.Theme.Configuration;
using AtomUI.Theme.Resources;
using AtomUI.Theme.DesignTokens;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Styling;
using Shouldly;
using Xunit;

namespace AtomUI.Core.Tests.Theme;

[Collection(ThemeConfigProviderTestCollection.Name)]
public class ThemeConfigProviderTests
{
    [Fact]
    public void First_Attach_Synchronously_Publishes_Context_Resources_And_Variant()
    {
        var manager = CreateInitializedManager();
        var provider = new ThemeConfigProvider
        {
            Config = ConfigWithToken(nameof(DesignToken.ColorPrimary), "#ff0000"),
            Child = new Border()
        };

        using var root = Attach(manager, provider);

        var context = provider.GetValue(ThemeScope.ContextProperty).ShouldNotBeNull();
        context.Snapshot.Global<Color>(nameof(DesignToken.ColorPrimary)).ShouldBe(Color.Parse("#ff0000"));
        context.ResourceProvider.ShouldBeSameAs(GetTokenResourceProvider(provider));
        provider.RequestedThemeVariant.ShouldBe(ThemeVariant.Light);
    }

    [Fact]
    public void Config_Replacement_Updates_The_Stable_Context_And_Resource_Provider()
    {
        var manager = CreateInitializedManager();
        var provider = new ThemeConfigProvider
        {
            Config = ConfigWithToken(nameof(DesignToken.ColorPrimary), "#ff0000"),
            Child = new Border()
        };
        using var root = Attach(manager, provider);
        var context = provider.GetValue(ThemeScope.ContextProperty).ShouldNotBeNull();
        var resourceProvider = GetTokenResourceProvider(provider);

        provider.Config = ConfigWithToken(nameof(DesignToken.ColorPrimary), "#00b96b");

        provider.GetValue(ThemeScope.ContextProperty).ShouldBeSameAs(context);
        GetTokenResourceProvider(provider).ShouldBeSameAs(resourceProvider);
        context.Snapshot.Global<Color>(nameof(DesignToken.ColorPrimary)).ShouldBe(Color.Parse("#00b96b"));
    }

    [Fact]
    public void Nested_Providers_Inherit_The_Parent_Effective_Config()
    {
        var manager = CreateInitializedManager();
        var child = new ThemeConfigProvider
        {
            Config = ConfigWithToken(nameof(DesignToken.ColorPrimary), "#00b96b"),
            Child = new Border()
        };
        var parent = new ThemeConfigProvider
        {
            Config = ConfigWithToken(nameof(DesignToken.BorderRadius), "12"),
            Child = child
        };

        using var root = Attach(manager, parent);

        var snapshot = child.GetValue(ThemeScope.ContextProperty).ShouldNotBeNull().Snapshot;
        snapshot.Global<Color>(nameof(DesignToken.ColorPrimary)).ShouldBe(Color.Parse("#00b96b"));
        snapshot.Global<CornerRadius>(nameof(DesignToken.BorderRadius)).ShouldBe(new CornerRadius(12));
    }

    [Fact]
    public void Inherit_False_Uses_The_Definition_Baseline_Instead_Of_Parent_Overrides()
    {
        var manager = CreateInitializedManager();
        var child = new ThemeConfigProvider
        {
            Config = new ThemeConfigBuilder().WithInherit(false).Build(),
            Child = new Border()
        };
        var parent = new ThemeConfigProvider
        {
            Config = ConfigWithToken(nameof(DesignToken.BorderRadius), "12"),
            Child = child
        };

        using var root = Attach(manager, parent);

        child.GetValue(ThemeScope.ContextProperty)
             .ShouldNotBeNull()
             .Snapshot.Global<CornerRadius>(nameof(DesignToken.BorderRadius))
             .ShouldBe(new CornerRadius(6));
    }

    [Fact]
    public void Invalid_Config_Keeps_The_Published_Snapshot_And_Last_Valid_Config()
    {
        var manager = CreateInitializedManager();
        var valid = ConfigWithToken(nameof(DesignToken.ColorPrimary), "#ff0000");
        var provider = new ThemeConfigProvider
        {
            Config = valid,
            Child = new Border()
        };
        using var root = Attach(manager, provider);
        var context = provider.GetValue(ThemeScope.ContextProperty).ShouldNotBeNull();
        var snapshot = context.Snapshot;
        ThemeChangeFailedEventArgs? failure = null;
        provider.ThemeChangeFailed += (_, args) => failure = args;

        provider.Config = ConfigWithToken("MissingToken", "1");

        failure.ShouldNotBeNull();
        context.Snapshot.ShouldBeSameAs(snapshot);
        manager.ScopeGraph.TryGetNode(provider, out var node).ShouldBeTrue();
        node!.LastValidConfig.ShouldBeSameAs(valid);
    }

    [Fact]
    public void Detach_And_Reattach_Uses_A_New_Registration_Without_Leaving_The_Old_Edge()
    {
        var manager = CreateInitializedManager();
        var provider = new ThemeConfigProvider
        {
            Config = new ThemeConfigBuilder().Build(),
            Child = new Border()
        };
        var firstRoot = Attach(manager, provider);
        manager.ScopeGraph.TryGetNode(provider, out var firstNode).ShouldBeTrue();
        var firstId = firstNode!.RegistrationId;

        firstRoot.Child = null;
        manager.ScopeGraph.TryGetNode(provider, out _).ShouldBeFalse();
        using var secondRoot = Attach(manager, provider);

        manager.ScopeGraph.TryGetNode(provider, out var secondNode).ShouldBeTrue();
        secondNode!.RegistrationId.ShouldBeGreaterThan(firstId);
        firstRoot.Dispose();
    }

    [Fact]
    public void Config_Replacement_Does_Not_Retain_The_Previous_Immutable_Config()
    {
        var manager = CreateInitializedManager();
        var provider = new ThemeConfigProvider
        {
            Child = new Border()
        };
        using var root = Attach(manager, provider);

        var oldConfig = SetTemporaryConfig(provider);
        provider.Config = new ThemeConfigBuilder().Build();
        Collect();

        oldConfig.IsAlive.ShouldBeFalse();
    }

    [Fact]
    public void Detached_Provider_Context_And_Resource_Provider_Are_Collectible()
    {
        var manager = CreateInitializedManager();
        var references = AttachAndDetachTemporaryProvider(manager);

        Collect();

        references.Provider.IsAlive.ShouldBeFalse();
        references.Context.IsAlive.ShouldBeFalse();
        references.ResourceProvider.IsAlive.ShouldBeFalse();
    }

    private static ThemeManager CreateInitializedManager()
    {
        var prepared = CreatePrepared();
        var manager = new ThemeManager(
            static () => true,
            (_, _, _) => ValueTask.FromResult(prepared));
        manager.ApplyThemeAsync(
                   new ThemeRequest("Test", null, ThemeTransitionReason.Startup))
               .GetAwaiter()
               .GetResult()
               .Status.ShouldBe(ThemeTransitionStatus.Committed);
        return manager;
    }

    private static ThemeTransactionPreparation CreatePrepared()
    {
        var registry = TypedThemeSnapshotCacheTests.CreateRegistry();
        var input = TypedThemeSnapshotCacheTests.CreateInput(registry);
        var snapshot = new ThemeCompiler().Compile(input).Snapshot!;
        return ThemeTransactionPreparation.Succeeded(snapshot, ThemeSnapshotCacheKey.Create(input));
    }

    private static ThemeConfig ConfigWithToken(string name, string value)
    {
        return new ThemeConfigBuilder().WithToken(name, value).Build();
    }

    private static ThemeTokenResourceProvider GetTokenResourceProvider(ThemeConfigProvider provider)
    {
        return provider.Resources.MergedDictionaries
                       .OfType<ThemeTokenResourceProvider>()
                       .ShouldHaveSingleItem();
    }

    private static TestRoot Attach(ThemeManager manager, Control child)
    {
        var root = new TestRoot();
        root.SetValue(ThemeScope.ContextProperty, manager.RootContext);
        root.Child = child;
        return root;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static WeakReference SetTemporaryConfig(ThemeConfigProvider provider)
    {
        var config = ConfigWithToken(nameof(DesignToken.ColorPrimary), "#ff0000");
        provider.Config = config;
        return new WeakReference(config);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static CollectibleReferences AttachAndDetachTemporaryProvider(ThemeManager manager)
    {
        var root = new TestRoot();
        root.SetValue(ThemeScope.ContextProperty, manager.RootContext);
        var provider = new ThemeConfigProvider
        {
            Config = new ThemeConfigBuilder().Build(),
            Child = new Border()
        };
        root.Child = provider;
        var context = provider.GetValue(ThemeScope.ContextProperty).ShouldNotBeNull();
        var resourceProvider = context.ResourceProvider;
        root.Child = null;
        provider.Child = null;
        return new CollectibleReferences(
            new WeakReference(provider),
            new WeakReference(context),
            new WeakReference(resourceProvider));
    }

    private static void Collect()
    {
        for (var attempt = 0; attempt < 3; attempt++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }
    }

    private sealed class TestRoot : Decorator, ILogicalRoot, IDisposable
    {
        public void Dispose()
        {
            Child = null;
            ClearValue(ThemeScope.ContextProperty);
        }
    }

    private sealed record CollectibleReferences(
        WeakReference Provider,
        WeakReference Context,
        WeakReference ResourceProvider);
}
