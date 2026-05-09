using AtomUIThumb = AtomUI.Controls.Primitives.Thumb;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

internal class SplitterDragBar : AtomUIThumb
{
    public static readonly StyledProperty<Orientation> OrientationProperty =
        AvaloniaProperty.Register<SplitterDragBar, Orientation>(nameof(Orientation), Orientation.Vertical);

    public static readonly StyledProperty<IBrush?> LineBrushProperty =
        AvaloniaProperty.Register<SplitterDragBar, IBrush?>(nameof(LineBrush));

    public static readonly StyledProperty<double> LineThicknessProperty =
        AvaloniaProperty.Register<SplitterDragBar, double>(nameof(LineThickness));

    public static readonly StyledProperty<bool> IsDragEnabledProperty =
        AvaloniaProperty.Register<SplitterDragBar, bool>(nameof(IsDragEnabled), true);

    public Orientation Orientation
    {
        get => GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    public IBrush? LineBrush
    {
        get => GetValue(LineBrushProperty);
        set => SetValue(LineBrushProperty, value);
    }

    public double LineThickness
    {
        get => GetValue(LineThicknessProperty);
        set => SetValue(LineThicknessProperty, value);
    }

    public bool IsDragEnabled
    {
        get => GetValue(IsDragEnabledProperty);
        set => SetValue(IsDragEnabledProperty, value);
    }
    
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        if (!IsDragEnabled)
        {
            return;
        }
        base.OnPointerPressed(e);
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        if (!IsDragEnabled)
        {
            return;
        }
        base.OnPointerMoved(e);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        if (!IsDragEnabled)
        {
            return;
        }
        base.OnPointerReleased(e);
    }

    internal void SetDragging(bool isDragging)
    {
        PseudoClasses.Set(StdPseudoClass.Dragging, isDragging);
    }
}
