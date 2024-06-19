using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

public partial class OptionButtonGroup
{
   // 获取 Token 值属性开始
   private TimeSpan _motionDuration;
   private static readonly DirectProperty<OptionButtonGroup, TimeSpan> MotionDurationTokenProperty
      = AvaloniaProperty.RegisterDirect<OptionButtonGroup, TimeSpan>(nameof(_motionDuration),
         (o) => o._motionDuration,
         (o, v) => o._motionDuration = v);
   
   private double _controlHeight;
   private static readonly DirectProperty<OptionButtonGroup, double> ControlHeightTokenProperty =
      AvaloniaProperty.RegisterDirect<OptionButtonGroup, double>(
         nameof(_controlHeight),
         o => o._controlHeight,
         (o, v) => o._controlHeight = v);
   
   private IBrush? _colorBorder;
   private static readonly DirectProperty<OptionButtonGroup, IBrush?> ColorBorderTokenProperty =
      AvaloniaProperty.RegisterDirect<OptionButtonGroup, IBrush?>(
         nameof(_colorBorder),
         o => o._colorBorder,
         (o, v) => o._colorBorder = v);
   
   private IBrush? _colorPrimary;
   private static readonly DirectProperty<OptionButtonGroup, IBrush?> ColorPrimaryTokenProperty =
      AvaloniaProperty.RegisterDirect<OptionButtonGroup, IBrush?>(
         nameof(_colorPrimary),
         o => o._colorPrimary,
         (o, v) => o._colorPrimary = v);
   
   private IBrush? _colorPrimaryHover;
   private static readonly DirectProperty<OptionButtonGroup, IBrush?> ColorPrimaryHoverTokenProperty =
      AvaloniaProperty.RegisterDirect<OptionButtonGroup, IBrush?>(
         nameof(_colorPrimaryHover),
         o => o._colorPrimaryHover,
         (o, v) => o._colorPrimaryHover = v);
   
   private IBrush? _colorPrimaryActive;
   private static readonly DirectProperty<OptionButtonGroup, IBrush?> ColorPrimaryActiveTokenProperty =
      AvaloniaProperty.RegisterDirect<OptionButtonGroup, IBrush?>(
         nameof(_colorPrimaryActive),
         o => o._colorPrimaryActive,
         (o, v) => o._colorPrimaryActive = v);
   
   // 获取 Token 值属性结束
   protected static readonly StyledProperty<IBrush?> SelectedOptionBorderColorProperty =
      AvaloniaProperty.Register<Button, IBrush?>(nameof(SelectedOptionBorderColor));
   
   protected IBrush? SelectedOptionBorderColor
   {
      get => GetValue(SelectedOptionBorderColorProperty);
      set => SetValue(SelectedOptionBorderColorProperty, value);
   }
}