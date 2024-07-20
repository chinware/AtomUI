using Avalonia;

namespace AtomUI.Controls;

public partial class AbstractCircleProgress
{
   // 获取 Token 值属性开始
   protected double _circleMinimumTextFontSizeToken;
   protected static readonly DirectProperty<AbstractCircleProgress, double> CircleMinimumTextFontSizeTokenProperty =
      AvaloniaProperty.RegisterDirect<AbstractCircleProgress, double>(
         nameof(_circleMinimumTextFontSizeToken),
         o => o._circleMinimumTextFontSizeToken,
         (o, v) => o._circleMinimumTextFontSizeToken = v);
   
   protected double _circleMinimumIconSizeToken;
   protected static readonly DirectProperty<AbstractCircleProgress, double> CircleMinimumIconSizeTokenProperty =
      AvaloniaProperty.RegisterDirect<AbstractCircleProgress, double>(
         nameof(_circleMinimumIconSizeToken),
         o => o._circleMinimumIconSizeToken,
         (o, v) => o._circleMinimumIconSizeToken = v);
   // 获取 Token 值属性开始

   protected static readonly StyledProperty<double> IndicatorAngleProperty = 
         AvaloniaProperty.Register<AbstractCircleProgress, double>(nameof(IndicatorAngle));

   protected double IndicatorAngle
   {
      get => GetValue(IndicatorAngleProperty);
      set => SetValue(IndicatorAngleProperty, value);
   }
}