using System.ComponentModel;
using System.Globalization;

namespace AtomUI.Controls;

public sealed class ImageSourceConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
    }

    public override object? ConvertFrom(
        ITypeDescriptorContext? context,
        CultureInfo? culture,
        object value)
    {
        return value is string source
            ? ImageSource.Parse(source)
            : base.ConvertFrom(context, culture, value);
    }
}
