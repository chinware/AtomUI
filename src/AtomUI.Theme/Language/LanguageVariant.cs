using System.ComponentModel;
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
            "RequestedLanguageVariant", defaultValue: zh_CN);
    
    public static LanguageVariant zh_CN { get; } = new(nameof(zh_CN));
    public static LanguageVariant en_US { get; } = new(nameof(en_US));

    private LanguageVariant(string key)
    {
        Key     = NormalizeLanguageCode(key);
    }
    
    /// <summary>
    /// Key of the theme variant by which variants are compared.
    /// </summary>
    public string Key { get; }

    public string DisplayText => GetDisplayText();
    
    public override string ToString()
    {
        return Key;
    }

    public override int GetHashCode()
    {
        return Key.GetHashCode();
    }

    public bool Equals(LanguageVariant? other)
    {
        return other is not null && Equals(Key, other.Key);
    }

    public static string NormalizeLanguageCode(string languageCode)
    {
        return languageCode.Replace("_", "-");
    }
    
    private string GetDisplayText()
    {
        if (this == zh_CN)
        {
            return "简体中文";
        }
        if (this == en_US)
        {
            return "English";
        }
        throw new InvalidEnumArgumentException("Unknown language variant.");
    }
}