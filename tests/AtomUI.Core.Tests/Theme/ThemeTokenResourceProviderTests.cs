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
    private static readonly ControlTokenIdentity s_buttonIdentity = new(null, CompilerButtonToken.ID);

    [Fact]
    public void ControlShared_Key_Resolves_Private_Value_Without_Changing_Global()
    {
        var snapshot = CompileButtonPrimary("#00b96b");
        var provider = new ThemeTokenResourceProvider(snapshot);

        provider.TryGetResource(SharedTokenKind.ColorPrimary, null, out var global).ShouldBeTrue();
        provider.TryGetResource(
            new ControlSharedTokenResourceKey(null, CompilerButtonToken.ID, SharedTokenKind.ColorPrimary),
            null,
            out var control).ShouldBeTrue();

        control.ShouldNotBe(global);
    }

    [Fact]
    public void ControlShared_Key_Falls_Back_To_Global_Value_For_Registered_Unconfigured_Control()
    {
        var snapshot = Compile();
        var provider = new ThemeTokenResourceProvider(snapshot);

        provider.TryGetResource(SharedTokenKind.ColorPrimary, null, out var global).ShouldBeTrue();
        provider.TryGetResource(
            new ControlSharedTokenResourceKey(null, CompilerButtonToken.ID, SharedTokenKind.ColorPrimary),
            null,
            out var control).ShouldBeTrue();

        control.ShouldBeSameAs(global);
    }

    [Fact]
    public void ControlShared_Key_Uses_Catalog_As_Part_Of_Control_Identity()
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
            new Dictionary<ControlTokenIdentity, ControlThemeSnapshot>
            {
                [new ControlTokenIdentity("First", CompilerButtonToken.ID)] = first.Controls[s_buttonIdentity],
                [new ControlTokenIdentity("Second", CompilerButtonToken.ID)] = second.Controls[s_buttonIdentity]
            });
        var provider = new ThemeTokenResourceProvider(snapshot);

        provider.TryGetResource(
            new ControlSharedTokenResourceKey("First", CompilerButtonToken.ID, SharedTokenKind.ColorPrimary),
            null,
            out var firstValue).ShouldBeTrue();
        provider.TryGetResource(
            new ControlSharedTokenResourceKey("Second", CompilerButtonToken.ID, SharedTokenKind.ColorPrimary),
            null,
            out var secondValue).ShouldBeTrue();

        firstValue.ShouldNotBe(secondValue);
    }

    [Fact]
    public void ControlShared_Keys_Isolate_Parent_Child_Control_Pairs_While_Global_Fallbacks_Remain_Shared()
    {
        var snapshot = CompileCrossControlSnapshot();
        var provider = new ThemeTokenResourceProvider(snapshot);
        provider.TryGetResource(SharedTokenKind.ColorInfo, null, out var globalInfo).ShouldBeTrue();

        foreach (var (parentControl, childControl) in CrossControlPairs)
        {
            provider.TryGetResource(
                new ControlSharedTokenResourceKey(null, parentControl, SharedTokenKind.ColorPrimary),
                null,
                out var parentPrimary).ShouldBeTrue();
            provider.TryGetResource(
                new ControlSharedTokenResourceKey(null, childControl, SharedTokenKind.ColorPrimary),
                null,
                out var childPrimary).ShouldBeTrue();

            parentPrimary.ShouldNotBe(childPrimary);

            provider.TryGetResource(
                new ControlSharedTokenResourceKey(null, parentControl, SharedTokenKind.ColorInfo),
                null,
                out var parentInfo).ShouldBeTrue();
            provider.TryGetResource(
                new ControlSharedTokenResourceKey(null, childControl, SharedTokenKind.ColorInfo),
                null,
                out var childInfo).ShouldBeTrue();

            parentInfo.ShouldBeSameAs(globalInfo);
            childInfo.ShouldBeSameAs(globalInfo);
        }
    }

    [Fact]
    public void ControlShared_Key_Normalizes_Empty_Catalog_To_Null()
    {
        var emptyCatalog = new ControlSharedTokenResourceKey(
            string.Empty,
            CompilerButtonToken.ID,
            SharedTokenKind.ColorPrimary);
        var noCatalog = new ControlSharedTokenResourceKey(
            null,
            CompilerButtonToken.ID,
            SharedTokenKind.ColorPrimary);

        emptyCatalog.ShouldBe(noCatalog);
    }

    [Fact]
    public void ControlShared_Extension_Produces_Normalized_Dynamic_Resource_Key()
    {
        var extension = new ControlSharedTokenResourceExtension(
            string.Empty,
            CompilerButtonToken.ID,
            SharedTokenKind.ColorPrimary);

        var dynamicResource = extension.ProvideValue(null!).ShouldBeOfType<DynamicResourceExtension>();

        dynamicResource.ResourceKey.ShouldBe(
            new ControlSharedTokenResourceKey(
                ControlDesignTokenAttribute.DefaultCatalog,
                CompilerButtonToken.ID,
                SharedTokenKind.ColorPrimary));
    }

    [Fact]
    public void Provider_Returns_False_For_Unknown_Resource_And_Control_Ids()
    {
        var provider = new ThemeTokenResourceProvider(Compile());

        provider.TryGetResource("Missing", null, out var unknownResource).ShouldBeFalse();
        unknownResource.ShouldBeNull();
        provider.TryGetResource(
            new ControlSharedTokenResourceKey(null, "Missing", SharedTokenKind.ColorPrimary),
            null,
            out var unknownControl).ShouldBeFalse();
        unknownControl.ShouldBeNull();
    }

    [Fact]
    public void Provider_Resolves_Control_Own_Token_Keys()
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
            new Dictionary<ControlTokenIdentity, ControlTokenConfigInfo>(),
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
            new Dictionary<ControlTokenIdentity, ControlTokenConfigInfo>(),
            [new ControlTokenRegistration(typeof(CompilerButtonToken))],
            new Dictionary<string, string>()));

        result.Success.ShouldBeTrue();
        return result.Snapshot!;
    }

    private static ThemeSnapshot CompileCrossControlSnapshot()
    {
        var controlSharedOverrides = new Dictionary<string, string>
        {
            [CompilerButtonToken.ID] = "#ff4d4f",
            [CompilerIconToken.ID] = "#1677ff",
            [CompilerLineEditToken.ID] = "#00b96b",
            [CompilerAddOnDecoratedBoxToken.ID] = "#faad14",
            [CompilerSelectToken.ID] = "#722ed1",
            [CompilerDatePickerToken.ID] = "#13c2c2",
            [CompilerDialogToken.ID] = "#eb2f96",
            [CompilerDataGridToken.ID] = "#52c41a"
        };
        var controlTokens = controlSharedOverrides.ToDictionary(
            entry => entry.Key,
            entry => new ThemeControlTokenDefinition(
                entry.Key,
                false,
                new Dictionary<string, string>(),
                Tokens((nameof(DesignToken.ColorPrimary), entry.Value))),
            StringComparer.Ordinal);
        var definition = new ThemeDefinition(
            "TestTheme",
            "Test Theme",
            false,
            [ThemeAlgorithm.Default],
            new Dictionary<string, string>(),
            controlTokens);
        var result = new ThemeCompiler().Compile(new ThemeCompileRequest(
            "TestTheme",
            definition,
            null,
            [ThemeAlgorithm.Default],
            Tokens((nameof(DesignToken.ColorInfo), "#722ed1")),
            new Dictionary<ControlTokenIdentity, ControlTokenConfigInfo>(),
            [
                new ControlTokenRegistration(typeof(CompilerButtonToken)),
                new ControlTokenRegistration(typeof(CompilerIconToken)),
                new ControlTokenRegistration(typeof(CompilerLineEditToken)),
                new ControlTokenRegistration(typeof(CompilerAddOnDecoratedBoxToken)),
                new ControlTokenRegistration(typeof(CompilerSelectToken)),
                new ControlTokenRegistration(typeof(CompilerDatePickerToken)),
                new ControlTokenRegistration(typeof(CompilerDialogToken)),
                new ControlTokenRegistration(typeof(CompilerDataGridToken))
            ],
            new Dictionary<string, string>()));

        result.Success.ShouldBeTrue();
        return result.Snapshot!;
    }

    private static readonly (string ParentControl, string ChildControl)[] CrossControlPairs =
    [
        (CompilerButtonToken.ID, CompilerIconToken.ID),
        (CompilerLineEditToken.ID, CompilerAddOnDecoratedBoxToken.ID),
        (CompilerSelectToken.ID, CompilerButtonToken.ID),
        (CompilerDatePickerToken.ID, CompilerButtonToken.ID),
        (CompilerDialogToken.ID, CompilerButtonToken.ID),
        (CompilerDataGridToken.ID, CompilerButtonToken.ID)
    ];

    private static Dictionary<string, string> Tokens(params (string Name, string Value)[] values)
    {
        return values.ToDictionary(value => value.Name, value => value.Value, StringComparer.Ordinal);
    }
}

