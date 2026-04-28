using System.Diagnostics;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Desktop.Controls;

internal class WindowsCaptionButton : CaptionButton
{
    internal static readonly DirectProperty<WindowsCaptionButton, bool> IsCloseButtonProperty =
        AvaloniaProperty.RegisterDirect<WindowsCaptionButton, bool>(nameof(IsCloseButton),
            o => o.IsCloseButton,
            (o, v) => o.IsCloseButton = v);

    private bool _isCloseButton;

    internal bool IsCloseButton
    {
        get => _isCloseButton;
        set => SetAndRaise(IsCloseButtonProperty, ref _isCloseButton, value);
    }
    
    protected override void OnSizeChanged(SizeChangedEventArgs e)
    {
        base.OnSizeChanged(e);
        Debug.Assert(MathUtils.AreClose(DesiredSize.Width, DesiredSize.Height));
        EffectiveCornerRadius = new CornerRadius(0);
    }
    
    protected override Size MeasureOverride(Size availableSize)
    {
        var width  = availableSize.Width;
        var height = availableSize.Height;
        if (double.IsInfinity(width))
        {
            width = height;
        }
        var availableMinSize = Math.Min(width, height);
        var size             = base.MeasureOverride(new Size(width, height));
        var minSize          = Math.Min(size.Width, size.Height);
        var finalSize = Math.Max(minSize, availableMinSize);
        return new Size(finalSize, finalSize);
    }
}