using AtomUI.Theme;
using AtomUI.Theme.Compilation;
using AtomUI.Theme.Resources;
using AtomUI.Theme.Scope;
using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Threading;
using Shouldly;
using Xunit;

namespace AtomUI.Core.Tests.Theme;

[Collection(ThemeConfigProviderTestCollection.Name)]
public class ThemeConfigProviderTests
{
    [Fact]
    public void Child_Provider_Inherits_Unchanged_Parent_Tokens()
    {
        using var _ = UseThemeManager();
        var childContent = new Border();
        var parent = Provider(
            Token(nameof(DesignToken.ColorPrimary), "#ff0000"),
            Token(nameof(DesignToken.BorderRadius), "12"));
        var child = Provider(Token(nameof(DesignToken.ColorPrimary), "#00b96b"));

        parent.Content = child;
        child.Content = childContent;
        Attach(parent);
        FlushThemeUpdates();

        child.SharedToken.ColorPrimary.ShouldBe(Color.Parse("#00b96b"));
        child.SharedToken.BorderRadius.ShouldBe(new CornerRadius(12));
        GetSnapshot(childContent).ShouldBeSameAs(GetTokenResourceProvider(child).Snapshot);
    }

    [Fact]
    public void Child_Provider_Inherits_Parent_Map_And_Alias_Overrides()
    {
        using var _ = UseThemeManager();
        var parent = Provider(
            Token(nameof(DesignToken.ColorPrimaryBg), "#010203"),
            Token(nameof(DesignToken.ColorBgTextHover), "#040506"));
        var child = Provider(Token(nameof(DesignToken.ColorPrimary), "#00b96b"));

        parent.Content = child;
        child.Content = new Border();
        Attach(parent);
        FlushThemeUpdates();

        child.SharedToken.ColorPrimary.ShouldBe(Color.Parse("#00b96b"));
        child.SharedToken.ColorPrimaryBg.ShouldBe(Color.Parse("#010203"));
        child.SharedToken.ColorBgTextHover.ShouldBe(Color.Parse("#040506"));
    }

    [Fact]
    public void Child_Provider_Can_Disable_Inheritance()
    {
        using var _ = UseThemeManager();
        var parent = Provider(Token(nameof(DesignToken.BorderRadius), "12"));
        var child = Provider();
        child.Inherit = false;

        parent.Content = child;
        child.Content = new Border();
        Attach(parent);
        FlushThemeUpdates();

        child.SharedToken.BorderRadius.ShouldBe(new CornerRadius(6));
    }

    [Fact]
    public void Child_Provider_Recompiles_When_Parent_Token_Changes()
    {
        using var _ = UseThemeManager();
        var parentColor = Token(nameof(DesignToken.ColorPrimary), "#ff0000");
        var parent = Provider(parentColor);
        var child = Provider();

        parent.Content = child;
        child.Content = new Border();
        Attach(parent);
        FlushThemeUpdates();
        child.SharedToken.ColorPrimary.ShouldBe(Color.Parse("#ff0000"));

        parentColor.Value = "#00b96b";
        FlushThemeUpdates();

        child.SharedToken.ColorPrimary.ShouldBe(Color.Parse("#00b96b"));
    }

    [Fact]
    public void Provider_Recompiles_For_Collection_Add_Remove_And_Item_Value_Changes()
    {
        using var _ = UseThemeManager();
        var provider = Provider();
        var primary = Token(nameof(DesignToken.ColorPrimary), "#ff0000");
        Attach(provider);

        provider.SharedTokenSetters.Add(primary);
        FlushThemeUpdates();
        provider.SharedToken.ColorPrimary.ShouldBe(Color.Parse("#ff0000"));

        primary.Value = "#00b96b";
        FlushThemeUpdates();
        provider.SharedToken.ColorPrimary.ShouldBe(Color.Parse("#00b96b"));

        provider.SharedTokenSetters.Remove(primary);
        FlushThemeUpdates();
        provider.SharedToken.ColorPrimary.ShouldBe(Color.Parse("#1677ff"));
    }

    [Fact]
    public void Provider_Recompiles_For_Algorithm_Collection_Changes()
    {
        using var _ = UseThemeManager();
        var provider = Provider();
        Attach(provider);

        provider.Algorithms.Add(nameof(ThemeAlgorithm.Dark));
        FlushThemeUpdates();

        provider.IsDarkMode.ShouldBeTrue();
        provider.SharedToken.ColorBgBase.ShouldBe(Color.FromRgb(0, 0, 0));

        provider.Algorithms.Clear();
        FlushThemeUpdates();

        provider.IsDarkMode.ShouldBeFalse();
        provider.SharedToken.ColorBgBase.ShouldBe(Color.FromRgb(255, 255, 255));
    }

