using AtomUI.Theme;
using AtomUI.Theme.Compilation;
using AtomUI.Theme.Definitions;
using AtomUI.Theme.Resources;
using AtomUI.Theme.Styling;
using AtomUI.Theme.TokenSystem;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Shouldly;
using Xunit;

namespace AtomUI.Core.Tests.Theme;

[Collection(ThemeConfigProviderTestCollection.Name)]
public class ThemeCompilerTests
{
    private static readonly ControlTokenIdentity s_buttonIdentity = new(null, CompilerButtonToken.ID);

    [Fact]
    public void Control_Algorithm_False_Does_Not_Derive_Map_Tokens()
    {
        var result = Compile(
            controls: Controls(Control(
                CompilerButtonToken.ID,
                enableAlgorithm: false,
                sharedTokens: Tokens((nameof(DesignToken.ColorPrimary), "#00b96b")))));

        result.Success.ShouldBeTrue();
        var snapshot = result.Snapshot!;
        var button = snapshot.Controls[s_buttonIdentity];

        button.EffectiveSharedToken.ColorPrimary.ShouldBe(Color.Parse("#00b96b"));
        button.EffectiveSharedToken.ColorPrimaryHover.ShouldBe(snapshot.SharedToken.ColorPrimaryHover);
    }

    [Fact]
    public void Control_Algorithm_True_Derives_Private_Map_Tokens()
    {
        var result = Compile(
            controls: Controls(Control(
                CompilerButtonToken.ID,
                enableAlgorithm: true,
                sharedTokens: Tokens((nameof(DesignToken.ColorPrimary), "#00b96b")))));

        result.Success.ShouldBeTrue();
        var snapshot = result.Snapshot!;
        var button = snapshot.Controls[s_buttonIdentity];

        button.EffectiveSharedToken.ColorPrimary.ShouldBe(Color.Parse("#00b96b"));
        button.EffectiveSharedToken.ColorPrimaryHover.ShouldNotBe(snapshot.SharedToken.ColorPrimaryHover);
    }

    [Fact]
    public void Ordered_Dark_Compact_Algorithms_Are_Composed_In_Request_Order()
    {
        ThemeAlgorithm[] algorithms =
        [
            ThemeAlgorithm.Default,
            ThemeAlgorithm.Dark,
            ThemeAlgorithm.Compact
        ];

        var result = Compile(algorithms: algorithms);

        result.Success.ShouldBeTrue();
        result.Snapshot!.Algorithms.ShouldBe(algorithms);
        result.Snapshot.IsDark.ShouldBeTrue();
        result.Snapshot.SharedToken.ControlHeight.ShouldBe(28);
        result.Snapshot.SharedToken.ColorBgBase.ShouldBe(Color.FromRgb(0, 0, 0));
    }

    [Fact]
    public void Map_And_Alias_Overrides_Are_Applied_After_Derivation()
    {
        var result = Compile(definitionTokens: Tokens(
            (nameof(DesignToken.ColorPrimary), "#ff0000"),
            (nameof(DesignToken.ColorPrimaryHover), "#010203"),
            (nameof(DesignToken.ColorTextDisabled), "#040506")));

        result.Success.ShouldBeTrue();
        result.Snapshot!.SharedToken.ColorPrimary.ShouldBe(Color.Parse("#ff0000"));
        result.Snapshot.SharedToken.ColorPrimaryHover.ShouldBe(Color.Parse("#010203"));
        result.Snapshot.SharedToken.ColorTextDisabled.ShouldBe(Color.Parse("#040506"));
    }

    [Fact]
    public void Runtime_Overrides_Take_Precedence_Over_Definition_And_Request_Overrides()
    {
        var result = Compile(
            definitionTokens: Tokens((nameof(DesignToken.ColorPrimary), "#ff0000")),
            sharedOverrides: Tokens((nameof(DesignToken.ColorPrimary), "#00b96b")),
            runtimeOverrides: Tokens((nameof(DesignToken.ColorPrimary), "#123456")));

        result.Success.ShouldBeTrue();
        result.Snapshot!.SharedToken.ColorPrimary.ShouldBe(Color.Parse("#123456"));
    }

