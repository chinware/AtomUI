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
        {
            var culture = CultureInfo.GetCultureInfo(cultureName);
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
