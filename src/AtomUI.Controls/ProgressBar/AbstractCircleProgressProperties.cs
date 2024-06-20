using Avalonia;

namespace AtomUI.Controls;

public partial class AbstractCircleProgress
{
   // 获取 Token 值属性开始
   protected double _circleMinimumTextFontSize;
   protected static readonly DirectProperty<AbstractCircleProgress, double> CircleMinimumTextFontSizeTokenProperty =
      AvaloniaProperty.RegisterDirect<AbstractCircleProgress, double>(
         nameof(_circleMinimumTextFontSize),
         o => o._circleMinimumTextFontSize,
         (o, v) => o._circleMinimumTextFontSize = v);
   
   protected double _circleMinimumIconSize;
   protected static readonly DirectProperty<AbstractCircleProgress, double> CircleMinimumIconSizeTokenProperty =
      AvaloniaProperty.RegisterDirect<AbstractCircleProgress, double>(
         nameof(_circleMinimumIconSize),
         o => o._circleMinimumIconSize,
         (o, v) => o._circleMinimumIconSize = v);
   // 获取 Token 值属性开始

   protected static readonly StyledProperty<double> IndicatorAngleProperty = 
         AvaloniaProperty.Register<AbstractCircleProgress, double>(nameof(IndicatorAngle));

   protected double IndicatorAngle
   {
      get => GetValue(IndicatorAngleProperty);
      set => SetValue(IndicatorAngleProperty, value);
   }
}