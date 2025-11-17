using System.ComponentModel;
using System.Globalization;
using AtomUI.Theme;

namespace AtomUI.Utils;

public class ThemeAlgorithmTypeConverter : TypeConverter
{
    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
    {
        return sourceType == typeof (string);
    }

    public override object ConvertFrom(
        ITypeDescriptorContext? context,
        CultureInfo? culture,
        object value)
    {
        if (value is string str)
        {
            switch (str)
            {
                case "DefaultAlgorithm":
                    return ThemeAlgorithm.Default;
                case "DarkAlgorithm":
                    return ThemeAlgorithm.Dark;
                case "CompactAlgorithm":
                    return ThemeAlgorithm.Compact;
            }
        }
        throw new NotSupportedException("ThemeAlgorithm type converter supports only build in variants. For custom variants please use x:Static markup extension.");
    }
}

