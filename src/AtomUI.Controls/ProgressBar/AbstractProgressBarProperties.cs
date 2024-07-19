using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

public partial class AbstractProgressBar
{
   // 内部动画属性
   internal static readonly StyledProperty<IBrush?> GrooveBrushProperty =
      AvaloniaProperty.Register<AbstractProgressBar, IBrush?>(nameof(GrooveBrush));

   internal IBrush? GrooveBrush
   {
      get => GetValue(GrooveBrushProperty);
      set => SetValue(GrooveBrushProperty, value);
   }
   
   // 获取 Token 值属性开始
   protected double _fontSizeToken;
   protected static readonly DirectProperty<AbstractLineProgress, double> FontSizeTokenProperty =
      AvaloniaProperty.RegisterDirect<AbstractLineProgress, double>(
         nameof(_fontSizeToken),
         o => o._fontSizeToken,
         (o, v) => o._fontSizeToken = v);
   
   protected double _fontSizeSMToken;
   protected static readonly DirectProperty<AbstractLineProgress, double> FontSizeSMTokenProperty =
      AvaloniaProperty.RegisterDirect<AbstractLineProgress, double>(
         nameof(_fontSizeSMToken),
         o => o._fontSizeSMToken,
         (o, v) => o._fontSizeSMToken = v);
   // 获取 Token 值属性结束
}