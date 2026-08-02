using AtomUI.Generated.AtomUI_Core;
using AtomUI.Theme;
using AtomUI.Theme.Algorithms;
using AtomUI.Theme.Compilation;
using AtomUI.Theme.Configuration;
using AtomUI.Theme.Definitions;
using AtomUI.Theme.Schema;
using AtomUI.Theme.DesignTokens;
using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Shouldly;
using Xunit;

namespace AtomUI.Core.Tests.Theme;

public class DenseThemeCompilerTests
{
    [Fact]
    public void Compile_Applies_Seed_Map_And_Alias_Overrides_In_Order()
    {
        var registry = CreateRegistry();
        var input = CreateInput(
            registry,
            globalTokens:
            [
                Token(registry, nameof(DesignToken.ColorPrimary), "#ff0000"),
                Token(registry, nameof(DesignToken.ColorPrimaryHover), "#010203"),
                Token(registry, nameof(DesignToken.ColorTextDisabled), "#040506")
            ]);

        var result = new ThemeCompiler().Compile(input);

        result.Success.ShouldBeTrue();
        GetGlobal<Color>(result.Snapshot!, registry, nameof(DesignToken.ColorPrimary))
            .ShouldBe(Color.Parse("#ff0000"));
        GetGlobal<Color>(result.Snapshot!, registry, nameof(DesignToken.ColorPrimaryHover))
            .ShouldBe(Color.Parse("#010203"));
        GetGlobal<Color>(result.Snapshot!, registry, nameof(DesignToken.ColorTextDisabled))
            .ShouldBe(Color.Parse("#040506"));
    }

    [Fact]
    public void Compile_Final_Normalization_Disables_Motion_And_Wave_Inputs()
    {
        var registry = CreateRegistry();
        var input = CreateInput(
            registry,
            globalTokens:
            [
                Token(registry, nameof(DesignToken.EnableMotion), "False"),
                Token(registry, nameof(DesignToken.MotionDurationFast), "00:00:01"),
                Token(registry, nameof(DesignToken.MotionDurationMid), "00:00:02"),
                Token(registry, nameof(DesignToken.MotionDurationSlow), "00:00:03"),
                Token(registry, nameof(DesignToken.MotionDurationVerySlow), "00:00:04"),
                Token(registry, nameof(DesignToken.EnableWaveSpirit), "False"),
                Token(registry, nameof(DesignToken.WaveAnimationRange), "24"),
                Token(registry, nameof(DesignToken.WaveStartOpacity), "0.8")
            ]);

        var result = new ThemeCompiler().Compile(input);

        result.Success.ShouldBeTrue();
        var snapshot = result.Snapshot!;
        GetGlobal<bool>(snapshot, registry, nameof(DesignToken.EnableMotion)).ShouldBeFalse();
        GetGlobal<TimeSpan>(snapshot, registry, nameof(DesignToken.MotionDurationFast)).ShouldBe(TimeSpan.Zero);
        GetGlobal<TimeSpan>(snapshot, registry, nameof(DesignToken.MotionDurationMid)).ShouldBe(TimeSpan.Zero);
        GetGlobal<TimeSpan>(snapshot, registry, nameof(DesignToken.MotionDurationSlow)).ShouldBe(TimeSpan.Zero);
        GetGlobal<TimeSpan>(snapshot, registry, nameof(DesignToken.MotionDurationVerySlow)).ShouldBe(TimeSpan.Zero);
        GetGlobal<bool>(snapshot, registry, nameof(DesignToken.EnableWaveSpirit)).ShouldBeFalse();
        GetGlobal<double>(snapshot, registry, nameof(DesignToken.WaveAnimationRange)).ShouldBe(0);
        GetGlobal<double>(snapshot, registry, nameof(DesignToken.WaveStartOpacity)).ShouldBe(0);
    }

