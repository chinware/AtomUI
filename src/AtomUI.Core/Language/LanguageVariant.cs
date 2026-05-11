using System.ComponentModel;
using System.Globalization;
using Avalonia;

namespace AtomUI.Theme.Language;

[TypeConverter(typeof(LanguageVariantTypeConverter))]
public sealed record LanguageVariant
{
    public static LanguageVariant zh_CN { get; } = new(LanguageCode.zh_CN);
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
            case LanguageCode.en_US:
                return en_US;
            default:
                return en_US;
        }
    }

    public static LanguageVariant FromCultureInfo(CultureInfo cultureInfo)
    {
        // CultureInfo.ToString() returns the hyphen form (e.g. "zh-CN"); LanguageCode enum names use '_'.
        var codeStr = cultureInfo.ToString().Replace('-', '_');
        if (Enum.TryParse<LanguageCode>(codeStr, out var code))
        {
            return FromCode(code);
        }

        // Fuzzy fallback: map any Chinese locale to zh_CN, everything else to en_US.
        return cultureInfo.TwoLetterISOLanguageName.Equals("zh", StringComparison.OrdinalIgnoreCase)
            ? zh_CN
            : en_US;
    }

    public CultureInfo ToCultureInfo()
    {
        return CultureInfo.GetCultureInfo(ToString());
    }
}