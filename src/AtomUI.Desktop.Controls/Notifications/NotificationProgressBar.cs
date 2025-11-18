using AtomUI.Utils;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

internal class NotificationProgressBar : TemplatedControl
{
    #region 公共属性定义

    public static readonly StyledProperty<double> ProgressIndicatorThicknessProperty =
        AvaloniaProperty.Register<NotificationProgressBar, double>(nameof(ProgressIndicatorThickness));

    public static readonly StyledProperty<IBrush?> ProgressIndicatorBrushProperty =
        AvaloniaProperty.Register<NotificationProgressBar, IBrush?>(nameof(ProgressIndicatorBrush));

    public static readonly StyledProperty<TimeSpan> ExpirationProperty =
        AvaloniaProperty.Register<NotificationProgressBar, TimeSpan>(nameof(Expiration));

    public static readonly StyledProperty<TimeSpan> CurrentExpirationProperty =
        AvaloniaProperty.Register<NotificationProgressBar, TimeSpan>(nameof(CurrentExpiration));

    internal double ProgressIndicatorThickness
    {
        get => GetValue(ProgressIndicatorThicknessProperty);
        set => SetValue(ProgressIndicatorThicknessProperty, value);
    }

    public IBrush? ProgressIndicatorBrush
    {
        get => GetValue(ProgressIndicatorBrushProperty);
        set => SetValue(ProgressIndicatorBrushProperty, value);
    }

    public TimeSpan Expiration
    {
        get => GetValue(ExpirationProperty);
        set => SetValue(ExpirationProperty, value);
    }

    public TimeSpan CurrentExpiration
    {
        get => GetValue(CurrentExpirationProperty);
        set => SetValue(CurrentExpirationProperty, value);
    }

    #endregion
    
    static NotificationProgressBar()
    {
        AffectsMeasure<NotificationProgressBar>(ProgressIndicatorThicknessProperty);
        AffectsRender<NotificationProgressBar>(ProgressIndicatorBrushProperty,
            ExpirationProperty,
            CurrentExpirationProperty);
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        return new Size(availableSize.Width, ProgressIndicatorThickness);
    }

    public override void Render(DrawingContext context)
    {
        var indicatorWidth = 0d;
        var total          = Expiration.TotalMilliseconds;
        if (MathUtils.GreaterThan(total, 0))
        {
            indicatorWidth = CurrentExpiration.TotalMilliseconds / total * Bounds.Width;
        }

        var offsetY       = Bounds.Height - ProgressIndicatorThickness;
        var indicatorRect = new Rect(new Point(0, offsetY), new Size(indicatorWidth, ProgressIndicatorThickness));
        context.FillRectangle(ProgressIndicatorBrush!, indicatorRect);
    }
}