    [Fact]
    public void Compile_Passes_The_Same_Effective_Seed_And_The_Previous_Map_Through_The_Algorithm_Chain()
    {
        var calls = new List<AlgorithmCall>();
        var first = Algorithm("First", calls);
        var second = Algorithm("Second", calls);
        var registry = new ThemeSchemaRegistry(
            GeneratedThemeSchema.GetGlobalTokens(),
            Array.Empty<ControlTokenDescriptor>(),
            [first, second]);
        var input = CreateInput(registry, algorithms: [first, second]);

        var result = new ThemeCompiler().Compile(input);

        result.Success.ShouldBeTrue();
        calls.Count.ShouldBe(2);
        calls[0].EffectiveSeed.ShouldBeSameAs(calls[1].EffectiveSeed);
        calls[0].PreviousMap.ShouldBeNull();
        calls[1].PreviousMap.ShouldBeSameAs(calls[0].NextMap);
        calls[1].NextMap.ShouldNotBeSameAs(calls[0].NextMap);
    }

    [Fact]
    public void Compile_Control_Disabled_Applies_Direct_Global_Overrides_Without_Rederivation()
    {
        var control = Control("Button");
        var registry = CreateRegistry([control]);
        registry.TryGetControl(control.Identity, out control).ShouldBeTrue();
        var config = new NormalizedControlThemeConfig(
            control.Identity,
            ControlAlgorithmMode.Disabled,
            Array.Empty<ThemeAlgorithmDescriptor>(),
            [Token(registry, nameof(DesignToken.ColorPrimary), "#00b96b")],
            Array.Empty<NormalizedTokenValue>());

        var result = new ThemeCompiler().Compile(CreateInput(registry, controls: [config]));

        result.Success.ShouldBeTrue();
        var snapshot = result.Snapshot!;
        var controlSnapshot = snapshot.Controls[control.Slot];
        GetControlGlobal<Color>(
                controlSnapshot,
                snapshot,
                registry,
                nameof(DesignToken.ColorPrimary))
            .ShouldBe(Color.Parse("#00b96b"));
        GetControlGlobal<Color>(
                controlSnapshot,
                snapshot,
                registry,
                nameof(DesignToken.ColorPrimaryHover))
            .ShouldBe(GetGlobal<Color>(snapshot, registry, nameof(DesignToken.ColorPrimaryHover)));
    }

    [Fact]
    public void Compile_Control_Without_Own_Tokens_Produces_Effective_Global_Delta_And_Empty_Own_State()
    {
        var control = new ControlTokenDescriptor(
            ThemeTestControlTypes.For("AtomUI", "Rating"),
            new ControlTokenIdentity("AtomUI", "Rating"));
        var registry = CreateRegistry([control]);
        registry.TryGetControl(control.Identity, out control).ShouldBeTrue();
        var config = new NormalizedControlThemeConfig(
            control.Identity,
            ControlAlgorithmMode.Disabled,
            Array.Empty<ThemeAlgorithmDescriptor>(),
            [Token(registry, nameof(DesignToken.ColorPrimary), "#00b96b")],
            Array.Empty<NormalizedTokenValue>());

        var result = new ThemeCompiler().Compile(CreateInput(registry, controls: [config]));

        result.Success.ShouldBeTrue();
        var snapshot = result.Snapshot!;
        var controlSnapshot = snapshot.Controls[control.Slot];
        GetControlGlobal<Color>(
                controlSnapshot,
                snapshot,
                registry,
                nameof(DesignToken.ColorPrimary))
            .ShouldBe(Color.Parse("#00b96b"));
        controlSnapshot.ControlTokenValues.ShouldBeSameAs(TokenValueTable.Empty);
        controlSnapshot.ControlResources.ShouldBeEmpty();
    }