    [Fact]
    public void Child_Provider_Inherits_Parent_Dark_Compact_Algorithms_When_Local_Algorithms_Are_Empty()
    {
        using var _ = UseThemeManager();
        var parent = Provider();
        var child = Provider();

        parent.Algorithms.Add(nameof(ThemeAlgorithm.Dark));
        parent.Algorithms.Add(nameof(ThemeAlgorithm.Compact));
        parent.Content = child;
        child.Content = new Border();
        Attach(parent);
        FlushThemeUpdates();

        child.IsDarkMode.ShouldBeTrue();
        child.SharedToken.ColorBgBase.ShouldBe(Color.FromRgb(0, 0, 0));
        GetSnapshot(child)!.Algorithms.ShouldBe(
        [
            ThemeAlgorithm.Default,
            ThemeAlgorithm.Dark,
            ThemeAlgorithm.Compact
        ]);
    }

    [Fact]
    public void Provider_Recompiles_For_Control_Token_Setter_Add_Remove_And_Value_Changes()
    {
        using var _ = UseThemeManager();
        var provider = Provider();
        var height = new ControlTokenSetter
        {
            Key = nameof(CompilerButtonToken.Height),
            Value = "44"
        };
        var infoSetter = new ControlTokenInfoSetter(CompilerButtonToken.ID);
        Attach(provider);

        infoSetter.Setters.Add(height);
        provider.ControlTokenInfoSetters.Add(infoSetter);
        FlushThemeUpdates();
        GetButtonToken(provider).Height.ShouldBe(44);

        height.Value = "48";
        FlushThemeUpdates();
        GetButtonToken(provider).Height.ShouldBe(48);

        provider.ControlTokenInfoSetters.Remove(infoSetter);
        FlushThemeUpdates();
        GetButtonToken(provider).Height.ShouldBe(32);
    }

    [Fact]
    public void Child_Provider_Inherits_Parent_Component_Override_When_It_Has_No_Local_Override()
    {
        using var _ = UseThemeManager();
        var parent = Provider();
        var child = Provider();

        parent.ControlTokenInfoSetters.Add(ComponentOverride(44));
        parent.Content = child;
        child.Content = new Border();
        Attach(parent);
        FlushThemeUpdates();

        GetButtonToken(child).Height.ShouldBe(44);
    }

    [Fact]
    public void Child_Provider_Component_Override_Wins_Over_Parent_Component_Override()
    {
        using var _ = UseThemeManager();
        var parent = Provider();
        var child = Provider();

        parent.ControlTokenInfoSetters.Add(ComponentOverride(44));
        child.ControlTokenInfoSetters.Add(ComponentOverride(48));
        parent.Content = child;
        child.Content = new Border();
        Attach(parent);
        FlushThemeUpdates();

        GetButtonToken(child).Height.ShouldBe(48);
    }

    [Fact]
    public void Failed_Compile_Retains_Previous_Published_Token_Provider_And_Content_Scope()
    {
        using var _ = UseThemeManager();
        var content = new Border();
        var provider = Provider(Token(nameof(DesignToken.ColorPrimary), "#ff0000"));
        provider.Content = content;
        Attach(provider);
        FlushThemeUpdates();
        var previousSharedToken = provider.SharedToken;
        var previousResourceProvider = GetTokenResourceProvider(provider);
        var previousSnapshot = GetSnapshot(content);
        ThemeScopeCompileFailedEventArgs? failed = null;
        provider.ThemeScopeCompileFailed += (_, args) => failed = args;

        provider.SharedTokenSetters.Add(Token("MissingSharedToken", "1"));
        FlushThemeUpdates();

        failed.ShouldNotBeNull();
        failed!.Diagnostics.ShouldContain(diagnostic =>
            diagnostic.Message.Contains("MissingSharedToken", StringComparison.Ordinal));
        provider.SharedToken.ShouldBeSameAs(previousSharedToken);
        GetTokenResourceProvider(provider).ShouldBeSameAs(previousResourceProvider);
        GetSnapshot(content).ShouldBeSameAs(previousSnapshot);
    }

