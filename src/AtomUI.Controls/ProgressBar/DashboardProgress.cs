using AtomUI.Media;
using AtomUI.Utils;
using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

public enum DashboardGapPosition
{
   Left,
   Top,
   Right,
   Bottom
};

public class DashboardProgress : AbstractCircleProgress
{
   public const double DEFAULT_GAP_DEGREE = 75;
   public const double MIN_GAP_DEGREE = 0;
   public const double MAX_GAP_DEGREE = 295;

   public static readonly StyledProperty<DashboardGapPosition> DashboardGapPositionProperty =
      AvaloniaProperty.Register<DashboardProgress, DashboardGapPosition>(
         nameof(DashboardGapPosition), DashboardGapPosition.Bottom);

   public static readonly StyledProperty<double> GapDegreeProperty =
      AvaloniaProperty.Register<DashboardProgress, double>(
         nameof(DashboardGapPosition), DEFAULT_GAP_DEGREE,
         coerce: (o, value) => NumberUtils.Clamp(value, MIN_GAP_DEGREE, MAX_GAP_DEGREE));

   public DashboardGapPosition DashboardGapPosition
   {
      get => GetValue(DashboardGapPositionProperty);
      set => SetValue(DashboardGapPositionProperty, value);
   }

   public double GapDegree
   {
      get => GetValue(GapDegreeProperty);
      set => SetValue(GapDegreeProperty, value);
   }

   private Rect _currentGrooveRect;
   private (double, double) _anglePair;

   static DashboardProgress()
   {
      AffectsRender<DashboardProgress>(DashboardGapPositionProperty,
                                       GapDegreeProperty);
   }

   protected override void RenderGroove(DrawingContext context)
   {
      var controlRect = new Rect(new Point(0, 0), DesiredSize); 
      _currentGrooveRect = GetProgressBarRect(controlRect).Deflate(StrokeThickness / 2);
      var pen = new Pen(GrooveBrush, StrokeThickness)  
      {
         LineCap = StrokeLineCap
      };
      _currentGrooveRect = new Rect(_currentGrooveRect.Position, new Size(Math.Floor(_currentGrooveRect.Size.Width), 
                                                                          Math.Floor(_currentGrooveRect.Size.Height)));
      context.DrawArc(pen, _currentGrooveRect, _anglePair.Item1, _anglePair.Item2);
   }

   protected override void RenderIndicatorBar(DrawingContext context)
   {
      var pen = new Pen(IndicatorBarBrush, StrokeThickness)  
      {
         LineCap = StrokeLineCap
      };
      context.DrawArc(pen, _currentGrooveRect, _anglePair.Item1, IndicatorAngle);
   }

   private (double, double) CalculateAngle(DashboardGapPosition position, double gapDegree)
   {
      double startAngle = 0;
      double spanAngle = 0;
      var halfGapDegree = gapDegree / 2;
      if (position == DashboardGapPosition.Bottom) {
         startAngle = 90 + halfGapDegree;
      } else if (position == DashboardGapPosition.Left) {
         startAngle = 180 + halfGapDegree;
      } else if (position == DashboardGapPosition.Top) {
         startAngle = 270 + halfGapDegree;
      } else {
         startAngle = halfGapDegree ;
      }
      spanAngle = 360 - gapDegree;
      return (startAngle, spanAngle);
   }

   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
   {
      base.OnPropertyChanged(e);
      if (e.Property == GapDegreeProperty || e.Property == DashboardGapPositionProperty) {
         _anglePair = CalculateAngle(DashboardGapPosition, GapDegree);
      }
   }
   
   protected override void NotifyUpdateProgress()
   {
      base.NotifyUpdateProgress();
      var percentage = Percentage / 100;
      IndicatorAngle = (360 - GapDegree) * percentage;
   }
}