using Avalonia.Markup.Xaml;

namespace AtomUIGallery.Utils;

public class EnumExtension : MarkupExtension
{
    [ConstructorArgument(nameof(Type))] public Type Type { get; set; }

    public EnumExtension(Type type)
    {
        Type = type;
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        // Issue I7:
        // Array can not perform well for Items of List.
        // Version : 11.0.0-preview4
        // By nlb at 2023.3.28.
        return Enum.GetValuesAsUnderlyingType(Type).OfType<object>().ToList();
    }
}