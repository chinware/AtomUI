using System.Globalization;
using AtomUI.Theme;
using AtomUI.Theme.TokenSystem;
using Avalonia;
using Shouldly;
using Xunit;

namespace AtomUI.Core.Tests.Theme;

public class TokenValueConverterTests
{
    [Theory]
    [InlineData("en-US")]
    [InlineData("fr-FR")]
    [InlineData("zh-CN")]
    public void Integer_Conversion_Uses_Invariant_Culture(string cultureName)
    {
        using var culture = new CultureScope(cultureName);

        new IntegerTokenValueConverter().Convert("-12").ShouldBe(-12);
    }

    [Fact]
    public void Integer_Conversion_Uses_Invariant_Negative_Sign()
    {
        var culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
        culture.NumberFormat.NegativeSign = "~";
        using var cultureScope = new CultureScope(culture);

        new IntegerTokenValueConverter().Convert("-12").ShouldBe(-12);
    }

    [Theory]
    [InlineData("en-US")]
    [InlineData("fr-FR")]
    [InlineData("zh-CN")]
    public void Double_Conversion_Uses_Invariant_Culture(string cultureName)
    {
        using var culture = new CultureScope(cultureName);

        new DoubleTokenValueConverter().Convert("1.5").ShouldBe(1.5d);
    }

    [Theory]
    [InlineData("en-US")]
    [InlineData("fr-FR")]
    [InlineData("zh-CN")]
    public void Float_Conversion_Uses_Invariant_Culture(string cultureName)
    {
        using var culture = new CultureScope(cultureName);

        new FloatTokenValueConverter().Convert("1.5").ShouldBe(1.5f);
    }

    [Fact]
    public void LoadConfig_Invalidates_Previously_Read_Token_Value()
    {
        var token = new DesignToken();
        token.GetTokenValue(nameof(DesignToken.BorderRadius));

        token.LoadConfig(new Dictionary<string, string>
        {
            [nameof(DesignToken.BorderRadius)] = "12"
        });

        token.GetTokenValue(nameof(DesignToken.BorderRadius))
             .ShouldBe(new CornerRadius(12));
    }

    [Fact]
    public void LoadConfig_Does_Not_Apply_Earlier_Tokens_When_A_Later_Conversion_Fails()
    {
        var token = new DesignToken();
        var originalFontSize = token.FontSize;
        var originalLineWidth = token.LineWidth;
        token.GetTokenValue(nameof(DesignToken.FontSize));
        token.GetTokenValue(nameof(DesignToken.LineWidth));

        Should.Throw<ThemeLoadException>(() => token.LoadConfig(new Dictionary<string, string>
        {
            [nameof(DesignToken.FontSize)] = "18",
            [nameof(DesignToken.LineWidth)] = "not-a-number"
        }));

        token.FontSize.ShouldBe(originalFontSize);
        token.LineWidth.ShouldBe(originalLineWidth);
        token.GetTokenValue(nameof(DesignToken.FontSize)).ShouldBe(originalFontSize);
        token.GetTokenValue(nameof(DesignToken.LineWidth)).ShouldBe(originalLineWidth);
    }

    [Fact]
    public void LoadConfig_Conversion_Error_Includes_Token_Context()
    {
        var token = new DesignToken();

        var exception = Should.Throw<ThemeLoadException>(() => token.LoadConfig(new Dictionary<string, string>
        {
            [nameof(DesignToken.FontSize)] = "not-a-number"
        }));

        var messages = new List<string>();
        for (Exception? current = exception; current is not null; current = current.InnerException)
        {
            messages.Add(current.Message);
        }

        string.Join("\n", messages).ShouldContain(nameof(DesignToken.FontSize));
        string.Join("\n", messages).ShouldContain(typeof(double).FullName!);
        string.Join("\n", messages).ShouldContain("not-a-number");
    }

    private sealed class CultureScope : IDisposable
    {
        private readonly CultureInfo _originalCulture = CultureInfo.CurrentCulture;
        private readonly CultureInfo _originalUICulture = CultureInfo.CurrentUICulture;

        public CultureScope(string cultureName)
            : this(CultureInfo.GetCultureInfo(cultureName))
        {
        }

        public CultureScope(CultureInfo culture)
        {
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
        }

        public void Dispose()
        {
            CultureInfo.CurrentCulture = _originalCulture;
            CultureInfo.CurrentUICulture = _originalUICulture;
        }
    }
}
