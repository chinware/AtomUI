using Avalonia.Markup.Xaml;

namespace AtomUI.Demo.Desktop;

/// <summary>
/// Xaml markup to get the enum values.
/// </summary>
/// TODO 优化性能时可以考虑缓存类型和列表
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
        // Array can not perform well for Items of ListBox.
        // Version : 11.0.0-preview4
        // By nlb at 2023.3.28.
        return Enum.GetValues(Type).OfType<object>().ToList();
    }
}