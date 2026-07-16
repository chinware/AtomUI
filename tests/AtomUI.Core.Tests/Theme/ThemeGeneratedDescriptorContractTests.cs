using System.Globalization;
using AtomUI.Theme;
using AtomUI.Theme.Algorithms;
using AtomUI.Theme.Schema;
using AtomUI.Theme.TokenSystem;
using Avalonia.Media;
using Avalonia.Media.Immutable;
using Shouldly;
using Xunit;

namespace AtomUI.Core.Tests.Theme;

public class ThemeGeneratedDescriptorContractTests
{
    [Fact]
    public void Descriptor_Registration_Preserves_Identity_And_Uses_Direct_Factory()
    {
        var descriptor = Control("Button", "Height");
        var registration = new ControlTokenRegistration(descriptor);

        registration.Descriptor.ShouldBeSameAs(descriptor);
        registration.TryGetIdentity(out var identity).ShouldBeTrue();
        identity.ShouldBe(descriptor.Identity);
        registration.Activate().ShouldBeOfType<SchemaControlToken>();
    }

    [Fact]
    public void Control_Token_Attribute_Exposes_An_Explicit_Catalog()
    {
        var attribute = new ControlDesignTokenAttribute("Acme");

        attribute.Catalog.ShouldBe("Acme");
        new ControlDesignTokenAttribute().Catalog.ShouldBe("AtomUI");
    }

    [Fact]
    public void Token_Descriptor_Uses_Typed_Delegates_For_Parse_Access_And_Projection()
    {
        var descriptor = NumberToken("Scale", 0, TokenStage.Seed);
        var token = new SchemaDesignToken();

        descriptor.Parse("1.5").ShouldBe(1.5);
        descriptor.SetValue(token, 2.5);

        token.Scale.ShouldBe(2.5);
        descriptor.GetValue(token).ShouldBe(2.5);
        descriptor.ProjectResourceValue(token).ShouldBe(2.5);
        descriptor.ResourceKey.ShouldBe(SchemaResourceKey.Scale);
    }

    [Fact]
    public void Registry_Assigns_Deterministic_Control_Slots_And_Shares_Global_Schema()
    {
        var global = NumberToken("Scale", 0, TokenStage.Seed);
        var input = Control("Input", "InputValue");
        var button = Control("Button", "Height");

        var registry = new ThemeSchemaRegistry(
            [global],
            [input, button],
            Array.Empty<ThemeAlgorithmDescriptor>());

        registry.Controls.Select(static descriptor => descriptor.Identity.Id).ShouldBe(["Button", "Input"]);
        registry.Controls.Select(static descriptor => descriptor.Slot).ShouldBe([0, 1]);
        registry.Controls.ShouldAllBe(descriptor => ReferenceEquals(descriptor.InheritedTokens, registry.GlobalTokens));
        registry.TryGetControl(new ControlTokenIdentity("AtomUI", "Button"), out var boundButton).ShouldBeTrue();
        boundButton.ShouldNotBeSameAs(button);

        var builder = boundButton!.CreateBuilder().ShouldBeOfType<SchemaControlToken>();
        boundButton.Evaluate(builder, isDark: true);
        builder.WasEvaluatedAsDark.ShouldBeTrue();
    }

    [Fact]
    public void Algorithm_Descriptor_Validates_Base_Requirements_Before_Aot_Factory_Invocation()
    {
        var defaultDescriptor = new ThemeAlgorithmDescriptor(
            "Default",
            ThemeAppearanceEffect.Preserve,
            requiresBase: false,
            static _ => new SchemaAlgorithm());
        var darkDescriptor = new ThemeAlgorithmDescriptor(
            "Dark",
            ThemeAppearanceEffect.Dark,
            requiresBase: true,
            static algorithm => new SchemaAlgorithm(algorithm));

        var algorithm = defaultDescriptor.Create(null);

        darkDescriptor.Create(algorithm).ShouldBeOfType<SchemaAlgorithm>().Base.ShouldBeSameAs(algorithm);
        Should.Throw<InvalidOperationException>(() => darkDescriptor.Create(null));
    }

    [Fact]
    public void Shared_Parser_And_Resource_Projector_Are_Invariant_And_Immutable()
    {
        var original = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");
            ThemeTokenValueParser.Parse<double>("1.5").ShouldBe(1.5);
        }
        finally
        {
            CultureInfo.CurrentCulture = original;
        }

        var color = Color.Parse("#1677ff");
        ThemeResourceValue.Project(color)
                          .ShouldBeOfType<ImmutableSolidColorBrush>()
                          .Color
                          .ShouldBe(color);
    }

    private static TokenDescriptor NumberToken(string name, int slot, TokenStage stage)
    {
        return new TokenDescriptor(
            name,
            slot,
            stage,
            typeof(double),
            SchemaResourceKey.Scale,
            static value => ThemeTokenValueParser.Parse<double>(value),
            static value => ThemeTokenValueFormatter.Format((double)value!),
            static token => ((SchemaDesignToken)token).Scale,
            static (token, value) => ((SchemaDesignToken)token).Scale = (double)value!,
            static token => ThemeResourceValue.Project(((SchemaDesignToken)token).Scale));
    }

    private static ControlTokenDescriptor Control(string id, string tokenName)
    {
        var ownToken = new TokenDescriptor(
            tokenName,
            0,
            TokenStage.Control,
            typeof(double),
            SchemaResourceKey.Scale,
            static value => ThemeTokenValueParser.Parse<double>(value),
            static value => ThemeTokenValueFormatter.Format((double)value!),
            static token => ((SchemaControlToken)token).Value,
            static (token, value) => ((SchemaControlToken)token).Value = (double)value!,
            static token => ThemeResourceValue.Project(((SchemaControlToken)token).Value));
        return new ControlTokenDescriptor(
            new ControlTokenIdentity("AtomUI", id),
            [ownToken],
            static () => new SchemaControlToken(),
            static (token, isDark) => ((SchemaControlToken)token).WasEvaluatedAsDark = isDark);
    }

    private enum SchemaResourceKey
    {
        Scale
    }

    private sealed class SchemaDesignToken : AbstractDesignToken
    {
        public double Scale { get; set; }
    }

    private sealed class SchemaControlToken : AbstractControlDesignToken
    {
        public SchemaControlToken()
            : base("Schema")
        {
        }

        public double Value { get; set; }
        public bool WasEvaluatedAsDark { get; set; }

        protected override Type GetTokenKindType() => typeof(SchemaResourceKey);
    }

    private sealed class SchemaAlgorithm : IThemeAlgorithm
    {
        public SchemaAlgorithm(IThemeAlgorithm? algorithm = null)
        {
            Base = algorithm;
        }

        public IThemeAlgorithm? Base { get; }
        public Color ColorBgBase => default;
        public Color ColorTextBase => default;

        public void Calculate(DesignToken designToken)
        {
        }
    }
}