    [Fact]
    public void Child_Compile_Does_Not_Mutate_Parent_Tokens_Resources_Or_Palettes()
    {
        var parent = Compile(
            sharedOverrides: Tokens((nameof(DesignToken.ColorPrimary), "#ff0000")))
            .Snapshot!;
        var parentPrimary = parent.SharedToken.ColorPrimary;
        var parentPrimaryResource = parent.SharedResources[SharedTokenKind.ColorPrimary];
        var parentPaletteEntries = parent.SharedToken.ColorPalettes.ToArray();

        var child = Compile(
            parent: parent,
            sharedOverrides: Tokens((nameof(DesignToken.ColorPrimary), "#00b96b")))
            .Snapshot!;

        parent.SharedToken.ColorPrimary.ShouldBe(parentPrimary);
        parent.SharedResources[SharedTokenKind.ColorPrimary].ShouldBe(parentPrimaryResource);
        child.SharedToken.ShouldNotBeSameAs(parent.SharedToken);
        child.SharedToken.ColorPalettes.ShouldNotBeSameAs(parent.SharedToken.ColorPalettes);
        foreach (var entry in parentPaletteEntries)
        {
            parent.SharedToken.ColorPalettes[entry.Key].ShouldBe(entry.Value);
            child.SharedToken.ColorPalettes[entry.Key].ShouldNotBeSameAs(entry.Value);
        }
    }

    [Fact]
    public void Child_Compile_Inherits_Parent_Map_And_Alias_Shared_Config()
    {
        var parent = Compile(sharedOverrides: Tokens(
            (nameof(DesignToken.ColorPrimaryBg), "#010203"),
            (nameof(DesignToken.ColorBgTextHover), "#040506")))
            .Snapshot!;

        var child = Compile(
            parent: parent,
            algorithms: Array.Empty<ThemeAlgorithm>(),
            sharedOverrides: Tokens((nameof(DesignToken.ColorPrimary), "#00b96b")))
            .Snapshot!;

        child.SharedToken.ColorPrimary.ShouldBe(Color.Parse("#00b96b"));
        child.SharedToken.ColorPrimaryBg.ShouldBe(Color.Parse("#010203"));
        child.SharedToken.ColorBgTextHover.ShouldBe(Color.Parse("#040506"));
    }

    [Fact]
    public void Control_Request_Overrides_Merge_Over_Definition_Own_Tokens()
    {
        var controlOverride = new ControlTokenConfigInfo
        {
            TokenId = CompilerButtonToken.ID,
            EnableAlgorithm = false,
            Tokens = Tokens((nameof(CompilerButtonToken.Height), "48"))
        };
        var result = Compile(
            controls: Controls(Control(
                CompilerButtonToken.ID,
                ownTokens: Tokens((nameof(CompilerButtonToken.Height), "40")))),
            controlOverrides: new Dictionary<ControlTokenIdentity, ControlTokenConfigInfo>
            {
                [s_buttonIdentity] = controlOverride
            });

        result.Success.ShouldBeTrue();
        var button = result.Snapshot!.Controls[s_buttonIdentity];
        ((CompilerButtonToken)button.ControlToken).Height.ShouldBe(48);
        button.ControlResources[CompilerButtonTokenKind.Height].ShouldBe(48d);
    }

    [Fact]
    public void Registered_Unconfigured_Control_Uses_A_Fresh_Global_Effective_Token()
    {
        var result = Compile(
            sharedOverrides: Tokens((nameof(DesignToken.ColorPrimary), "#00b96b")));

        result.Success.ShouldBeTrue();
        var snapshot = result.Snapshot!;
        var button = snapshot.Controls[s_buttonIdentity];
        button.EffectiveSharedToken.ShouldNotBeSameAs(snapshot.SharedToken);
        button.EffectiveSharedToken.ColorPrimary.ShouldBe(snapshot.SharedToken.ColorPrimary);
        button.SharedResourceDelta.ShouldBeEmpty();
    }

    [Fact]
    public void Unknown_Control_Config_Fails_Without_A_Partial_Snapshot()
    {
        var result = Compile(
            controls: Controls(Control("UnknownControl")));

        result.Success.ShouldBeFalse();
        result.Snapshot.ShouldBeNull();
        result.Diagnostics.ShouldContain(diagnostic =>
            diagnostic.Severity == ThemeDiagnosticSeverity.Error &&
            diagnostic.Message.Contains("UnknownControl", StringComparison.Ordinal));
    }

