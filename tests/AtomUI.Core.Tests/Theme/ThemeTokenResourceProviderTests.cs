using AtomUI.Theme;
using AtomUI.Theme.Compilation;
using AtomUI.Theme.Definitions;
using AtomUI.Theme.Resources;
using AtomUI.Theme.Styling;
using AtomUI.Theme.TokenSystem;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Shouldly;
using Xunit;

namespace AtomUI.Core.Tests.Theme;

[Collection(ThemeConfigProviderTestCollection.Name)]
public class ThemeTokenResourceProviderTests
{
    private static readonly ComponentTokenIdentity s_buttonIdentity = new(null, CompilerButtonToken.ID);

    [Fact]
    public void ComponentShared_Key_Resolves_Private_Value_Without_Changing_Global()
    {
        var snapshot = CompileButtonPrimary("#00b96b");
        var provider = new ThemeTokenResourceProvider(snapshot);

        provider.TryGetResource(SharedTokenKind.ColorPrimary, null, out var global).ShouldBeTrue();
        provider.TryGetResource(
            new ComponentSharedTokenResourceKey(null, CompilerButtonToken.ID, SharedTokenKind.ColorPrimary),
            null,
            out var component).ShouldBeTrue();

        component.ShouldNotBe(global);
    }

    [Fact]
    public void ComponentShared_Key_Falls_Back_To_Global_Value_For_Registered_Unconfigured_Component()
    {
        var snapshot = Compile();
        var provider = new ThemeTokenResourceProvider(snapshot);

        provider.TryGetResource(SharedTokenKind.ColorPrimary, null, out var global).ShouldBeTrue();
        provider.TryGetResource(
            new ComponentSharedTokenResourceKey(null, CompilerButtonToken.ID, SharedTokenKind.ColorPrimary),
            null,
            out var component).ShouldBeTrue();

        component.ShouldBeSameAs(global);
    }

    [Fact]
    public void ComponentShared_Key_Uses_Catalog_As_Part_Of_Component_Identity()
    {
        var first = CompileButtonPrimary("#00b96b");
        var second = CompileButtonPrimary("#ff4d4f");
        var snapshot = new ThemeSnapshot(
            first.Id,
            first.Version,
            first.Algorithms,
            first.IsDark,
            first.SharedToken,
            first.SharedResources,
            new Dictionary<ComponentTokenIdentity, ComponentThemeSnapshot>
            {
                [new ComponentTokenIdentity("First", CompilerButtonToken.ID)] = first.Components[s_buttonIdentity],
                [new ComponentTokenIdentity("Second", CompilerButtonToken.ID)] = second.Components[s_buttonIdentity]
            });
        var provider = new ThemeTokenResourceProvider(snapshot);

        provider.TryGetResource(
            new ComponentSharedTokenResourceKey("First", CompilerButtonToken.ID, SharedTokenKind.ColorPrimary),
            null,
            out var firstValue).ShouldBeTrue();
        provider.TryGetResource(
            new ComponentSharedTokenResourceKey("Second", CompilerButtonToken.ID, SharedTokenKind.ColorPrimary),
            null,
            out var secondValue).ShouldBeTrue();

        firstValue.ShouldNotBe(secondValue);
    }

    [Fact]
    public void ComponentShared_Key_Normalizes_Empty_Catalog_To_Null()
    {
        var emptyCatalog = new ComponentSharedTokenResourceKey(
            string.Empty,
            CompilerButtonToken.ID,
            SharedTokenKind.ColorPrimary);
        var noCatalog = new ComponentSharedTokenResourceKey(
            null,
            CompilerButtonToken.ID,
            SharedTokenKind.ColorPrimary);

        emptyCatalog.ShouldBe(noCatalog);
    }

    [Fact]
    public void ComponentShared_Extension_Produces_Normalized_Dynamic_Resource_Key()
    {
        var extension = new ComponentSharedTokenResourceExtension(
            string.Empty,
            CompilerButtonToken.ID,
            SharedTokenKind.ColorPrimary);

        var dynamicResource = extension.ProvideValue(null!).ShouldBeOfType<DynamicResourceExtension>();

        dynamicResource.ResourceKey.ShouldBe(
            new ComponentSharedTokenResourceKey(
                null,
                CompilerButtonToken.ID,
                SharedTokenKind.ColorPrimary));
    }