    [Fact]
    public void Compile_Control_Global_Rederives_Map_And_Freezes_Own_Tokens_With_Appearance()
    {
        var control = Control("Button");
        var registry = CreateRegistry([control]);
        registry.TryGetControl(control.Identity, out control).ShouldBeTrue();
        var config = new NormalizedControlThemeConfig(
            control.Identity,
            ControlAlgorithmMode.Global,
            Array.Empty<ThemeAlgorithmDescriptor>(),
            [Token(registry, nameof(DesignToken.ColorPrimary), "#00b96b")],
            [ControlToken(control, "48")]);

        var result = new ThemeCompiler().Compile(CreateInput(registry, controls: [config]));

        result.Success.ShouldBeTrue();
        var snapshot = result.Snapshot!;
        var controlSnapshot = snapshot.Controls[control.Slot];
        GetControlGlobal<Color>(
                controlSnapshot,
                snapshot,
                registry,
                nameof(DesignToken.ColorPrimaryHover))
            .ShouldNotBe(GetGlobal<Color>(snapshot, registry, nameof(DesignToken.ColorPrimaryHover)));
        controlSnapshot.ControlTokenValues.Get<double>(0).ShouldBe(48);
        controlSnapshot.Appearance.ShouldBe(ThemeAppearance.Light);
    }

    [Fact]
    public void Compile_Only_Replaces_The_Changed_Control_When_Global_Content_Is_Unchanged()
    {
        var button = Control("Button");
        var input = Control("Input");
        var registry = CreateRegistry([button, input]);
        registry.TryGetControl(button.Identity, out button).ShouldBeTrue();
        registry.TryGetControl(input.Identity, out input).ShouldBeTrue();
        var parent = new ThemeCompiler().Compile(CreateInput(registry)).Snapshot!;
        var buttonConfig = new NormalizedControlThemeConfig(
            button.Identity,
            ControlAlgorithmMode.Disabled,
            Array.Empty<ThemeAlgorithmDescriptor>(),
            Array.Empty<NormalizedTokenValue>(),
            [ControlToken(button, "48")]);

        var child = new ThemeCompiler().Compile(CreateInput(
            registry,
            controls: [buttonConfig],
            reusableParent: parent)).Snapshot!;

        child.GlobalTokenValues.ShouldBeSameAs(parent.GlobalTokenValues);
        child.GlobalResources.ShouldBeSameAs(parent.GlobalResources);
        child.PresetColorPalettes.ShouldBeSameAs(parent.PresetColorPalettes);
        child.Controls[button.Slot].ShouldNotBeSameAs(parent.Controls[button.Slot]);
        child.Controls[input.Slot].ShouldBeSameAs(parent.Controls[input.Slot]);
    }

    [Fact]
    public void Compile_Reuses_A_WithoutOwnToken_Control_When_Global_Content_Changes()
    {
        var control = new ControlTokenDescriptor(
            ThemeTestControlTypes.For("AtomUI", "Rating"),
            new ControlTokenIdentity("AtomUI", "Rating"));
        var registry = CreateRegistry([control]);
        registry.TryGetControl(control.Identity, out control).ShouldBeTrue();
        var parent = new ThemeCompiler().Compile(CreateInput(registry)).Snapshot!;
        var changed = new ThemeCompiler().Compile(CreateInput(
            registry,
            globalTokens: [Token(registry, nameof(DesignToken.ColorPrimary), "#00b96b")],
            reusableParent: parent)).Snapshot!;

        changed.Controls[control.Slot].ShouldBeSameAs(parent.Controls[control.Slot]);
    }

    [Fact]
    public void Compile_Recomputes_An_OwnToken_Control_When_Global_Content_Changes()
    {
        var control = Control("Button");
        var registry = CreateRegistry([control]);
        registry.TryGetControl(control.Identity, out control).ShouldBeTrue();
        var parent = new ThemeCompiler().Compile(CreateInput(registry)).Snapshot!;

        var changed = new ThemeCompiler().Compile(CreateInput(
            registry,
            globalTokens: [Token(registry, nameof(DesignToken.ControlHeight), "48")],
            reusableParent: parent)).Snapshot!;

        changed.Controls[control.Slot].ShouldNotBeSameAs(parent.Controls[control.Slot]);
        changed.Controls[control.Slot].ControlTokenValues.Get<double>(0).ShouldBe(48);
    }

