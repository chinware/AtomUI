using System.ComponentModel;
using System.Globalization;

namespace AtomUI.Theme.Language;

public class LanguageVariantTypeConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(string);
    }

    public override object ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
        return value switch
        {
            _ => throw new NotSupportedException("LanguageVariant type converter supports only build in variants. For custom variants please use x:Static markup extension.")
        };
    }
}
