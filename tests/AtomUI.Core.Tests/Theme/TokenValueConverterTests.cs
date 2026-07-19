using System.Globalization;
using AtomUI.Theme;
using AtomUI.Theme.Tokens;
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
