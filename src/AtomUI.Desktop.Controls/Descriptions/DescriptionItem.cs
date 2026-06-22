using AtomUI.Controls;
using Avalonia;
using Avalonia.Collections;

namespace AtomUI.Desktop.Controls;

[GenerateScopedResourceHost]
public partial class DescriptionItem : AvaloniaObject
{
    #region 公共属性定义

    public static readonly DirectProperty<DescriptionItem, string> LabelProperty =
        AvaloniaProperty.RegisterDirect<DescriptionItem, string>(
            nameof(Label),
            o => o.Label,
            (o, v) => o.Label = v);

    public static readonly DirectProperty<DescriptionItem, object?> ContentProperty =
        AvaloniaProperty.RegisterDirect<DescriptionItem, object?>(
            nameof(Content),
            o => o.Content,
            (o, v) => o.Content = v);

    public static readonly DirectProperty<DescriptionItem, bool> IsFilledProperty =
        AvaloniaProperty.RegisterDirect<DescriptionItem, bool>(
            nameof(IsFilled),
            o => o.IsFilled,
            (o, v) => o.IsFilled = v);

    public static readonly DirectProperty<DescriptionItem, ResponsiveInt> SpanProperty =
        AvaloniaProperty.RegisterDirect<DescriptionItem, ResponsiveInt>(
            nameof(Span),
            o => o.Span,
            (o, v) => o.Span = v);

    private string _label = string.Empty;

    public string Label
    {
        get => _label;
        set => SetAndRaise(LabelProperty, ref _label, value);
    }

    private object? _content;

    public object? Content
    {
        get => _content;
        set => SetAndRaise(ContentProperty, ref _content, value);
    }

    private bool _isFilled;

    public bool IsFilled
    {
        get => _isFilled;
        set => SetAndRaise(IsFilledProperty, ref _isFilled, value);
    }

    private ResponsiveInt _span = new(1);

    public ResponsiveInt Span
    {
        get => _span;
        set => SetAndRaise(SpanProperty, ref _span, value);
    }

    #endregion
}

public class DescriptionItems : AvaloniaList<DescriptionItem> {}
