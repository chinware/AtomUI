using AtomUI.Media;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls.Commons;

public abstract class AbstractGeneralCircleProgress : AbstractCircleProgress
{
    private Rect _currentGrooveRect;
    private readonly Pen _groovePen = new();
    private readonly Pen _indicatorPen = new();
    private readonly Pen _successPen = new();

    protected override void RenderGroove(DrawingContext context)
    {
        var controlRect = new Rect(new Point(0, 0), Bounds.Size);

        _currentGrooveRect = GetProgressBarRect(controlRect).Deflate(StrokeThickness / 2);
        _currentGrooveRect = new Rect(_currentGrooveRect.Position, new Size(Math.Floor(_currentGrooveRect.Size.Width),
            Math.Floor(_currentGrooveRect.Size.Height)));
        if (StepCount > 0 && StepGap > 0)
        {
            DrawGrooveStep(context);
        }
        else
        {
            DrawGrooveNormal(context);
        }
    }

    private void DrawGrooveNormal(DrawingContext context)
    {
        context.DrawEllipse(null, ConfigurePen(_groovePen, GrooveBrush, PenLineCap.Flat), _currentGrooveRect);
    }

    private void DrawGrooveStep(DrawingContext context)
    {
        var pen        = ConfigurePen(_groovePen, GrooveBrush, PenLineCap.Flat);
        var spanAngle  = (360 - StepGap * StepCount) / StepCount;
        var startAngle = -90d;
        for (var i = 0; i < StepCount; ++i)
        {
            context.DrawArc(pen, _currentGrooveRect, startAngle, spanAngle);
            startAngle += StepGap + spanAngle;
        }
    }

    protected override void RenderIndicatorBar(DrawingContext context)
    {
        if (StepCount > 0 && StepGap > 0)
        {
            DrawIndicatorBarStep(context);
        }
        else
        {
            DrawIndicatorBarNormal(context);
        }
    }

    private void DrawIndicatorBarNormal(DrawingContext context)
    {
        var pen = ConfigurePen(_indicatorPen, StrokeBrush, StrokeLineCap);

        double startAngle = -90;
        context.DrawArc(pen, _currentGrooveRect, startAngle, IndicatorAngle);

        if (!double.IsNaN(SuccessThreshold))
        {
            var successPen = ConfigurePen(_successPen, SuccessStrokeBrush, StrokeLineCap);
            context.DrawArc(successPen, _currentGrooveRect, startAngle, CalculateAngle(SuccessThreshold));
        }
    }

    private void DrawIndicatorBarStep(DrawingContext context)
    {
        var pen = ConfigurePen(_indicatorPen, StrokeBrush, PenLineCap.Flat);

        var   filledSteps  = (int)Math.Round(StepCount * Percentage / 100);
        int?  successSteps = null;
        IPen? successPen   = null;

        if (!double.IsNaN(SuccessThreshold))
        {
            successPen   = ConfigurePen(_successPen, SuccessStrokeBrush, PenLineCap.Flat);
            successSteps = (int)Math.Round(StepCount * CalculatePercentageValue(SuccessThreshold) / 100);
        }

        var   spanAngle  = (360 - StepGap * StepCount) / StepCount;
        var   startAngle = -90d;
        IPen? currentPen;
        for (var i = 0; i < filledSteps; ++i)
        {
            currentPen = pen;
            if (successSteps.HasValue)
            {
                if (i < successSteps)
                {
                    currentPen = successPen;
                }
            }

            context.DrawArc(currentPen, _currentGrooveRect, startAngle, spanAngle);
            startAngle += StepGap + spanAngle;
        }
    }

    protected override void NotifyUpdateProgress()
    {
        base.NotifyUpdateProgress();
        IndicatorAngle = CalculateAngle(Value);
    }

    private double CalculateAngle(double value)
    {
        return 360 * CalculatePercentageValue(value) / 100;
    }

    private Pen ConfigurePen(Pen pen, IBrush? brush, PenLineCap lineCap)
    {
        pen.Brush     = brush;
        pen.Thickness = StrokeThickness;
        pen.LineCap   = lineCap;
        return pen;
    }
}
