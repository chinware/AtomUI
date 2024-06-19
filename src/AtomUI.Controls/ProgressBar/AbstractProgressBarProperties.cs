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
}