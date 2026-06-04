using System.ComponentModel;
using System.Globalization;
using Avalonia;

namespace AtomUI.Theme.Language;

[TypeConverter(typeof(LanguageVariantTypeConverter))]
public sealed record LanguageVariant
{
    public static LanguageVariant zh_CN { get; } = new(LanguageCode.zh_CN);
    public static LanguageVariant zh_TW { get; } = new(LanguageCode.zh_TW);
    public static LanguageVariant en_US { get; } = new(LanguageCode.en_US);
    
    internal static readonly StyledProperty<LanguageVariant> LanguageVariantProperty =
        AvaloniaProperty.Register<StyledElement, LanguageVariant>(
            "LanguageVariant", defaultValue: en_US);

    private LanguageVariant(LanguageCode code)
    {
        Code = code;
    }
    
    public LanguageCode Code { get; }

    public string DisplayText => GetDisplayText();
    
    public override string ToString()
    {
        return Code.ToHyphenString();
    }

    public override int GetHashCode()
    {
        return Code.GetHashCode();
    }

    public bool Equals(LanguageVariant? other)
    {
        return other is not null && Equals(Code, other.Code);
    }
    
    private string GetDisplayText()
    {
        switch (Code)
        {
            case LanguageCode.zh_CN:
                return "简体中文";
            case LanguageCode.zh_TW:
                return "繁體中文";
            case LanguageCode.en_US:
                return "English";
        }
        throw new InvalidEnumArgumentException("Unknown language variant.");
    }

    public static LanguageVariant FromCode(LanguageCode code)
    {
        switch (code)
        {
            case LanguageCode.zh_CN:
                return zh_CN;
            case LanguageCode.zh_TW:
                return zh_TW;
            case LanguageCode.en_US:
                return en_US;
            default:
                return en_US;
        }
    }

    public static LanguageVariant FromCultureInfo(CultureInfo cultureInfo)
    {
        // CultureInfo.Name usually returns the hyphen form (e.g. "zh-CN"); LanguageCode enum names use '_'.
        var cultureName = cultureInfo.Name;
        Span<char> codeBuffer = stackalloc char[cultureName.Length];
        for (int i = 0; i < cultureName.Length; i++)
        {
            var ch = cultureName[i];
            codeBuffer[i] = ch == '-' ? '_' : ch;
        }

        if (Enum.TryParse<LanguageCode>(codeBuffer, out var code))
        {
            return FromCode(code);
        }

        if (cultureInfo.TwoLetterISOLanguageName.Equals("zh", StringComparison.OrdinalIgnoreCase))
        {
            var cultureNameLower = cultureInfo.Name.ToLowerInvariant();
            return cultureNameLower.Contains("tw", StringComparison.Ordinal) ||
                   cultureNameLower.Contains("hk", StringComparison.Ordinal) ||
                   cultureNameLower.Contains("mo", StringComparison.Ordinal) ||
                   cultureNameLower.Contains("hant", StringComparison.Ordinal)
                ? zh_TW
                : zh_CN;
        }

        return en_US;
    }

    public CultureInfo ToCultureInfo()
    {
        return CultureInfo.GetCultureInfo(ToString());
    }
}
