using AtomUI.Theme;
using AtomUI.Theme.Compilation;
using AtomUI.Theme.Configuration;
using AtomUI.Theme.Resources;
using AtomUI.Theme.DesignTokens;
using Avalonia.Controls;
using Shouldly;
using Xunit;

namespace AtomUI.Core.Tests.Theme;

[Collection(ThemeConfigProviderTestCollection.Name)]
public class ThemeTokenResourceProviderTests
{
    [Fact]
    public void ControlShared_Key_Resolves_Private_Value_Without_Changing_Global()
    {
        var snapshot = CompileButtonPrimary("#00b96b");
        var provider = new ThemeTokenResourceProvider(snapshot);

        provider.TryGetResource(SharedTokenKind.ColorPrimary, null, out var global).ShouldBeTrue();
        provider.TryGetResource(
            BoundKey(snapshot, "AtomUI", CompilerButtonToken.ID, SharedTokenKind.ColorPrimary),
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
            BoundKey(snapshot, "AtomUI", CompilerButtonToken.ID, SharedTokenKind.ColorPrimary),
            null,
            out var control).ShouldBeTrue();

        control.ShouldBeSameAs(global);
    }

    [Fact]
    public void ControlShared_Key_Uses_Catalog_As_Part_Of_Control_Identity()
    {
        var snapshot = CompileCatalogControls();
        var provider = new ThemeTokenResourceProvider(snapshot);

        provider.TryGetResource(
            BoundKey(snapshot, "First", CompilerButtonToken.ID, SharedTokenKind.ColorPrimary),
            null,
            out var firstValue).ShouldBeTrue();
        provider.TryGetResource(
            BoundKey(snapshot, "Second", CompilerButtonToken.ID, SharedTokenKind.ColorPrimary),
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
                BoundKey(snapshot, "AtomUI", parentControl, SharedTokenKind.ColorPrimary),
                null,
                out var parentPrimary).ShouldBeTrue();
            provider.TryGetResource(
                BoundKey(snapshot, "AtomUI", childControl, SharedTokenKind.ColorPrimary),
                null,
                out var childPrimary).ShouldBeTrue();

            parentPrimary.ShouldNotBe(childPrimary);

            provider.TryGetResource(
                BoundKey(snapshot, "AtomUI", parentControl, SharedTokenKind.ColorInfo),
                null,
                out var parentInfo).ShouldBeTrue();
            provider.TryGetResource(
                BoundKey(snapshot, "AtomUI", childControl, SharedTokenKind.ColorInfo),
                null,
                out var childInfo).ShouldBeTrue();

            parentInfo.ShouldBeSameAs(globalInfo);
            childInfo.ShouldBeSameAs(globalInfo);
        }
    }

    [Fact]
    public void ControlShared_Key_Is_Boxed_Once_Per_Registry_Slot()
    {
        var snapshot = Compile();
        var first = BoundKey(snapshot, "AtomUI", CompilerButtonToken.ID, SharedTokenKind.ColorPrimary);
        var second = BoundKey(snapshot, "AtomUI", CompilerButtonToken.ID, SharedTokenKind.ColorPrimary);

        first.ShouldBeSameAs(second);
    }

    [Fact]
    public void Provider_Returns_False_For_Unknown_Resource_And_Control_Ids()
    {
        var provider = new ThemeTokenResourceProvider(Compile());

        provider.TryGetResource("Missing", null, out var unknownResource).ShouldBeFalse();
        unknownResource.ShouldBeNull();
        provider.TryGetResource(
            ControlSharedTokenResourceKey.Unbound(
                new AtomUI.Theme.Schema.ControlTokenIdentity("AtomUI", "Missing"),
                SharedTokenKind.ColorPrimary),
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

    internal static ThemeSnapshot Compile(string? globalPrimary = null)
    {
        var builder = new ThemeConfigBuilder();
        if (globalPrimary is not null)
        {
            builder.WithToken(nameof(DesignToken.ColorPrimary), globalPrimary);
        }
        return ThemeTestSnapshotFactory.Compile(
            builder.Build(),
            ThemeCompilerTests.CreateCompilerButtonDescriptor());
    }

    private static ThemeSnapshot CompileButtonPrimary(string primary)
    {
        var identity = new AtomUI.Theme.Schema.ControlTokenIdentity("AtomUI", CompilerButtonToken.ID);
        var config = new ThemeConfigBuilder()
                     .WithControl(
                         identity,
                         new ControlThemeConfigBuilder()
                             .WithAlgorithm(ControlAlgorithmMode.Disabled)
                             .WithToken(nameof(DesignToken.ColorPrimary), primary)
                             .Build())
                     .Build();
        return ThemeTestSnapshotFactory.Compile(
            config,
            ThemeCompilerTests.CreateCompilerButtonDescriptor());
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
        var descriptors = new[]
        {
            ThemeCompilerTests.CreateCompilerButtonDescriptor(),
            CreateCrossControlDescriptor(CompilerIconToken.ID, static () => new CompilerIconToken()),
            CreateCrossControlDescriptor(CompilerLineEditToken.ID, static () => new CompilerLineEditToken()),
            CreateCrossControlDescriptor(CompilerAddOnDecoratedBoxToken.ID, static () => new CompilerAddOnDecoratedBoxToken()),
            CreateCrossControlDescriptor(CompilerSelectToken.ID, static () => new CompilerSelectToken()),
            CreateCrossControlDescriptor(CompilerDatePickerToken.ID, static () => new CompilerDatePickerToken()),
            CreateCrossControlDescriptor(CompilerDialogToken.ID, static () => new CompilerDialogToken()),
            CreateCrossControlDescriptor(CompilerDataGridToken.ID, static () => new CompilerDataGridToken())
        };
        var builder = new ThemeConfigBuilder()
                      .WithToken(nameof(DesignToken.ColorInfo), "#722ed1");
        foreach (var entry in controlSharedOverrides)
        {
            builder.WithControl(
                new AtomUI.Theme.Schema.ControlTokenIdentity("AtomUI", entry.Key),
                new ControlThemeConfigBuilder()
                    .WithAlgorithm(ControlAlgorithmMode.Disabled)
                    .WithToken(nameof(DesignToken.ColorPrimary), entry.Value)
                    .Build());
        }
        return ThemeTestSnapshotFactory.Compile(builder.Build(), descriptors);
    }

    internal static ThemeSnapshot CompileCatalogControls()
    {
        var first = CreateCrossControlDescriptor(
            CompilerButtonToken.ID,
            static () => new CompilerButtonToken(),
            "First");
        var second = CreateCrossControlDescriptor(
            CompilerButtonToken.ID,
            static () => new CompilerButtonToken(),
            "Second");
        var config = new ThemeConfigBuilder()
                     .WithControl(
                         first.Identity,
                         new ControlThemeConfigBuilder()
                             .WithAlgorithm(ControlAlgorithmMode.Disabled)
                             .WithToken(nameof(DesignToken.ColorPrimary), "#00b96b")
                             .Build())
                     .WithControl(
                         second.Identity,
                         new ControlThemeConfigBuilder()
                             .WithAlgorithm(ControlAlgorithmMode.Disabled)
                             .WithToken(nameof(DesignToken.ColorPrimary), "#ff4d4f")
                             .Build())
                     .Build();
        return ThemeTestSnapshotFactory.Compile(config, first, second);
    }

    private static AtomUI.Theme.Schema.ControlTokenDescriptor CreateCrossControlDescriptor(
        string id,
        Func<AbstractControlDesignToken> factory,
        string catalog = ControlDesignTokenAttribute.DefaultCatalog)
    {
        return new AtomUI.Theme.Schema.ControlTokenDescriptor(
            new AtomUI.Theme.Schema.ControlTokenIdentity(catalog, id),
            Array.Empty<AtomUI.Theme.Schema.TokenDescriptor>(),
            factory,
            static (token, appearance) => token.CalculateTokenValues(appearance == ThemeAppearance.Dark));
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

    private static object BoundKey(
        ThemeSnapshot snapshot,
        string catalog,
        string id,
        SharedTokenKind kind)
    {
        return snapshot.Registry.GetControlSharedResourceKey(
            new AtomUI.Theme.Schema.ControlTokenIdentity(catalog, id),
            kind);
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
