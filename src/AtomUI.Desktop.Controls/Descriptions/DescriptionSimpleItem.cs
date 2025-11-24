using Avalonia;
using Avalonia.Controls.Primitives;

namespace AtomUI.Desktop.Controls;

internal class DescriptionSimpleItem : HeaderedContentControl
{
    public static readonly StyledProperty<double> RelativeLineHeightProperty =
        AvaloniaProperty.Register<DescriptionSimpleItem, double>(nameof(RelativeLineHeight));
    
    public static readonly StyledProperty<double> LineHeightProperty =
        AvaloniaProperty.Register<DescriptionSimpleItem, double>(nameof(LineHeight));
    
    internal static readonly DirectProperty<DescriptionSimpleItem, bool> IsColonVisibleProperty =
        AvaloniaProperty.RegisterDirect<DescriptionSimpleItem, bool>(nameof(IsColonVisible),
            o => o.IsColonVisible,
            (o, v) => o.IsColonVisible = v);
    
    public double RelativeLineHeight
    {
        get => GetValue(RelativeLineHeightProperty);
        set => SetValue(RelativeLineHeightProperty, value);
    }
    
    public double LineHeight
    {
        get => GetValue(LineHeightProperty);
        set => SetValue(LineHeightProperty, value);
    }
    
    private bool _isColonVisible;

    internal bool IsColonVisible
    {
        get => _isColonVisible;
        set => SetAndRaise(IsColonVisibleProperty, ref _isColonVisible, value);
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == RelativeLineHeightProperty ||
            change.Property == FontSizeProperty)
        {
            SetCurrentValue(LineHeightProperty, RelativeLineHeight * FontSize);
        }
    }
}