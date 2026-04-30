using Avalonia.Markup.Xaml;

namespace AtomUIGallery.Utils;

public class EnumExtension : MarkupExtension
{
    public Type Type { get; set; }

    public EnumExtension(Type type)
    {
        Type = type;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return Enum.GetValuesAsUnderlyingType(Type).OfType<object>().ToList();
    }
}
