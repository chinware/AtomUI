using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

public partial class OptionButton
{
   // 获取 Token 值属性开始
   private TimeSpan _motionDuration;
   private static readonly DirectProperty<OptionButton, TimeSpan> MotionDurationTokenProperty
      = AvaloniaProperty.RegisterDirect<OptionButton, TimeSpan>(nameof(_motionDuration),
         (o) => o._motionDuration,
         (o, v) => o._motionDuration = v);
   
   private double _controlHeight;
   private static readonly DirectProperty<OptionButton, double> ControlHeightTokenProperty =
      AvaloniaProperty.RegisterDirect<OptionButton, double>(
         nameof(_controlHeight),
         o => o._controlHeight,
         (o, v) => o._controlHeight = v);
   
   private IBrush? _colorPrimaryHover;
   private static readonly DirectProperty<OptionButton, IBrush?> ColorPrimaryHoverTokenProperty =
      AvaloniaProperty.RegisterDirect<OptionButton, IBrush?>(
         nameof(_colorPrimaryHover),
         o => o._colorPrimaryHover,
         (o, v) => o._colorPrimaryHover = v);
   
   private IBrush? _colorPrimaryActive;
   private static readonly DirectProperty<OptionButton, IBrush?> ColorPrimaryActiveTokenProperty =
      AvaloniaProperty.RegisterDirect<OptionButton, IBrush?>(
         nameof(_colorPrimaryActive),
         o => o._colorPrimaryActive,
         (o, v) => o._colorPrimaryActive = v);
   // 获取 Token 值属性结束
}