internal abstract class CrossControlCompilerToken : AbstractControlDesignToken
{
    public double Height { get; set; }

    protected CrossControlCompilerToken(string id)
        : base(id)
    {
    }

    public override void CalculateTokenValues(bool isDarkMode)
    {
        Height = SharedToken.ControlHeight;
    }

    protected override Type GetTokenKindType()
    {
        return typeof(CompilerButtonTokenKind);
    }
}

internal sealed class CompilerIconToken : CrossControlCompilerToken
{
    internal const string ID = "Icon";

    public CompilerIconToken()
        : base(ID)
    {
    }
}

internal sealed class CompilerLineEditToken : CrossControlCompilerToken
{
    internal const string ID = "LineEdit";

    public CompilerLineEditToken()
        : base(ID)
    {
    }
}

internal sealed class CompilerAddOnDecoratedBoxToken : CrossControlCompilerToken
{
    internal const string ID = "AddOnDecoratedBox";

    public CompilerAddOnDecoratedBoxToken()
        : base(ID)
    {
    }
}

internal sealed class CompilerSelectToken : CrossControlCompilerToken
{
    internal const string ID = "Select";

    public CompilerSelectToken()
        : base(ID)
    {
    }
}

internal sealed class CompilerDatePickerToken : CrossControlCompilerToken
{
    internal const string ID = "DatePicker";

    public CompilerDatePickerToken()
        : base(ID)
    {
    }
}

internal sealed class CompilerDialogToken : CrossControlCompilerToken
{
    internal const string ID = "Dialog";

    public CompilerDialogToken()
        : base(ID)
    {
    }
}

internal sealed class CompilerDataGridToken : CrossControlCompilerToken
{
    internal const string ID = "DataGrid";

    public CompilerDataGridToken()
        : base(ID)
    {
    }
}
