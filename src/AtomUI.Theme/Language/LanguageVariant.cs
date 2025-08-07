using System.ComponentModel;
using System.Globalization;
using Avalonia;

namespace AtomUI.Theme.Language;

[TypeConverter(typeof(LanguageVariantTypeConverter))]
public sealed record LanguageVariant
{
    /// <summary>
    /// Defines the ActualLanguageVariant property.
    /// </summary>
    internal static readonly StyledProperty<LanguageVariant> ActualLanguageVariantProperty =
        AvaloniaProperty.Register<StyledElement, LanguageVariant>(
            "ActualLanguageVariant",
            inherits: true);

    /// <summary>
    /// Defines the RequestedLanguageVariant property.
    /// </summary>
    internal static readonly StyledProperty<LanguageVariant?> RequestedLanguageVariantProperty =
        AvaloniaProperty.Register<StyledElement, LanguageVariant?>(
            "RequestedLanguageVariant", defaultValue: en_US);
    
    public static LanguageVariant zh_CN { get; } = new(LanguageCode.zh_CN);
    public static LanguageVariant en_US { get; } = new(LanguageCode.en_US);

    private LanguageVariant(LanguageCode code)
    {
        Code        = code;
    }
    
    public LanguageCode Code { get; }

    public string DisplayText => GetDisplayText();
    
    public override string ToString()
    {
        return NormalizeLanguageCode(Code.ToString());
    }

    public override int GetHashCode()
    {
        return Code.GetHashCode();
    }

    public bool Equals(LanguageVariant? other)
    {
        return other is not null && Equals(Code, other.Code);
    }

    public static string NormalizeLanguageCode(string languageCode)
    {
        return languageCode.Replace("_", "-");
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
        var codeStr =  cultureInfo.ToString();
        if (!Enum.TryParse<LanguageCode>(codeStr, out var code))
        {
            // TODO 输出日志
            return zh_CN;
        }
        return FromCode(code);
    }

    public CultureInfo ToCultureInfo()
    {
        return CultureInfo.GetCultureInfo(ToString());
    }
}