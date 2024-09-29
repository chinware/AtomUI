using AtomUI.Media;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace AtomUI.Controls;

public class ArrowIndicator : Control
{    
    public static readonly StyledProperty<double> ArrowSizeProperty
        = AvaloniaProperty.Register<ArrowIndicator, double>(nameof(ArrowSize));
    
    public static readonly StyledProperty<IBrush?> FilledColorProperty =
        AvaloniaProperty.Register<Border, IBrush?>(nameof(FilledColor));
    
    public double ArrowSize
    {
        get => GetValue(ArrowSizeProperty);
        set => SetValue(ArrowSizeProperty, value);
    }
    
    public IBrush? FilledColor
    {
        get => GetValue(FilledColorProperty);
        set => SetValue(FilledColorProperty, value);
    }
    
    private Geometry? _arrowGeometry;

    static ArrowIndicator()
    {
        AffectsMeasure<ArrowIndicator>(ArrowSizeProperty);
        HorizontalAlignmentProperty.OverrideDefaultValue<ArrowIndicator>(HorizontalAlignment.Left);
        VerticalAlignmentProperty.OverrideDefaultValue<ArrowIndicator>(VerticalAlignment.Top);
        ClipToBoundsProperty.OverrideDefaultValue<ArrowIndicator>(true);
    }
    
    private void BuildGeometry(bool force = false)
    {
        if (_arrowGeometry is null || force)
        {
            _arrowGeometry = CommonShapeBuilder.BuildArrow(ArrowSize, 1.5);
            var matrix = Matrix.CreateTranslation(_arrowGeometry.Bounds.X, -_arrowGeometry.Bounds.Y);
            _arrowGeometry.Transform =  new MatrixTransform(matrix);
        }
    }
    
    public sealed override void ApplyTemplate()
    {
        base.ApplyTemplate();
        BuildGeometry();
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        return _arrowGeometry?.Bounds.Size ?? new Size(ArrowSize, ArrowSize);
    }

    public sealed override void Render(DrawingContext context)
    {
        if (_arrowGeometry is not null)
        {
            context.DrawGeometry(FilledColor, null, _arrowGeometry);
        }
    }
}