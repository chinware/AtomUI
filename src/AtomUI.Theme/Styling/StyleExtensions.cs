using AtomUI.Theme.TokenSystem;
using Avalonia;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Styling;

namespace AtomUI.Theme.Styling;

internal static class StyleExtensions
{
    public static StyleBase Add(this StyleBase style, AvaloniaProperty targetProperty, object? value)
    {
        style.Add(new Setter(targetProperty, value));
        return style;
    }

    public static StyleBase Add<T>(this StyleBase style, AvaloniaProperty targetProperty, Func<T?> valueFactory)
        where T : class
    {
        style.Add(new Setter(targetProperty, new SetterValueFactory<T>(valueFactory)));
        return style;
    }

    public static StyleBase Add(this StyleBase style, AvaloniaProperty targetProperty, TokenResourceKey resourceKey)
    {
        style.Add(new Setter(targetProperty, new DynamicResourceExtension(resourceKey.QualifiedKey())));
        return style;
    }
}