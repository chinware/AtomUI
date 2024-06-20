using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

public partial class AbstractProgressBar
{
   // 内部动画属性
   protected static readonly StyledProperty<IBrush?> GrooveBrushProperty =
      AvaloniaProperty.Register<AbstractProgressBar, IBrush?>(nameof(GrooveBrush));

   protected IBrush? GrooveBrush
   {
      get => GetValue(GrooveBrushProperty);
      set => SetValue(GrooveBrushProperty, value);
   }
   
   // 获取 Token 值属性开始
   protected double _fontSize;
   protected static readonly DirectProperty<AbstractLineProgress, double> FontSizeTokenProperty =
      AvaloniaProperty.RegisterDirect<AbstractLineProgress, double>(
         nameof(_fontSize),
         o => o._fontSize,
         (o, v) => o._fontSize = v);
   
   protected double _fontSizeSM;
   protected static readonly DirectProperty<AbstractLineProgress, double> FontSizeSMTokenProperty =
      AvaloniaProperty.RegisterDirect<AbstractLineProgress, double>(
         nameof(_fontSizeSM),
         o => o._fontSizeSM,
         (o, v) => o._fontSizeSM = v);
   // 获取 Token 值属性结束
}