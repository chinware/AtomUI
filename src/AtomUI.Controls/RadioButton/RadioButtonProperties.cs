using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

public partial class RadioButton
{
   internal static readonly StyledProperty<double> RadioSizeProperty =
      AvaloniaProperty.Register<Button, double>(nameof(RadioSize), 0);

   internal double RadioSize
   {
      get => GetValue(RadioSizeProperty);
      set => SetValue(RadioSizeProperty, value);
   }
   
   internal static readonly StyledProperty<double> PaddingInlineProperty =
      AvaloniaProperty.Register<Button, double>(nameof(PaddingInline), 0);
   
   internal double PaddingInline
   {
      get => GetValue(PaddingInlineProperty);
      set => SetValue(PaddingInlineProperty, value);
   }
   
   internal static readonly StyledProperty<IBrush?> RadioBorderBrushProperty =
      AvaloniaProperty.Register<Button, IBrush?>(nameof(RadioBorderBrush));
   
   internal IBrush? RadioBorderBrush
   {
      get => GetValue(RadioBorderBrushProperty);
      set => SetValue(RadioBorderBrushProperty, value);
   }
   
   internal static readonly StyledProperty<IBrush?> RadioInnerBackgroundProperty =
      AvaloniaProperty.Register<Button, IBrush?>(nameof(RadioInnerBackground));
   
   internal IBrush? RadioInnerBackground
   {
      get => GetValue(RadioInnerBackgroundProperty);
      set => SetValue(RadioInnerBackgroundProperty, value);
   }
   
   internal static readonly StyledProperty<IBrush?> RadioBackgroundProperty =
      AvaloniaProperty.Register<Button, IBrush?>(nameof(RadioBackground));
   
   internal IBrush? RadioBackground
   {
      get => GetValue(RadioBackgroundProperty);
      set => SetValue(RadioBackgroundProperty, value);
   }
   
   internal static readonly StyledProperty<Thickness> RadioBorderThicknessProperty =
      AvaloniaProperty.Register<Button, Thickness>(nameof(RadioBorderThickness));
   
   internal Thickness RadioBorderThickness
   {
      get => GetValue(RadioBorderThicknessProperty);
      set => SetValue(RadioBorderThicknessProperty, value);
   }
   
   internal static readonly StyledProperty<double> RadioDotEffectSizeProperty =
      AvaloniaProperty.Register<Button, double>(nameof(RadioDotEffectSize));
   
   internal double RadioDotEffectSize
   {
      get => GetValue(RadioDotEffectSizeProperty);
      set => SetValue(RadioDotEffectSizeProperty, value);
   }
   
   // 获取 Token 值属性开始
   private double _dotSizeValueToken;
   private static readonly DirectProperty<RadioButton, double> DotSizeValueTokenProperty =
      AvaloniaProperty.RegisterDirect<RadioButton, double>(
         nameof(_dotSizeValueToken),
         o => o._dotSizeValueToken,
         (o, v) => o._dotSizeValueToken = v);
   
   private double _dotPaddingValueToken;
   private static readonly DirectProperty<RadioButton, double> DotPaddingValueTokenProperty =
      AvaloniaProperty.RegisterDirect<RadioButton, double>(
         nameof(_dotPaddingValueToken),
         o => o._dotPaddingValueToken,
         (o, v) => o._dotPaddingValueToken = v);
   
   // 获取 Token 值属性结束
}