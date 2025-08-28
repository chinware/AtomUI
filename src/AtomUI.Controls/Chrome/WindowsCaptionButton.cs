using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Utilities;

namespace AtomUI.Controls;

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
        Debug.Assert(MathUtilities.AreClose(DesiredSize.Width, DesiredSize.Height));
        EffectiveCornerRadius = new CornerRadius(0);
    }
}