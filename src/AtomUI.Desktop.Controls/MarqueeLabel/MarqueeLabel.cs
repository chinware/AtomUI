using AtomUI.Controls.Commons;
using AtomUI.Data;
using AtomUI.Desktop.Controls.DesignTokens;
using Avalonia;

namespace AtomUI.Desktop.Controls;

public class MarqueeLabel : AbstractMarqueeLabel
{
    private IDisposable? _tokenBindings;

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _tokenBindings?.Dispose();
        _tokenBindings = new System.Reactive.Disposables.CompositeDisposable
        {
            TokenResourceBinder.CreateControlTokenBinding(
                this,
                CycleSpaceProperty,
                MarqueeLabelTokenKind.CycleSpace),
            TokenResourceBinder.CreateControlTokenBinding(
                this,
                MoveSpeedProperty,
                MarqueeLabelTokenKind.DefaultSpeed)
        };
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _tokenBindings?.Dispose();
        _tokenBindings = null;
        base.OnDetachedFromVisualTree(e);
    }
}
