using System.Reactive.Disposables;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

using AvaloniaTextBlock = Avalonia.Controls.TextBlock;

public class TextBlock : AvaloniaTextBlock, IResourceBindingManager
{
    #region 内部属性定义

    CompositeDisposable? IResourceBindingManager.ResourceBindingsDisposable => _resourceBindingsDisposable;
    
    #endregion
    
    private CompositeDisposable? _resourceBindingsDisposable;
    private double _deltaOffsetY;

    static TextBlock()
    {
        FontStyleProperty.OverrideDefaultValue<TextBlock>(FontStyle.Normal);
    }
    
    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _resourceBindingsDisposable = new CompositeDisposable();
        this.AddResourceBindingDisposable(TokenResourceBinder.CreateTokenBinding(this, LineHeightProperty, SharedTokenKey.FontHeight));
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        this.DisposeTokenBindings();
    }
    
    protected override void RenderTextLayout(DrawingContext context, Point origin)
    {
        base.RenderTextLayout(context, new Point(origin.X, origin.Y + _deltaOffsetY));
    }

    private void CalculateDeltaOffsetY()
    {
        if (!this.IsAttachedToVisualTree() || !OperatingSystem.IsMacOS())
        {
            return;
        }
        var typeface = new Typeface(FontFamily, FontStyle, FontWeight, FontStretch);
        var textLayout = new TextLayout(Text, typeface, null, FontSize, Foreground, textWrapping: TextWrapping, textAlignment: TextAlignment);
        double baseLine = textLayout.Baseline;
        double height = textLayout.Height;
        double offset = (height - baseLine) * 0.35;
        _deltaOffsetY = offset;
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == TextProperty || 
            change.Property == TextAlignmentProperty ||
            change.Property == TextWrappingProperty ||
            change.Property == FontSizeProperty ||
            change.Property == FontFamilyProperty ||
            change.Property == FontStyleProperty ||
            change.Property == FontWeightProperty ||
            change.Property == FontStretchProperty)
        {
            CalculateDeltaOffsetY();
        }
    }
}