    [Fact]
    public void Unknown_Control_Own_Token_Fails_Without_A_Partial_Snapshot()
    {
        var controlOverride = new ControlTokenConfigInfo
        {
            TokenId = CompilerButtonToken.ID,
            Tokens = Tokens(("MissingToken", "12"))
        };

        var result = Compile(controlOverrides:
            new Dictionary<ControlTokenIdentity, ControlTokenConfigInfo>
            {
                [s_buttonIdentity] = controlOverride
            });

        result.Success.ShouldBeFalse();
        result.Snapshot.ShouldBeNull();
        result.Diagnostics.ShouldContain(diagnostic =>
            diagnostic.Severity == ThemeDiagnosticSeverity.Error &&
            diagnostic.Message.Contains("MissingToken", StringComparison.Ordinal));
    }

    [Fact]
    public void Duplicate_Registration_Identity_Fails_Without_A_Partial_Snapshot()
    {
        ControlTokenRegistration[] registrations =
        [
            new(typeof(CompilerButtonToken)),
            new(typeof(DuplicateCompilerButtonToken))
        ];

        var result = Compile(registrations: registrations);

        result.Success.ShouldBeFalse();
        result.Snapshot.ShouldBeNull();
        result.Diagnostics.ShouldContain(diagnostic =>
            diagnostic.Severity == ThemeDiagnosticSeverity.Error &&
            diagnostic.Message.Contains(CompilerButtonToken.ID, StringComparison.Ordinal));
    }

    [Fact]
    public void Registration_Is_Activated_Exactly_Once_Per_Compile()
    {
        ActivationCountingCompilerButtonToken.ResetActivationCount();

        var result = Compile(registrations:
        [
            new ControlTokenRegistration(typeof(ActivationCountingCompilerButtonToken))
        ]);

        result.Success.ShouldBeTrue();
        ActivationCountingCompilerButtonToken.ActivationCount.ShouldBe(1);
    }

    [Fact]
    public void Invalid_Non_Control_Token_Registration_Fails_With_A_Diagnostic_And_No_Snapshot()
    {
        var result = Compile(registrations:
        [
            new ControlTokenRegistration(typeof(InvalidCompilerToken))
        ]);

        result.Success.ShouldBeFalse();
        result.Snapshot.ShouldBeNull();
        result.Diagnostics.ShouldContain(diagnostic =>
            diagnostic.Severity == ThemeDiagnosticSeverity.Error &&
            diagnostic.Message.Contains(nameof(InvalidCompilerToken), StringComparison.Ordinal));
    }

