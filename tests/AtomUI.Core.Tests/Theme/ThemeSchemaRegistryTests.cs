using AtomUI.Theme.Schema;
using Shouldly;
using Xunit;

namespace AtomUI.Core.Tests.Theme;

public class ThemeSchemaRegistryTests
{
    [Fact]
    public void Registry_Indexes_Descriptors_By_Ordinal_Identity_And_Dense_Slot()
    {
        var colorPrimary = Token("ColorPrimary", 0, TokenStage.Seed);
        var borderRadius = Token("BorderRadius", 1, TokenStage.Map);
        var contentFontSize = Token("ContentFontSize", 0, TokenStage.Control);
        var buttonIdentity = new ControlTokenIdentity("AtomUI", "Button");
        var button = new ControlTokenDescriptor(
            buttonIdentity,
            [contentFontSize],
            static () => throw new InvalidOperationException(),
            static (_, _) => throw new InvalidOperationException());
        var defaultAlgorithm = Algorithm("Default", ThemeAppearanceEffect.Preserve);

        var registry = new ThemeSchemaRegistry(
            [colorPrimary, borderRadius],
            [button],
            [defaultAlgorithm]);

        registry.GlobalTokens.ShouldBe([colorPrimary, borderRadius]);
        registry.Controls.ShouldHaveSingleItem().Identity.ShouldBe(buttonIdentity);
        registry.Algorithms.ShouldBe([defaultAlgorithm]);
        registry.TryGetGlobalToken("ColorPrimary", out var foundGlobal).ShouldBeTrue();
        foundGlobal.ShouldBeSameAs(colorPrimary);
        registry.TryGetControl(buttonIdentity, out var foundControl).ShouldBeTrue();
        foundControl.ShouldBeSameAs(registry.Controls[0]);
        registry.TryGetAlgorithm("Default", out var foundAlgorithm).ShouldBeTrue();
        foundAlgorithm.ShouldBeSameAs(defaultAlgorithm);
        registry.TryGetGlobalToken("colorprimary", out _).ShouldBeFalse();
        registry.TryGetControl(new ControlTokenIdentity("atomui", "Button"), out _).ShouldBeFalse();
    }

    [Theory]
    [InlineData("global-name")]
    [InlineData("global-slot")]
    [InlineData("control-identity")]
    [InlineData("algorithm-id")]
    public void Registry_Rejects_Duplicate_Descriptors(string duplicate)
    {
        var global0 = Token("ColorPrimary", 0, TokenStage.Seed);
        var globals = duplicate switch
        {
            "global-name" => new[] { global0, Token("ColorPrimary", 1, TokenStage.Map) },
            "global-slot" => new[] { global0, Token("BorderRadius", 0, TokenStage.Map) },
            _             => new[] { global0 }
        };
        var button = Control("AtomUI", "Button");
        var controls = duplicate switch
        {
            "control-identity" => new[] { button, Control("AtomUI", "Button") },
            _                  => new[] { button }
        };
        var defaultAlgorithm = Algorithm("Default", ThemeAppearanceEffect.Preserve);
        var algorithms = duplicate == "algorithm-id"
            ? new[] { defaultAlgorithm, Algorithm("Default", ThemeAppearanceEffect.Dark) }
            : new[] { defaultAlgorithm };

        Should.Throw<ThemeSchemaException>(() => new ThemeSchemaRegistry(globals, controls, algorithms));
    }

    [Fact]
    public void Registry_Requires_Contiguous_Global_And_Control_Own_Slots()
    {
        Should.Throw<ThemeSchemaException>(() => new ThemeSchemaRegistry(
            [Token("ColorPrimary", 1, TokenStage.Seed)],
            Array.Empty<ControlTokenDescriptor>(),
            [Algorithm("Default", ThemeAppearanceEffect.Preserve)]));

        var invalidControl = new ControlTokenDescriptor(
            new ControlTokenIdentity("AtomUI", "Button"),
            [Token("ContentFontSize", 1, TokenStage.Control)],
            static () => throw new InvalidOperationException(),
            static (_, _) => throw new InvalidOperationException());
        Should.Throw<ThemeSchemaException>(() => new ThemeSchemaRegistry(
            [Token("ColorPrimary", 0, TokenStage.Seed)],
            [invalidControl],
            [Algorithm("Default", ThemeAppearanceEffect.Preserve)]));
    }

    [Theory]
    [InlineData("", "Button")]
    [InlineData("AtomUI", "")]
    [InlineData("Atom UI", "Button")]
    [InlineData("AtomUI", "Button:Primary")]
    public void Control_Identity_Requires_Valid_Explicit_Identifiers(string catalog, string id)
    {
        Should.Throw<ArgumentException>(() => new ControlTokenIdentity(catalog, id));
    }

    [Fact]
    public void Registry_Revision_Is_Order_Independent_And_Changes_With_Algorithm_Revision()
    {
        var token = Token("ColorPrimary", 0, TokenStage.Seed);
        var control = Control("AtomUI", "Button");
        var first = new ThemeSchemaRegistry(
            [token],
            [control],
            [Algorithm("Default", ThemeAppearanceEffect.Light, revision: 1)]);
        var reordered = new ThemeSchemaRegistry(
            [token],
            [control],
            [Algorithm("Default", ThemeAppearanceEffect.Light, revision: 1)]);
        var changed = new ThemeSchemaRegistry(
            [token],
            [control],
            [Algorithm("Default", ThemeAppearanceEffect.Light, revision: 2)]);

        reordered.Revision.ShouldBe(first.Revision);
        changed.Revision.ShouldNotBe(first.Revision);
    }

    private static TokenDescriptor Token(string name, int slot, TokenStage stage)
    {
        return new TokenDescriptor(
            name,
            slot,
            stage,
            typeof(string),
            name,
            static value => value,
            static value => ThemeTokenValueFormatter.Format((string)value!),
            static _ => throw new InvalidOperationException(),
            static (_, _) => throw new InvalidOperationException(),
            static _ => throw new InvalidOperationException());
    }

    private static ControlTokenDescriptor Control(string catalog, string id)
    {
        return new ControlTokenDescriptor(
            new ControlTokenIdentity(catalog, id),
            Array.Empty<TokenDescriptor>(),
            static () => throw new InvalidOperationException(),
            static (_, _) => throw new InvalidOperationException());
    }

    private static ThemeAlgorithmDescriptor Algorithm(
        string id,
        ThemeAppearanceEffect effect,
        int revision = 1)
    {
        return new ThemeAlgorithmDescriptor(
            id,
            revision,
            effect,
            static () => throw new InvalidOperationException());
    }
}
