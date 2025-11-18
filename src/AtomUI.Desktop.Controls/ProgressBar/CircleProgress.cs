using AtomUI.Media;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Desktop.Controls;

public class CircleProgress : AbstractCircleProgress
{
    private Rect _currentGrooveRect;

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
        var pen = new Pen(GrooveBrush, StrokeThickness);
        context.DrawEllipse(null, pen, _currentGrooveRect);
    }

    private void DrawGrooveStep(DrawingContext context)
    {
        var pen = new Pen(GrooveBrush, StrokeThickness)
        {
            LineCap = PenLineCap.Flat
        };
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
        var pen = new Pen(StrokeBrush, StrokeThickness)
        {
            LineCap = StrokeLineCap
        };

        double startAngle = -90;
        context.DrawArc(pen, _currentGrooveRect, startAngle, IndicatorAngle);

        if (!double.IsNaN(SuccessThreshold))
        {
            var successPen = new Pen(SuccessStrokeBrush, StrokeThickness)
            {
                LineCap = StrokeLineCap
            };
            context.DrawArc(successPen, _currentGrooveRect, startAngle, CalculateAngle(SuccessThreshold));
        }
    }

    private void DrawIndicatorBarStep(DrawingContext context)
    {
        var pen = new Pen(StrokeBrush, StrokeThickness)
        {
            LineCap = PenLineCap.Flat
        };

        var   filledSteps  = (int)Math.Round(StepCount * Percentage / 100);
        int?  successSteps = null;
        IPen? successPen   = null;

        if (!double.IsNaN(SuccessThreshold))
        {
            successPen = new Pen(SuccessStrokeBrush, StrokeThickness)
            {
                LineCap = PenLineCap.Flat
            };
            successSteps = (int)Math.Round(StepCount * SuccessThreshold / (Maximum - Minimum));
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
        return 360 * value / (Maximum - Minimum);
    }
}