    [Fact]
    public void Registration_Without_A_Public_Parameterless_Constructor_Fails_With_A_Theme001_Diagnostic()
    {
        var result = Compile(registrations:
        [
            new ControlTokenRegistration(typeof(NoDefaultConstructorCompilerButtonToken))
        ]);

        result.Success.ShouldBeFalse();
        result.Snapshot.ShouldBeNull();
        result.Exception.ShouldBeNull();
        result.Diagnostics.ShouldContain(diagnostic =>
            diagnostic.Code == "THEME001" &&
            diagnostic.Message.Contains(nameof(NoDefaultConstructorCompilerButtonToken), StringComparison.Ordinal) &&
            diagnostic.Message.Contains("parameterless constructor", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void Registration_With_A_Throwing_Constructor_Fails_With_A_Theme001_Diagnostic()
    {
        var result = Compile(registrations:
        [
            new ControlTokenRegistration(typeof(ThrowingConstructorCompilerButtonToken))
        ]);

        result.Success.ShouldBeFalse();
        result.Snapshot.ShouldBeNull();
        result.Exception.ShouldBeNull();
        result.Diagnostics.ShouldContain(diagnostic =>
            diagnostic.Code == "THEME001" &&
            diagnostic.Message.Contains(nameof(ThrowingConstructorCompilerButtonToken), StringComparison.Ordinal) &&
            diagnostic.Message.Contains(ThrowingConstructorCompilerButtonToken.ExceptionMessage, StringComparison.Ordinal));
    }

    [Fact]
    public void Snapshot_Is_Independent_From_Mutable_Compile_Inputs()
    {
        var algorithms = new List<ThemeAlgorithm> { ThemeAlgorithm.Default };
        var sharedOverrides = Tokens((nameof(DesignToken.ColorPrimary), "#00b96b"));
        var controlOverride = new ControlTokenConfigInfo
        {
            TokenId = CompilerButtonToken.ID,
            EnableAlgorithm = false,
            Tokens = Tokens((nameof(CompilerButtonToken.Height), "48")),
            SharedTokens = Tokens((nameof(DesignToken.ColorPrimary), "#ff0000"))
        };
        var controlOverrides = new Dictionary<ControlTokenIdentity, ControlTokenConfigInfo>
        {
            [s_buttonIdentity] = controlOverride
        };
        var registrations = new List<ControlTokenRegistration>
        {
            new(typeof(CompilerButtonToken))
        };

        var result = Compile(
            algorithms: algorithms,
            sharedOverrides: sharedOverrides,
            controlOverrides: controlOverrides,
            registrations: registrations);

        result.Success.ShouldBeTrue();
        algorithms.Add(ThemeAlgorithm.Dark);
        sharedOverrides[nameof(DesignToken.ColorPrimary)] = "#123456";
        controlOverride.Tokens[nameof(CompilerButtonToken.Height)] = "64";
        controlOverride.SharedTokens[nameof(DesignToken.ColorPrimary)] = "#654321";
        registrations.Clear();

        algorithms.ShouldBe([ThemeAlgorithm.Default, ThemeAlgorithm.Dark]);
        sharedOverrides[nameof(DesignToken.ColorPrimary)].ShouldBe("#123456");
        controlOverride.Tokens[nameof(CompilerButtonToken.Height)].ShouldBe("64");
        controlOverride.SharedTokens[nameof(DesignToken.ColorPrimary)].ShouldBe("#654321");
        registrations.ShouldBeEmpty();

        var snapshot = result.Snapshot!;
        snapshot.Algorithms.ShouldBe([ThemeAlgorithm.Default]);
        snapshot.SharedToken.ColorPrimary.ShouldBe(Color.Parse("#00b96b"));
        snapshot.SharedResources[SharedTokenKind.ColorPrimary]
                .ShouldBeOfType<ImmutableSolidColorBrush>()
                .Color
                .ShouldBe(Color.Parse("#00b96b"));
        var button = snapshot.Controls[s_buttonIdentity];
        button.EffectiveSharedToken.ColorPrimary.ShouldBe(Color.Parse("#ff0000"));
        ((CompilerButtonToken)button.ControlToken).Height.ShouldBe(48);
        button.ControlResources[CompilerButtonTokenKind.Height].ShouldBe(48d);
    }

    [Fact]
    public void Snapshot_Control_Configs_Are_Read_Only_Copies()
    {
        var controlOverride = new ControlTokenConfigInfo
        {
            TokenId = CompilerButtonToken.ID,
            Tokens = Tokens((nameof(CompilerButtonToken.Height), "48"))
        };
        var snapshot = Compile(controlOverrides:
        new Dictionary<ControlTokenIdentity, ControlTokenConfigInfo>
        {
            [s_buttonIdentity] = controlOverride
        }).Snapshot!;

        controlOverride.Tokens[nameof(CompilerButtonToken.Height)] = "64";
        var config = snapshot.ControlConfigs[s_buttonIdentity];

        config.Tokens[nameof(CompilerButtonToken.Height)].ShouldBe("48");
        Should.Throw<NotSupportedException>(() =>
            config.Tokens[nameof(CompilerButtonToken.Height)] = "64");
        Should.Throw<NotSupportedException>(() => config.EnableAlgorithm = true);
        Should.Throw<NotSupportedException>(() =>
            ((IDictionary<ControlTokenIdentity, ControlTokenConfigInfo>)snapshot.ControlConfigs)
            .Add(new ControlTokenIdentity(null, "New"), config));
    }

    [Fact]
    public void Snapshot_Public_SharedToken_Mutations_Do_Not_Affect_Internal_State_Resources_Or_Child_Compiles()
    {
        var parent = Compile(
            sharedOverrides: Tokens((nameof(DesignToken.ColorPrimary), "#ff0000")))
            .Snapshot!;
        var provider = new ThemeTokenResourceProvider(parent);
        var exposed = parent.SharedToken;

        exposed.ColorPrimary = Color.Parse("#00b96b");

        parent.SharedToken.ColorPrimary.ShouldBe(Color.Parse("#ff0000"));
        provider.TryGetResource(SharedTokenKind.ColorPrimary, null, out var primaryResource).ShouldBeTrue();
        primaryResource.ShouldBeOfType<ImmutableSolidColorBrush>()
                       .Color
                       .ShouldBe(Color.Parse("#ff0000"));

        var child = Compile(
            parent: parent,
            algorithms: Array.Empty<ThemeAlgorithm>())
            .Snapshot!;
        child.SharedToken.ColorPrimary.ShouldBe(Color.Parse("#ff0000"));
    }

    [Fact]
    public void Control_Public_Token_Mutations_Do_Not_Affect_Internal_State_Or_Resources()
    {
        var snapshot = Compile(controls: Controls(Control(
            CompilerButtonToken.ID,
            ownTokens: Tokens((nameof(CompilerButtonToken.Height), "48")),
            sharedTokens: Tokens((nameof(DesignToken.ColorPrimary), "#ff0000")))))
            .Snapshot!;
        var control = snapshot.Controls[s_buttonIdentity];
        var exposedSharedToken = control.EffectiveSharedToken;
        var exposedControlToken = control.ControlToken.ShouldBeOfType<CompilerButtonToken>();

        exposedSharedToken.ColorPrimary = Color.Parse("#00b96b");
        exposedControlToken.Height = 64;

        control.EffectiveSharedToken.ColorPrimary.ShouldBe(Color.Parse("#ff0000"));
        control.TryGetSharedResource(SharedTokenKind.ColorPrimary, snapshot.SharedResources, out var sharedResource)
                 .ShouldBeTrue();
        sharedResource.ShouldBeOfType<ImmutableSolidColorBrush>()
                      .Color
                      .ShouldBe(Color.Parse("#ff0000"));
        control.ControlToken.ShouldBeOfType<CompilerButtonToken>()
                 .Height
                 .ShouldBe(48);
        control.ControlResources[CompilerButtonTokenKind.Height].ShouldBe(48d);
    }

    [Fact]
    public void Invalid_Shared_Override_Returns_An_Exception_Without_A_Partial_Snapshot()
    {
        var result = Compile(
            sharedOverrides: Tokens((nameof(DesignToken.FontSize), "not-a-number")));

        result.Success.ShouldBeFalse();
        result.Snapshot.ShouldBeNull();
        result.Exception.ShouldNotBeNull();
    }

    [Fact]
    public void Snapshot_Resource_Maps_Are_Read_Only_And_Colors_Use_Immutable_Brushes()
    {
        var snapshot = Compile().Snapshot!;
        var control = snapshot.Controls[s_buttonIdentity];

        snapshot.SharedResources[SharedTokenKind.ColorPrimary]
                .ShouldBeOfType<ImmutableSolidColorBrush>();
        Should.Throw<NotSupportedException>(() =>
            ((IDictionary<object, object?>)snapshot.SharedResources).Add("new", 1));
        Should.Throw<NotSupportedException>(() =>
            ((IDictionary<ControlTokenIdentity, ControlThemeSnapshot>)snapshot.Controls)
            .Add(new ControlTokenIdentity(null, "New"), control));
        Should.Throw<NotSupportedException>(() =>
            ((IDictionary<object, object?>)control.ControlResources).Add("new", 1));
    }

    [Fact]
    public void Repeated_Uncached_Compilation_Produces_Distinct_Equivalent_Snapshots()
    {
        var request = CreateRequest(
            sharedOverrides: Tokens((nameof(DesignToken.ColorPrimary), "#00b96b")));
        var compiler = new ThemeCompiler();

        var first = compiler.Compile(request).Snapshot!;
        var second = compiler.Compile(request).Snapshot!;

        first.ShouldNotBeSameAs(second);
        first.Version.ShouldNotBe(second.Version);
        first.Algorithms.ShouldBe(second.Algorithms);
        first.SharedToken.ColorPrimary.ShouldBe(second.SharedToken.ColorPrimary);
        first.SharedToken.ColorPrimaryHover.ShouldBe(second.SharedToken.ColorPrimaryHover);
        first.SharedResources[SharedTokenKind.ColorPrimary]
             .ShouldBe(second.SharedResources[SharedTokenKind.ColorPrimary]);
    }

    private static ThemeCompileResult Compile(
        IReadOnlyDictionary<string, string>? definitionTokens = null,
        IReadOnlyDictionary<string, ThemeControlTokenDefinition>? controls = null,
        ThemeSnapshot? parent = null,
        IReadOnlyList<ThemeAlgorithm>? algorithms = null,
        IReadOnlyDictionary<string, string>? sharedOverrides = null,
        IReadOnlyDictionary<ControlTokenIdentity, ControlTokenConfigInfo>? controlOverrides = null,
        IReadOnlyList<ControlTokenRegistration>? registrations = null,
        IReadOnlyDictionary<string, string>? runtimeOverrides = null)
    {
        return new ThemeCompiler().Compile(CreateRequest(
            definitionTokens,
            controls,
            parent,
            algorithms,
            sharedOverrides,
            controlOverrides,
            registrations,
            runtimeOverrides));
    }

    private static ThemeCompileRequest CreateRequest(
        IReadOnlyDictionary<string, string>? definitionTokens = null,
        IReadOnlyDictionary<string, ThemeControlTokenDefinition>? controls = null,
        ThemeSnapshot? parent = null,
        IReadOnlyList<ThemeAlgorithm>? algorithms = null,
        IReadOnlyDictionary<string, string>? sharedOverrides = null,
        IReadOnlyDictionary<ControlTokenIdentity, ControlTokenConfigInfo>? controlOverrides = null,
        IReadOnlyList<ControlTokenRegistration>? registrations = null,
        IReadOnlyDictionary<string, string>? runtimeOverrides = null)
    {
        var effectiveAlgorithms = algorithms ?? [ThemeAlgorithm.Default];
        var definition = new ThemeDefinition(
            "TestTheme",
            "Test Theme",
            false,
            effectiveAlgorithms,
            definitionTokens ?? new Dictionary<string, string>(),
            controls ?? new Dictionary<string, ThemeControlTokenDefinition>());
        return new ThemeCompileRequest(
            "TestTheme",
            definition,
            parent,
            effectiveAlgorithms,
            sharedOverrides ?? new Dictionary<string, string>(),
            controlOverrides ?? new Dictionary<ControlTokenIdentity, ControlTokenConfigInfo>(),
            registrations ?? [new ControlTokenRegistration(typeof(CompilerButtonToken))],
            runtimeOverrides ?? new Dictionary<string, string>());
    }

    private static IReadOnlyDictionary<string, ThemeControlTokenDefinition> Controls(
        params ThemeControlTokenDefinition[] controls)
    {
        return controls.ToDictionary(control => control.TokenId, StringComparer.Ordinal);
    }

    private static ThemeControlTokenDefinition Control(
        string id,
        bool enableAlgorithm = false,
        IReadOnlyDictionary<string, string>? ownTokens = null,
        IReadOnlyDictionary<string, string>? sharedTokens = null)
    {
        return new ThemeControlTokenDefinition(
            id,
            enableAlgorithm,
            ownTokens ?? new Dictionary<string, string>(),
            sharedTokens ?? new Dictionary<string, string>());
    }

    private static Dictionary<string, string> Tokens(params (string Name, string Value)[] values)
    {
        return values.ToDictionary(value => value.Name, value => value.Value, StringComparer.Ordinal);
    }
}

internal enum CompilerButtonTokenKind
{
    Height
}

internal sealed class CompilerButtonToken : AbstractControlDesignToken
{
    internal const string ID = "Button";

    public double Height { get; set; }

    public CompilerButtonToken()
        : base(ID)
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

internal sealed class DuplicateCompilerButtonToken : AbstractControlDesignToken
{
    public DuplicateCompilerButtonToken()
        : base(CompilerButtonToken.ID)
    {
    }

    protected override Type GetTokenKindType()
    {
        return typeof(CompilerButtonTokenKind);
    }
}

internal sealed class ActivationCountingCompilerButtonToken : AbstractControlDesignToken
{
    internal const string ID = "ActivationCountingButton";

    internal static int ActivationCount { get; private set; }

    public double Height { get; set; }

    public ActivationCountingCompilerButtonToken()
        : base(ID)
    {
        ActivationCount++;
    }

    internal static void ResetActivationCount()
    {
        ActivationCount = 0;
    }

    protected override Type GetTokenKindType()
    {
        return typeof(CompilerButtonTokenKind);
    }
}

internal sealed class InvalidCompilerToken
{
}

internal sealed class NoDefaultConstructorCompilerButtonToken : AbstractControlDesignToken
{
    public NoDefaultConstructorCompilerButtonToken(string ignored)
        : base("NoDefaultConstructor")
    {
    }

    protected override Type GetTokenKindType()
    {
        return typeof(CompilerButtonTokenKind);
    }
}

internal sealed class ThrowingConstructorCompilerButtonToken : AbstractControlDesignToken
{
    internal const string ExceptionMessage = "Token activation failed.";

    public ThrowingConstructorCompilerButtonToken()
        : base("ThrowingConstructor")
    {
        throw new InvalidOperationException(ExceptionMessage);
    }

    protected override Type GetTokenKindType()
    {
        return typeof(CompilerButtonTokenKind);
    }
}
