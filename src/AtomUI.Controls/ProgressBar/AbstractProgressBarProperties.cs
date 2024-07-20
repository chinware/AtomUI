using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

public partial class AbstractProgressBar
{
   // 内部动画属性
   internal static readonly StyledProperty<IBrush?> GrooveBrushProperty =
      AvaloniaProperty.Register<AbstractProgressBar, IBrush?>(nameof(GrooveBrush));
   
   internal static readonly StyledProperty<bool> PercentLabelVisibleProperty = 
      AvaloniaProperty.Register<ProgressBar, bool>(nameof(PercentLabelVisible), true);
   
   internal static readonly StyledProperty<bool> StatusIconVisibleProperty = 
      AvaloniaProperty.Register<ProgressBar, bool>(nameof(StatusIconVisible), true);
   
   internal static readonly StyledProperty<bool> IsCompletedProperty = 
      AvaloniaProperty.Register<ProgressBar, bool>(nameof(IsCompleted), false);

   internal IBrush? GrooveBrush
   {
      get => GetValue(GrooveBrushProperty);
      set => SetValue(GrooveBrushProperty, value);
   }
   
   internal bool PercentLabelVisible
   {
      get => GetValue(PercentLabelVisibleProperty);
      set => SetValue(PercentLabelVisibleProperty, value);
   }
   
   internal bool StatusIconVisible
   {
      get => GetValue(StatusIconVisibleProperty);
      set => SetValue(StatusIconVisibleProperty, value);
   }
   
   internal bool IsCompleted
   {
      get => GetValue(IsCompletedProperty);
      set => SetValue(IsCompletedProperty, value);
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