    [Fact]
    public void Provider_Returns_False_For_Unknown_Resource_And_Component_Ids()
    {
        var provider = new ThemeTokenResourceProvider(Compile());

        provider.TryGetResource("Missing", null, out var unknownResource).ShouldBeFalse();
        unknownResource.ShouldBeNull();
        provider.TryGetResource(
            new ComponentSharedTokenResourceKey(null, "Missing", SharedTokenKind.ColorPrimary),
            null,
            out var unknownComponent).ShouldBeFalse();
        unknownComponent.ShouldBeNull();
    }

    [Fact]
    public void Provider_Resolves_Component_Own_Token_Keys()
    {
        var provider = new ThemeTokenResourceProvider(Compile());

        provider.TryGetResource(CompilerButtonTokenKind.Height, null, out var value).ShouldBeTrue();

        value.ShouldBe(32d);
    }

    [Fact]
    public void Provider_Attaches_To_The_Owning_Resource_Host()
    {
        var provider = new ThemeTokenResourceProvider(Compile());
        var owner = new Border();

        owner.Resources.MergedDictionaries.Add(provider);

        provider.Owner.ShouldBeSameAs(owner);
    }

    [Fact]
    public void PrepareSnapshot_Is_Silent_Until_One_Publish_Notification()
    {
        var provider = new ThemeTokenResourceProvider(Compile());
        var owner = new Border();
        owner.Resources.MergedDictionaries.Add(provider);
        var notifications = 0;
        ((IResourceHost)owner).ResourcesChanged += (_, _) => notifications++;

        provider.PrepareSnapshot(Compile(globalPrimary: "#00b96b"));

        notifications.ShouldBe(0);
        provider.PublishSnapshotChanged();

        notifications.ShouldBe(1);
    }

    [Fact]
    public void ReplaceSnapshot_Publishes_One_Resource_Notification()
    {
        var provider = new ThemeTokenResourceProvider(Compile());
        var owner = new Border();
        owner.Resources.MergedDictionaries.Add(provider);
        var notifications = 0;
        ((IResourceHost)owner).ResourcesChanged += (_, _) => notifications++;

        provider.ReplaceSnapshot(Compile(globalPrimary: "#00b96b"));

        notifications.ShouldBe(1);
    }

    private static ThemeSnapshot Compile(string? globalPrimary = null)
    {
        var sharedOverrides = globalPrimary is null
            ? new Dictionary<string, string>()
            : Tokens((nameof(DesignToken.ColorPrimary), globalPrimary));
        var definition = new ThemeDefinition(
            "TestTheme",
            "Test Theme",
            false,
            [ThemeAlgorithm.Default],
            new Dictionary<string, string>(),
            new Dictionary<string, ThemeControlTokenDefinition>());
        var result = new ThemeCompiler().Compile(new ThemeCompileRequest(
            "TestTheme",
            definition,
            null,
            [ThemeAlgorithm.Default],
            sharedOverrides,
            new Dictionary<ComponentTokenIdentity, ControlTokenConfigInfo>(),
            [new ControlTokenRegistration(typeof(CompilerButtonToken))],
            new Dictionary<string, string>()));

        result.Success.ShouldBeTrue();
        return result.Snapshot!;
    }

    private static ThemeSnapshot CompileButtonPrimary(string primary)
    {
        var definition = new ThemeDefinition(
            "TestTheme",
            "Test Theme",
            false,
            [ThemeAlgorithm.Default],
            new Dictionary<string, string>(),
            new Dictionary<string, ThemeControlTokenDefinition>
            {
                [CompilerButtonToken.ID] = new ThemeControlTokenDefinition(
                    CompilerButtonToken.ID,
                    false,
                    new Dictionary<string, string>(),
                    Tokens((nameof(DesignToken.ColorPrimary), primary)))
            });
        var result = new ThemeCompiler().Compile(new ThemeCompileRequest(
            "TestTheme",
            definition,
            null,
            [ThemeAlgorithm.Default],
            new Dictionary<string, string>(),
            new Dictionary<ComponentTokenIdentity, ControlTokenConfigInfo>(),
            [new ControlTokenRegistration(typeof(CompilerButtonToken))],
            new Dictionary<string, string>()));

        result.Success.ShouldBeTrue();
        return result.Snapshot!;
    }

    private static Dictionary<string, string> Tokens(params (string Name, string Value)[] values)
    {
        return values.ToDictionary(value => value.Name, value => value.Value, StringComparer.Ordinal);
    }
}
