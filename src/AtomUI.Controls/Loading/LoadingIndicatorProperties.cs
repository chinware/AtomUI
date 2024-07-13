using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

public partial class LoadingIndicator
{
   #region Control token 值绑定属性定义
   
   private double _dotSizeToken;

   private static readonly DirectProperty<LoadingIndicator, double> DotSizeTokenProperty =
      AvaloniaProperty.RegisterDirect<LoadingIndicator, double>(
         nameof(_dotSizeToken),
         o => o._dotSizeToken,
         (o, v) => o._dotSizeToken = v);
   
   private double _dotSizeSMToken;

   private static readonly DirectProperty<LoadingIndicator, double> DotSizeSMTokenProperty =
      AvaloniaProperty.RegisterDirect<LoadingIndicator, double>(
         nameof(_dotSizeSMToken),
         o => o._dotSizeSMToken,
         (o, v) => o._dotSizeSMToken = v);
   
   private double _dotSizeLGToken;

   private static readonly DirectProperty<LoadingIndicator, double> DotSizeLGTokenProperty =
      AvaloniaProperty.RegisterDirect<LoadingIndicator, double>(
         nameof(_dotSizeLGToken),
         o => o._dotSizeLGToken,
         (o, v) => o._dotSizeLGToken = v);
   
   private TimeSpan _indicatorDurationToken;

   private static readonly DirectProperty<LoadingIndicator, TimeSpan> IndicatorDurationTokenProperty =
      AvaloniaProperty.RegisterDirect<LoadingIndicator, TimeSpan>(
         nameof(_indicatorDurationToken),
         o => o._indicatorDurationToken,
         (o, v) => o._indicatorDurationToken = v);
   
   private double _fontSizeToken;

   private static readonly DirectProperty<LoadingIndicator, double> FontSizeTokenProperty =
      AvaloniaProperty.RegisterDirect<LoadingIndicator, double>(
         nameof(_fontSizeToken),
         o => o._fontSizeToken,
         (o, v) => o._fontSizeToken = v);
   
   private double _marginXXSToken;

   private static readonly DirectProperty<LoadingIndicator, double> MarginXXSTokenProperty =
      AvaloniaProperty.RegisterDirect<LoadingIndicator, double>(
         nameof(_marginXXSToken),
         o => o._marginXXSToken,
         (o, v) => o._marginXXSToken = v);
   
   private IBrush? _colorPrimaryToken;
   private static readonly DirectProperty<LoadingIndicator, IBrush?> ColorPrimaryTokenProperty =
      AvaloniaProperty.RegisterDirect<LoadingIndicator, IBrush?>(
         nameof(_colorPrimaryToken),
         o => o._colorPrimaryToken,
         (o, v) => o._colorPrimaryToken = v);
   #endregion

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