    [Fact]
    public void Compile_Preserves_Immutable_Control_Token_Values_Without_String_Round_Trip()
    {
        var transform = new ImmutableTransform(Matrix.CreateTranslation(4, 8));
        var control = ImmutableTransformControl("Badge", transform);
        var registry = CreateRegistry([control]);
        registry.TryGetControl(control.Identity, out control).ShouldBeTrue();

        var result = new ThemeCompiler().Compile(CreateInput(registry));

        result.Success.ShouldBeTrue();
        result.Snapshot!.Controls[control.Slot]
              .ControlTokenValues.Get<ImmutableTransform>(0)
              .ShouldBeSameAs(transform);
    }

    [Fact]
    public void Compile_Clones_Mutable_Spline_Easing_Control_Token_Values()
    {
        var easing = new SplineEasing(0.1, 0.9, 0.2, 1.0);
        var control = EasingControl("SplitView", easing);
        var registry = CreateRegistry([control]);
        registry.TryGetControl(control.Identity, out control).ShouldBeTrue();

        var result = new ThemeCompiler().Compile(CreateInput(registry));

        result.Success.ShouldBeTrue();
        var frozen = result.Snapshot!.Controls[control.Slot]
                           .ControlTokenValues.Get<Easing>(0)
                           .ShouldBeOfType<SplineEasing>();
        frozen.ShouldNotBeSameAs(easing);
        frozen.X1.ShouldBe(easing.X1);
        frozen.Y1.ShouldBe(easing.Y1);
        frozen.X2.ShouldBe(easing.X2);
        frozen.Y2.ShouldBe(easing.Y2);
    }

    [Fact]
    public void Compile_Preserves_Concrete_SolidColorBrush_Resource_Type_With_An_Owned_Copy()
    {
        var brush = new SolidColorBrush(Colors.CornflowerBlue, 0.75);
        var control = SolidColorBrushControl("Window", brush);
        var registry = CreateRegistry([control]);
        registry.TryGetControl(control.Identity, out control).ShouldBeTrue();

        var result = new ThemeCompiler().Compile(CreateInput(registry));

        result.Success.ShouldBeTrue();
        var snapshot = result.Snapshot!.Controls[control.Slot];
        var frozen = snapshot.ControlTokenValues.Get<SolidColorBrush>(0);
        frozen.ShouldNotBeSameAs(brush);
        frozen.Color.ShouldBe(brush.Color);
        frozen.Opacity.ShouldBe(brush.Opacity);
        snapshot.ControlResources[control.OwnTokens[0].ResourceKey]
                .ShouldBeOfType<SolidColorBrush>()
                .ShouldNotBeSameAs(brush);
    }

    private static ThemeSchemaRegistry CreateRegistry()
    {
        return CreateRegistry(Array.Empty<ControlTokenDescriptor>());
    }

    private static ThemeSchemaRegistry CreateRegistry(IReadOnlyList<ControlTokenDescriptor> controls)
    {
        return new ThemeSchemaRegistry(
            GeneratedThemeSchema.GetGlobalTokens(),
            controls,
            GeneratedThemeSchema.GetAlgorithms());
    }

