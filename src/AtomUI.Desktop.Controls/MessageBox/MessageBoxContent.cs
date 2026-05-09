using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

internal class MessageBoxContent : ContentControl
{
    public static readonly StyledProperty<PathIcon?> StyleIconProperty =
        AvaloniaProperty.Register<MessageBoxContent, PathIcon?>(nameof(StyleIcon));

    public static readonly StyledProperty<MessageBoxStyle> StyleProperty =
        MessageBox.StyleProperty.AddOwner<MessageBoxContent>();

    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<MessageBoxContent, string?>(nameof(Title));

    public PathIcon? StyleIcon
    {
        get => GetValue(StyleIconProperty);
        set => SetValue(StyleIconProperty, value);
    }

    public MessageBoxStyle Style
    {
        get => GetValue(StyleProperty);
        set => SetValue(StyleProperty, value);
    }

    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    // 预留给 host chrome（标题栏按钮/边距）和 body padding 的安全边距，用来保证测到的
    // title 宽度不会被关闭按钮等 chrome 元素压掉。
    private const double TitleChromeOverhead = 160d;

    protected override Size MeasureOverride(Size availableSize)
    {
        var natural = base.MeasureOverride(availableSize);

        if (string.IsNullOrEmpty(Title))
        {
            return natural;
        }

        var titleWidth = MeasureTitleWidth(Title!);
        var requiredWidth = titleWidth + TitleChromeOverhead;
        if (requiredWidth <= natural.Width)
        {
            return natural;
        }

        var width = Math.Min(requiredWidth, availableSize.Width);
        return new Size(width, natural.Height);
    }

    private double MeasureTitleWidth(string title)
    {
        var typeface = new Typeface(FontFamily, FontStyle, FontWeight, FontStretch);
        var formatted = new FormattedText(
            title,
            System.Globalization.CultureInfo.CurrentCulture,
            FlowDirection.LeftToRight,
            typeface,
            FontSize,
            Foreground);
        return formatted.Width;
    }
}
