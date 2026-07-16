using System.Globalization;
using AtomUI.Theme.Algorithms;
using AtomUI.Theme.Configuration;
using AtomUI.Theme.Schema;
using AtomUI.Theme.TokenSystem;
using Avalonia.Media;
using Shouldly;
using Xunit;

namespace AtomUI.Core.Tests.Theme;

public class ThemeConfigNormalizerTests
{
    [Fact]
    public void Normalize_Captures_Typed_Values_And_Classifies_Control_Tokens()
    {
        var schema = ThemeConfigTestSchema.Create();
        var input = new ThemeConfig
        {
            Algorithms = ["Default"]
        };
        input.Tokens["Alpha"] = "1.5";
        input.Controls[ThemeConfigTestSchema.ButtonIdentity] = new ControlThemeConfig
        {
            Algorithm = ControlAlgorithmMode.Global,
            Tokens =
            {
                ["Alpha"] = "2.5",
                ["Height"] = "32"
            }
        };

        var result = ThemeConfigNormalizer.Normalize(input, schema);

        result.Success.ShouldBeTrue();
        var normalized = result.Config.ShouldNotBeNull();
        normalized.AlgorithmsSpecified.ShouldBeTrue();
        normalized.Algorithms.Select(static item => item.Id).ShouldBe(["Default"]);
        normalized.GlobalTokens.Single().Value.ShouldBe(1.5d);
        var button = normalized.Controls.Single();
        button.AlgorithmMode.ShouldBe(ControlAlgorithmMode.Global);
        button.GlobalTokens.Single().Value.ShouldBe(2.5d);
        button.OwnTokens.Single(item => item.Descriptor.Name == "Alpha").Value.ShouldBe(2.5d);
        button.OwnTokens.Single(item => item.Descriptor.Name == "Height").Value.ShouldBe(32d);
    }

    [Fact]
    public void Normalize_Captures_Input_And_Produces_Order_And_Culture_Independent_Fingerprint()
    {
        var schema = ThemeConfigTestSchema.Create();
        var first = new ThemeConfig
        {
            Algorithms = ["Default", "Compact"]
        };
        first.Tokens["Beta"] = "2.50";
        first.Tokens["Alpha"] = "1.50";

        var second = new ThemeConfig
        {
            Algorithms = ["Default", "Compact"]
        };
        second.Tokens["Alpha"] = "1.5";
        second.Tokens["Beta"] = "2.5";

        NormalizedThemeConfig firstNormalized;
        using (new CultureScope("fr-FR"))
        {
            firstNormalized = ThemeConfigNormalizer.Normalize(first, schema).Config.ShouldNotBeNull();
        }

        var secondNormalized = ThemeConfigNormalizer.Normalize(second, schema).Config.ShouldNotBeNull();
        firstNormalized.ShouldBe(secondNormalized);
        firstNormalized.Fingerprint.ShouldBe(secondNormalized.Fingerprint);

        first.Tokens["Alpha"] = "99";
        first.Algorithms![0] = "Compact";
        firstNormalized.GlobalTokens.Single(item => item.Descriptor.Name == "Alpha").Value.ShouldBe(1.5d);
        firstNormalized.Algorithms[0].Id.ShouldBe("Default");
    }

    [Fact]
    public void Normalize_Returns_Diagnostics_Without_Publishing_Partial_Config()
    {
        var schema = ThemeConfigTestSchema.Create();
        var input = new ThemeConfig
        {
            Algorithms = ["Unknown"]
        };
        input.Tokens["Alpha"] = "not-a-number";
        input.Tokens["Missing"] = "1";
        input.Controls[new ControlTokenIdentity("AtomUI", "Missing")] = new ControlThemeConfig();
        input.Controls[ThemeConfigTestSchema.ButtonIdentity] = new ControlThemeConfig
        {
            Algorithm = ControlAlgorithmMode.Custom
        };

        var result = ThemeConfigNormalizer.Normalize(input, schema);

        result.Success.ShouldBeFalse();
        result.Config.ShouldBeNull();
        result.Diagnostics.Select(static item => item.Code).ShouldContain("ATMTHM4001");
        result.Diagnostics.Select(static item => item.Code).ShouldContain("ATMTHM4002");
        result.Diagnostics.Select(static item => item.Code).ShouldContain("ATMTHM4004");
        result.Diagnostics.Select(static item => item.Code).ShouldContain("ATMTHM4005");
        result.Diagnostics.Select(static item => item.Code).ShouldContain("ATMTHM4006");
    }

    private sealed class CultureScope : IDisposable
    {
        private readonly CultureInfo _originalCulture = CultureInfo.CurrentCulture;
        private readonly CultureInfo _originalUICulture = CultureInfo.CurrentUICulture;

        internal CultureScope(string name)
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(name);
            CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(name);
        }

        public void Dispose()
        {
            CultureInfo.CurrentCulture = _originalCulture;
            CultureInfo.CurrentUICulture = _originalUICulture;
        }
    }
}

internal static class ThemeConfigTestSchema
{
    internal static readonly ControlTokenIdentity ButtonIdentity = new("AtomUI", "Button");

    internal static ThemeSchemaRegistry Create()
    {
        var globalTokens = new[]
        {
            Token("Alpha", 0, TokenStage.Seed),
            Token("Beta", 1, TokenStage.Alias)
        };
        var ownTokens = new[]
        {
            Token("Height", 0, TokenStage.Control),
            Token("Alpha", 1, TokenStage.Control)
        };
        var control = new ControlTokenDescriptor(
            ButtonIdentity,
            ownTokens,
            static () => new TestControlToken(),
            static (_, _) => { });
        var algorithms = new[]
        {
            new ThemeAlgorithmDescriptor(
                "Compact",
                ThemeAppearanceEffect.Preserve,
                false,
                static _ => new TestAlgorithm()),
            new ThemeAlgorithmDescriptor(
                "Default",
                ThemeAppearanceEffect.Preserve,
                false,
                static _ => new TestAlgorithm())
        };
        return new ThemeSchemaRegistry(globalTokens, [control], algorithms);
    }

    private static TokenDescriptor Token(string name, int slot, TokenStage stage)
    {
        return new TokenDescriptor(
            name,
            slot,
            stage,
            typeof(double),
            name,
            static value => ThemeTokenValueParser.Parse<double>(value),
            static value => ThemeTokenValueFormatter.Format((double)value!),
            static token => ((TestDesignToken)token).Value,
            static (token, value) => ((TestDesignToken)token).Value = (double)value!,
            static token => ((TestDesignToken)token).Value);
    }

    private sealed class TestDesignToken : AbstractDesignToken
    {
        internal double Value { get; set; }
    }

    private sealed class TestControlToken : AbstractControlDesignToken
    {
        internal TestControlToken()
            : base("Button")
        {
        }

        protected override Type GetTokenKindType() => typeof(TestTokenKind);
    }

    private sealed class TestAlgorithm : IThemeAlgorithm
    {
        public Color ColorBgBase => default;
        public Color ColorTextBase => default;

        public void Calculate(DesignToken designToken)
        {
        }
    }

    private enum TestTokenKind
    {
        Value
    }
}