    [Fact]
    public void Invalid_Algorithm_Failure_Raises_Event_And_Retains_Previous_Published_State()
    {
        using var _ = UseThemeManager();
        var content = new Border();
        var provider = Provider(Token(nameof(DesignToken.ColorPrimary), "#ff0000"));
        provider.Content = content;
        Attach(provider);
        FlushThemeUpdates();
        var previousSharedToken = provider.SharedToken;
        var previousControlTokens = provider.ControlTokens;
        var previousButtonToken = GetButtonToken(provider);
        var previousResourceProvider = GetTokenResourceProvider(provider);
        var previousSnapshot = previousResourceProvider.Snapshot;
        var previousContentSnapshot = GetSnapshot(content);
        var previousDarkMode = provider.IsDarkMode;
        var resourcesChanged = 0;
        ThemeScopeCompileFailedEventArgs? failed = null;
        ((IResourceHost)provider).ResourcesChanged += (_, _) => resourcesChanged++;
        provider.ThemeScopeCompileFailed += (_, args) => failed = args;

        provider.Algorithms.Add("DefinitelyNotAnAlgorithm");
        Should.NotThrow(FlushThemeUpdates);

        failed.ShouldNotBeNull();
        failed!.Exception.ShouldBeOfType<ThemeLoadException>();
        provider.SharedToken.ShouldBeSameAs(previousSharedToken);
        provider.ControlTokens.ShouldBeSameAs(previousControlTokens);
        GetButtonToken(provider).ShouldBeSameAs(previousButtonToken);
        GetTokenResourceProvider(provider).ShouldBeSameAs(previousResourceProvider);
        previousResourceProvider.Snapshot.ShouldBeSameAs(previousSnapshot);
        provider.IsDarkMode.ShouldBe(previousDarkMode);
        GetSnapshot(content).ShouldBeSameAs(previousContentSnapshot);
        resourcesChanged.ShouldBe(0);
    }

    [Fact]
    public void Provider_Maintains_One_Token_Resource_Provider_After_Repeated_Updates()
    {
        using var _ = UseThemeManager();
        var primary = Token(nameof(DesignToken.ColorPrimary), "#ff0000");
        var provider = Provider(primary);
        Attach(provider);
        var resourceProvider = GetTokenResourceProvider(provider);

        for (var i = 0; i < 20; i++)
        {
            primary.Value = $"#{i + 1:00}{i + 2:00}{i + 3:00}";
            FlushThemeUpdates();
            GetTokenResourceProvider(provider).ShouldBeSameAs(resourceProvider);
        }

        provider.Resources.MergedDictionaries
                .OfType<ThemeTokenResourceProvider>()
                .Count()
                .ShouldBe(1);
    }

    [Fact]
    public void Content_Replacement_Clears_Old_Scope_And_Publishes_New_Scope()
    {
        using var _ = UseThemeManager();
        var first = new Border();
        var second = new Border();
        var provider = Provider(Token(nameof(DesignToken.ColorPrimary), "#ff0000"));

        provider.Content = first;
        Attach(provider);
        FlushThemeUpdates();
        GetSnapshot(first).ShouldNotBeNull();

        provider.Content = second;
        FlushThemeUpdates();

        GetSnapshot(first).ShouldBeNull();
        GetSnapshot(second).ShouldBeSameAs(GetTokenResourceProvider(provider).Snapshot);
    }

    [Fact]
    public void Compatibility_Token_Mutations_Do_Not_Change_Scoped_Snapshots_Or_Inheriting_Child()
    {
        using var _ = UseThemeManager();
        var childContent = new Border();
        var parent = Provider(Token(nameof(DesignToken.ColorPrimary), "#ff0000"));
        var child = Provider();
        parent.Content = child;
        child.Content = childContent;
        Attach(parent);
        FlushThemeUpdates();
        var parentSnapshot = GetSnapshot(child).ShouldNotBeNull();
        var childSnapshot = GetSnapshot(childContent).ShouldNotBeNull();

        parent.SharedToken.ColorPrimary = Color.Parse("#00b96b");
        GetButtonToken(parent).Height = 44;

        parentSnapshot.SharedToken.ColorPrimary.ShouldBe(Color.Parse("#ff0000"));
        GetSnapshot(child).ShouldBeSameAs(parentSnapshot);
        childSnapshot.SharedToken.ColorPrimary.ShouldBe(Color.Parse("#ff0000"));
        GetSnapshot(childContent).ShouldBeSameAs(childSnapshot);
        GetSnapshot(childContent)!.Components.Values
            .Single()
            .ControlToken
            .ShouldBeOfType<CompilerButtonToken>()
            .Height
            .ShouldBe(32);
    }

