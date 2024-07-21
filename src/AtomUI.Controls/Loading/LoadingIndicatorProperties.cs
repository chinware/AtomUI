using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

public partial class LoadingIndicator
{
   internal static readonly StyledProperty<double> DotSizeProperty =
      AvaloniaProperty.Register<LoadingIndicator, double>(
         nameof(DotSize));
   
   internal static readonly StyledProperty<IBrush?> DotBgBrushProperty =
      AvaloniaProperty.Register<LoadingIndicator, IBrush?>(
         nameof(DotBgBrush));

   internal double DotSize
   {
      get => GetValue(DotSizeProperty);
      set => SetValue(DotSizeProperty, value);
   }
   
   internal IBrush? DotBgBrush
   {
      get => GetValue(DotBgBrushProperty);
      set => SetValue(DotBgBrushProperty, value);
   }

   internal static readonly StyledProperty<double> IndicatorTextMarginProperty =
      AvaloniaProperty.Register<LoadingIndicator, double>(
         nameof(IndicatorTextMargin));
   
   internal double IndicatorTextMargin
   {
      get => GetValue(IndicatorTextMarginProperty);
      set => SetValue(IndicatorTextMarginProperty, value);
   }

   #region 私有属性

   // 当前指示器的角度，动画输出目标属性
  
   private static readonly DirectProperty<LoadingIndicator, double> IndicatorAngleProperty =
      AvaloniaProperty.RegisterDirect<LoadingIndicator, double>(
         nameof(IndicatorAngle),
         o => o.IndicatorAngle,
         (o, v) => o.IndicatorAngle = v);
   
   
   private double _indicatorAngle;
   private double IndicatorAngle
   {
      get => _indicatorAngle;
      set => SetAndRaise(IndicatorAngleProperty, ref _indicatorAngle, value);
   }
   #endregion
}