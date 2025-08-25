using System.Reactive.Disposables;
using AtomUI.Theme;
using AtomUI.Theme.Data;
using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;
using Avalonia.VisualTree;

namespace AtomUI.Controls;

using AvaloniaTextBlock = Avalonia.Controls.TextBlock;

public class TextBlock : AvaloniaTextBlock, IResourceBindingManager
{
    #region 内部属性定义

    CompositeDisposable? IResourceBindingManager.ResourceBindingsDisposable { get; set; }
    
    #endregion
    
    private double _deltaOffsetY;

    static TextBlock()
    {
        FontStyleProperty.OverrideDefaultValue<TextBlock>(FontStyle.Normal);
    }

    protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnAttachedToLogicalTree(e);
        this.AddResourceBindingDisposable(TokenResourceBinder.CreateTokenBinding(this, LineHeightProperty, SharedTokenKey.FontHeight));
    }

    protected override void OnDetachedFromLogicalTree(LogicalTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromLogicalTree(e);
        this.DisposeTokenBindings();
    }
    
    protected override void RenderTextLayout(DrawingContext context, Point origin)
    {
        base.RenderTextLayout(context, new Point(origin.X, origin.Y + _deltaOffsetY));
    }

    private void CalculateDeltaOffsetY()
    {
        if (!this.IsAttachedToVisualTree() || !OperatingSystem.IsMacOS() || MathUtils.AreClose(FontSize, 0))
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