    [Fact]
    public void Logical_Attach_Compiles_Synchronously_And_Logical_Remove_Cleans_And_Suspends_Updates()
    {
        using var _ = UseThemeManager();
        var root = new LogicalTestRoot();
        var firstContent = new Border();
        var provider = new ThemeConfigProvider
        {
            Content = firstContent
        };
        provider.SharedTokenSetters.Add(Token(nameof(DesignToken.ColorPrimary), "#ff0000"));

        root.Child = provider;
        var publishedSnapshot = GetSnapshot(firstContent);

        publishedSnapshot.ShouldNotBeNull();
        provider.SharedToken.ColorPrimary.ShouldBe(Color.Parse("#ff0000"));
        var publishedSharedToken = provider.SharedToken;
        var publishedResourceProvider = GetTokenResourceProvider(provider);
        var publishedProviderSnapshot = publishedResourceProvider.Snapshot;

        root.Child = null;
        GetSnapshot(firstContent).ShouldBeNull();

        var secondContent = new Border();
        provider.Content = secondContent;
        provider.Inherit = false;
        provider.SharedTokenSetters.Add(Token(nameof(DesignToken.ColorPrimary), "#00b96b"));
        provider.ControlTokenInfoSetters.Add(ComponentOverride(48));
        FlushThemeUpdates();

        GetSnapshot(secondContent).ShouldBeNull();
        provider.SharedToken.ShouldBeSameAs(publishedSharedToken);
        GetTokenResourceProvider(provider).ShouldBeSameAs(publishedResourceProvider);
        publishedResourceProvider.Snapshot.ShouldBeSameAs(publishedProviderSnapshot);

        root.Child = provider;

        provider.SharedToken.ColorPrimary.ShouldBe(Color.Parse("#00b96b"));
        GetButtonToken(provider).Height.ShouldBe(48);
        GetSnapshot(secondContent).ShouldBeSameAs(GetTokenResourceProvider(provider).Snapshot);
    }

    [Fact]
    public void Initial_Compile_Notifies_Once_After_Public_State_Is_Published()
    {
        using var _ = UseThemeManager();
        var content = new Border();
        var provider = new ThemeConfigProvider
        {
            Content = content
        };
        provider.SharedTokenSetters.Add(Token(nameof(DesignToken.ColorPrimary), "#ff0000"));
        var root = new LogicalTestRoot();
        var notifications = 0;
        ThemeSnapshot? notifiedSnapshot = null;
        var wasCommittedAtNotification = true;
        ((IResourceHost)provider).ResourcesChanged += (_, _) =>
        {
            var providerSnapshot = provider.Resources.MergedDictionaries
                                           .OfType<ThemeTokenResourceProvider>()
                                           .SingleOrDefault()
                                           ?.Snapshot;
            if (providerSnapshot is null || ReferenceEquals(providerSnapshot, notifiedSnapshot))
            {
                return;
            }

            notifiedSnapshot = providerSnapshot;
            notifications++;
            wasCommittedAtNotification &= GetSnapshot(content)?.SharedToken.ColorPrimary == Color.Parse("#ff0000") &&
                                         provider.SharedToken.ColorPrimary == Color.Parse("#ff0000");
        };

        root.Child = provider;
        FlushThemeUpdates();

        notifications.ShouldBe(1);
        wasCommittedAtNotification.ShouldBeTrue();
    }

    private static ThemeConfigProvider Provider(params TokenSetter[] sharedTokenSetters)
    {
        var provider = new ThemeConfigProvider
        {
            Content = new Border()
        };
        foreach (var setter in sharedTokenSetters)
        {
            provider.SharedTokenSetters.Add(setter);
        }

        FlushThemeUpdates();
        return provider;
    }

    private static TokenSetter Token(string key, string value)
    {
        return new TokenSetter(null, key, value);
    }

    private static ControlTokenInfoSetter ComponentOverride(double height)
    {
        var setter = new ControlTokenInfoSetter(CompilerButtonToken.ID);
        setter.Setters.Add(new ControlTokenSetter
        {
            Key = nameof(CompilerButtonToken.Height),
            Value = height.ToString(System.Globalization.CultureInfo.InvariantCulture)
        });
        return setter;
    }

    private static ThemeSnapshot? GetSnapshot(StyledElement element)
    {
        return element.GetValue(ThemeScope.SnapshotProperty);
    }

    private static ThemeTokenResourceProvider GetTokenResourceProvider(ThemeConfigProvider provider)
    {
        return provider.Resources.MergedDictionaries
                       .OfType<ThemeTokenResourceProvider>()
                       .ShouldHaveSingleItem();
    }

    private static CompilerButtonToken GetButtonToken(ThemeConfigProvider provider)
    {
        return provider.GetControlToken(CompilerButtonToken.ID)
                       .ShouldBeOfType<CompilerButtonToken>();
    }

    private static IDisposable UseThemeManager()
    {
        var scope = AvaloniaLocator.EnterScope();
        var manager = new ThemeManager();
        manager.RegisterControlTokenType(typeof(CompilerButtonToken));
        AvaloniaLocator.CurrentMutable.BindToSelf(manager);
        return scope;
    }

    private static void FlushThemeUpdates()
    {
        if (Dispatcher.UIThread.CheckAccess())
        {
            Dispatcher.UIThread.RunJobs();
            return;
        }

        Dispatcher.UIThread.Invoke(static () => Dispatcher.UIThread.RunJobs());
    }

    private static LogicalTestRoot Attach(Control control)
    {
        var root = new LogicalTestRoot
        {
            Child = control
        };
        FlushThemeUpdates();
        return root;
    }

    private sealed class LogicalTestRoot : Decorator, ILogicalRoot
    {
    }
}
