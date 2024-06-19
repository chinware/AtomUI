using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

public partial class RadioButton
{
   protected static readonly StyledProperty<double> RadioSizeProperty =
      AvaloniaProperty.Register<Button, double>(nameof(RadioSize), 0);

   protected double RadioSize
   {
      get => GetValue(RadioSizeProperty);
      set => SetValue(RadioSizeProperty, value);
   }
   
   protected static readonly StyledProperty<double> PaddingInlineProperty =
      AvaloniaProperty.Register<Button, double>(nameof(PaddingInline), 0);
   
   protected double PaddingInline
   {
      get => GetValue(PaddingInlineProperty);
      set => SetValue(PaddingInlineProperty, value);
   }
   
   protected static readonly StyledProperty<IBrush?> RadioBorderBrushProperty =
      AvaloniaProperty.Register<Button, IBrush?>(nameof(RadioBorderBrush));
   
   protected IBrush? RadioBorderBrush
   {
      get => GetValue(RadioBorderBrushProperty);
      set => SetValue(RadioBorderBrushProperty, value);
   }
   
   protected static readonly StyledProperty<IBrush?> RadioInnerBackgroundProperty =
      AvaloniaProperty.Register<Button, IBrush?>(nameof(RadioInnerBackground));
   
   protected IBrush? RadioInnerBackground
   {
      get => GetValue(RadioInnerBackgroundProperty);
      set => SetValue(RadioInnerBackgroundProperty, value);
   }
   
   protected static readonly StyledProperty<IBrush?> RadioBackgroundProperty =
      AvaloniaProperty.Register<Button, IBrush?>(nameof(RadioBackground));
   
   protected IBrush? RadioBackground
   {
      get => GetValue(RadioBackgroundProperty);
      set => SetValue(RadioBackgroundProperty, value);
   }
   
   protected static readonly StyledProperty<Thickness> RadioBorderThicknessProperty =
      AvaloniaProperty.Register<Button, Thickness>(nameof(RadioBorderThickness));
   
   protected Thickness RadioBorderThickness
   {
      get => GetValue(RadioBorderThicknessProperty);
      set => SetValue(RadioBorderThicknessProperty, value);
   }
   
   protected static readonly StyledProperty<double> RadioDotEffectSizeProperty =
      AvaloniaProperty.Register<Button, double>(nameof(RadioDotEffectSize));
   
   protected double RadioDotEffectSize
   {
      get => GetValue(RadioDotEffectSizeProperty);
      set => SetValue(RadioDotEffectSizeProperty, value);
   }
   
   // 获取 Token 值属性开始
   private double _dotSizeValue;

   private static readonly DirectProperty<RadioButton, double> DotSizeValueProperty =
      AvaloniaProperty.RegisterDirect<RadioButton, double>(
         nameof(_dotSizeValue),
         o => o._dotSizeValue,
         (o, v) => o._dotSizeValue = v);
   
   private double _dotPaddingValue;
   private static readonly DirectProperty<RadioButton, double> DotPaddingValueProperty =
      AvaloniaProperty.RegisterDirect<RadioButton, double>(
         nameof(_dotPaddingValue),
         o => o._dotPaddingValue,
         (o, v) => o._dotPaddingValue = v);
   
   // 获取 Token 值属性结束
}