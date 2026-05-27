using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

internal class DataGridColumnDraggingOverIndicator : Control
{
    internal static readonly DirectProperty<DataGridColumnDraggingOverIndicator, DataGridColumn?> DraggingOverColumnProperty =
        AvaloniaProperty.RegisterDirect<DataGridColumnDraggingOverIndicator, DataGridColumn?>(
            nameof(DraggingOverColumn),
            o => o.DraggingOverColumn,
            (o, v) => o.DraggingOverColumn = v);
    
    internal static readonly DirectProperty<DataGridColumnDraggingOverIndicator, DataGridColumn?> DraggedColumnProperty =
        AvaloniaProperty.RegisterDirect<DataGridColumnDraggingOverIndicator, DataGridColumn?>(
            nameof(DraggedColumn),
            o => o.DraggedColumn,
            (o, v) => o.DraggedColumn = v);
    
    internal DataGridColumn? DraggingOverColumn
    {
        get => _draggingOverColumn;
        set => SetAndRaise(DraggingOverColumnProperty, ref _draggingOverColumn, value);
    }
    private DataGridColumn? _draggingOverColumn;
    
    internal DataGridColumn? DraggedColumn
    {
        get => _draggedColumn;
        set => SetAndRaise(DraggedColumnProperty, ref _draggedColumn, value);
    }
    private DataGridColumn? _draggedColumn;

    static DataGridColumnDraggingOverIndicator()
    {
        AffectsRender<DataGridColumnDraggingOverIndicator>(
            DraggingOverColumnProperty,
            DraggedColumnProperty,
            TextElement.ForegroundProperty);
    }

    private double _indicatorOffsetX;
    private const double IndicatorLineWidth = 1.0;
    private Pen? _indicatorPen;
    private IBrush? _indicatorPenBrush;

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        if (change.Property == DraggingOverColumnProperty ||
            change.Property == DraggedColumnProperty)
        {
            CalculateIndicatorOffset();
        }
        else if (change.Property == TextElement.ForegroundProperty)
        {
            _indicatorPen = null;
            _indicatorPenBrush = null;
        }
    }

    private void CalculateIndicatorOffset()
    {
        if (DraggingOverColumn != null && DraggedColumn != null)
        {
            var offset = DraggingOverColumn.HeaderCell.TranslatePoint(new Point(0, 0), this);
            if (offset.HasValue)
            {
                var headerSize = DraggingOverColumn.HeaderCell.Bounds.Size;
                if (DraggedColumn.DisplayIndex < DraggingOverColumn.DisplayIndex)
                {
                    _indicatorOffsetX = offset.Value.X + headerSize.Width;
                }
                else
                {
                    _indicatorOffsetX = offset.Value.X;
                }
            }
        }
    }

    public override void Render(DrawingContext context)
    {
        if (DraggingOverColumn != null)
        {
            using var state = context.PushRenderOptions(new RenderOptions
            {
                EdgeMode = EdgeMode.Aliased
            });
            var startPoint = new Point(_indicatorOffsetX, 0);
            var endPoint   = new Point(_indicatorOffsetX, Bounds.Height);
            context.DrawLine(GetIndicatorPen(), startPoint, endPoint);
        }
    }

    private Pen GetIndicatorPen()
    {
        var foreground = TextElement.GetForeground(this);
        if (_indicatorPen == null || !ReferenceEquals(_indicatorPenBrush, foreground))
        {
            _indicatorPenBrush = foreground;
            _indicatorPen      = new Pen(foreground, IndicatorLineWidth, DashStyle.Dash);
        }
        return _indicatorPen;
    }
}