    private static ThemeCompileInput CreateInput(
        ThemeSchemaRegistry registry,
        IReadOnlyList<ThemeAlgorithmDescriptor>? algorithms = null,
        IReadOnlyList<NormalizedTokenValue>? globalTokens = null,
        IReadOnlyList<NormalizedControlThemeConfig>? controls = null,
        ThemeSnapshot? reusableParent = null)
    {
        var effectiveAlgorithms = algorithms ??
            [registry.Algorithms.Single(static algorithm => algorithm.Id == "Default")];
        var location = new ThemeSourceLocation("test", 1, 1, "/Theme");
        var definition = new BoundThemeDefinition(
            "TestTheme",
            "Test Theme",
            ThemeAppearance.Light,
            ThemeAppearance.Light,
            true,
            effectiveAlgorithms,
            Array.Empty<BoundTokenValue>(),
            Array.Empty<ControlThemeDefinition>(),
            location);
        var config = new NormalizedThemeConfig(
            false,
            true,
            effectiveAlgorithms,
            globalTokens ?? Array.Empty<NormalizedTokenValue>(),
            controls ?? Array.Empty<NormalizedControlThemeConfig>());
        return new ThemeCompileInput(
            definition,
            new ThemeDefinitionRevision("test", "1", "A1"),
            config,
            registry,
            reusableParent);
    }

    private static NormalizedTokenValue Token(
        ThemeSchemaRegistry registry,
        string name,
        string value)
    {
        registry.TryGetGlobalToken(name, out var descriptor).ShouldBeTrue();
        var parsed = descriptor!.Parse(value);
        return new NormalizedTokenValue(descriptor, parsed, descriptor.Format(parsed));
    }

    private static T GetGlobal<T>(ThemeSnapshot snapshot, ThemeSchemaRegistry registry, string name)
    {
        registry.TryGetGlobalToken(name, out var descriptor).ShouldBeTrue();
        return snapshot.GlobalTokenValues.Get<T>(descriptor!.Slot);
    }

    private static T GetControlGlobal<T>(
        ControlThemeSnapshot control,
        ThemeSnapshot snapshot,
        ThemeSchemaRegistry registry,
        string name)
    {
        registry.TryGetGlobalToken(name, out var descriptor).ShouldBeTrue();
        return control.GetEffectiveGlobalValue<T>(snapshot.GlobalTokenValues, descriptor!.Slot);
    }

    private static NormalizedTokenValue ControlToken(ControlTokenDescriptor control, string value)
    {
        var descriptor = control.OwnTokens.ShouldHaveSingleItem();
        var parsed = descriptor.Parse(value);
        return new NormalizedTokenValue(descriptor, parsed, descriptor.Format(parsed));
    }

    private static ControlTokenDescriptor Control(string id)
    {
        var token = new TokenDescriptor(
            "Height",
            0,
            TokenStage.Control,
            typeof(double),
            $"{id}.Height",
            static value => ThemeTokenValueParser.Parse<double>(value),
            static value => ThemeTokenValueFormatter.Format((double)value!),
            static builder => ((DenseControlToken)builder).Height,
            static (builder, value) => ((DenseControlToken)builder).Height = (double)value!,
            static builder => ((DenseControlToken)builder).Height);
        return new ControlTokenDescriptor(
            ThemeTestControlTypes.For("AtomUI", id),
            new ControlTokenIdentity("AtomUI", id),
            [token],
            static () => new DenseControlToken(),
            static (builder, appearance) => ((DenseControlToken)builder).Evaluate(appearance));
    }

    private static ControlTokenDescriptor ImmutableTransformControl(
        string id,
        ImmutableTransform transform)
    {
        var token = new TokenDescriptor(
            "Transform",
            0,
            TokenStage.Control,
            typeof(ImmutableTransform),
            $"{id}.Transform",
            static value => ThemeTokenValueParser.Parse<ImmutableTransform>(value),
            static value => ThemeTokenValueFormatter.Format((ImmutableTransform)value!),
            static builder => ((DenseTransformControlToken)builder).Transform,
            static (builder, value) =>
                ((DenseTransformControlToken)builder).Transform = (ImmutableTransform)value!,
            static builder => ((DenseTransformControlToken)builder).Transform);
        return new ControlTokenDescriptor(
            ThemeTestControlTypes.For("AtomUI", id),
            new ControlTokenIdentity("AtomUI", id),
            [token],
            () => new DenseTransformControlToken(transform),
            static (builder, appearance) => ((DenseTransformControlToken)builder).Evaluate(appearance));
    }

