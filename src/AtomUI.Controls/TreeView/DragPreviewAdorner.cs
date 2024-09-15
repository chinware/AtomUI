﻿using AtomUI.Theme.Styling;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace AtomUI.Controls;

internal class DragPreviewAdorner : Decorator
{
    private readonly TranslateTransform _translation;
    private readonly DragPreview _dragPreview;

    public DragPreviewAdorner(Border previewControl)
    {
        _translation                 = new TranslateTransform();
        _dragPreview                 = new DragPreview(previewControl);
        _dragPreview.RenderTransform = _translation;
        Child                        = _dragPreview;
    }

    public double OffsetX
    {
        get => _translation.X;
        set => _translation.X = value;
    }

    /// <summary>
    /// The Preview's Offset in the Y direction from the GridSplitter.
    /// </summary>
    public double OffsetY
    {
        get => _translation.Y;
        set => _translation.Y = value;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        // Adorners always get clipped to the owner control. In this case we want
        // to constrain size to the splitter size but draw on top of the parent grid.
        Clip = null;
        return base.ArrangeOverride(finalSize);
    }
}

internal class DragPreview : Decorator
{
    public static readonly StyledProperty<IBrush?> BackgroundProperty =
        Border.BackgroundProperty.AddOwner<DragPreviewAdorner>();

    public IBrush? Background
    {
        get => GetValue(BackgroundProperty);
        set => SetValue(BackgroundProperty, value);
    }

    private readonly VisualBrush _visualBrush;

    public DragPreview(Border previewControl)
    {
        Width               = previewControl.Bounds.Width;
        Height              = previewControl.Bounds.Height;
        HorizontalAlignment = HorizontalAlignment.Left;
        VerticalAlignment   = VerticalAlignment.Top;
        _visualBrush = new VisualBrush
        {
            Visual     = previewControl,
            Stretch    = Stretch.None,
            AlignmentX = AlignmentX.Left
        };
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        TokenResourceBinder.CreateTokenBinding(this, BackgroundProperty, TreeViewTokenResourceKey.NodeHoverBg);
    }

    public override void Render(DrawingContext context)
    {
        using var state = context.PushOpacity(0.4);
        context.FillRectangle(Background!, new Rect(new Point(0, 0), Bounds.Size));
        context.FillRectangle(_visualBrush, new Rect(new Point(0, 0), Bounds.Size));
    }
}