    private static ControlTokenDescriptor EasingControl(string id, Easing easing)
    {
        var token = new TokenDescriptor(
            "Easing",
            0,
            TokenStage.Control,
            typeof(Easing),
            $"{id}.Easing",
            static value => ThemeTokenValueParser.Parse<Easing>(value),
            static value => ThemeTokenValueFormatter.Format((Easing)value!),
            static builder => ((DenseEasingControlToken)builder).Easing,
            static (builder, value) => ((DenseEasingControlToken)builder).Easing = (Easing)value!,
            static builder => ((DenseEasingControlToken)builder).Easing);
        return new ControlTokenDescriptor(
            ThemeTestControlTypes.For("AtomUI", id),
            new ControlTokenIdentity("AtomUI", id),
            [token],
            () => new DenseEasingControlToken(easing),
            static (builder, appearance) => ((DenseEasingControlToken)builder).Evaluate(appearance));
    }

    private static ControlTokenDescriptor SolidColorBrushControl(
        string id,
        SolidColorBrush brush)
    {
        var token = new TokenDescriptor(
            "Brush",
            0,
            TokenStage.Control,
            typeof(SolidColorBrush),
            $"{id}.Brush",
            static value => ThemeTokenValueParser.Parse<SolidColorBrush>(value),
            static value => ThemeTokenValueFormatter.Format((SolidColorBrush)value!),
            static builder => ((DenseSolidColorBrushControlToken)builder).Brush,
            static (builder, value) =>
                ((DenseSolidColorBrushControlToken)builder).Brush = (SolidColorBrush)value!,
            static builder => ThemeResourceValue.Project(
                ((DenseSolidColorBrushControlToken)builder).Brush));
        return new ControlTokenDescriptor(
            ThemeTestControlTypes.For("AtomUI", id),
            new ControlTokenIdentity("AtomUI", id),
            [token],
            () => new DenseSolidColorBrushControlToken(brush),
            static (builder, appearance) =>
                ((DenseSolidColorBrushControlToken)builder).Evaluate(appearance));
    }

    private static ThemeAlgorithmDescriptor Algorithm(string id, List<AlgorithmCall> calls)
    {
        return new ThemeAlgorithmDescriptor(
            id,
            1,
            ThemeAppearanceEffect.Preserve,
            () => new RecordingAlgorithm(calls));
    }

    private sealed class RecordingAlgorithm(List<AlgorithmCall> calls) : IThemeAlgorithm
    {
        public void Evaluate(DesignToken effectiveSeed, DesignToken? previousMap, DesignToken nextMap)
        {
            calls.Add(new AlgorithmCall(effectiveSeed, previousMap, nextMap));
        }
    }

    private sealed record AlgorithmCall(
        DesignToken EffectiveSeed,
        DesignToken? PreviousMap,
        DesignToken NextMap);

    private sealed class DenseControlToken : AbstractControlDesignToken
    {
        public double Height { get; set; }
        public ThemeAppearance Appearance { get; set; }

        internal void Evaluate(ThemeAppearance appearance)
        {
            Appearance = appearance;
            Height     = EffectiveGlobalToken.ControlHeight;
        }
    }

    private sealed class DenseTransformControlToken(
        ImmutableTransform transform) : AbstractControlDesignToken
    {
        public ImmutableTransform? Transform { get; set; }

        internal void Evaluate(ThemeAppearance appearance)
        {
            Transform = transform;
        }
    }

    private sealed class DenseEasingControlToken(
        Easing easing) : AbstractControlDesignToken
    {
        public Easing? Easing { get; set; }

        internal void Evaluate(ThemeAppearance appearance)
        {
            Easing = easing;
        }
    }

    private sealed class DenseSolidColorBrushControlToken(
        SolidColorBrush brush) : AbstractControlDesignToken
    {
        public SolidColorBrush? Brush { get; set; }

        internal void Evaluate(ThemeAppearance appearance)
        {
            Brush = brush;
        }
    }
}
