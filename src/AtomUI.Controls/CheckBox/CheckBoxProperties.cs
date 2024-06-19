using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

public partial class CheckBox
{
   protected static readonly StyledProperty<double> CheckIndicatorSizeProperty =
      AvaloniaProperty.Register<CheckBox, double>(nameof(CheckIndicatorSize), 0);

   protected double CheckIndicatorSize
   {
      get => GetValue(CheckIndicatorSizeProperty);
      set => SetValue(CheckIndicatorSizeProperty, value);
   }
   
   protected static readonly StyledProperty<double> PaddingInlineProperty =
      AvaloniaProperty.Register<CheckBox, double>(nameof(PaddingInline), 0);
   
   protected double PaddingInline
   {
      get => GetValue(PaddingInlineProperty);
      set => SetValue(PaddingInlineProperty, value);
   }
   
   protected static readonly StyledProperty<IBrush?> IndicatorBorderBrushProperty =
      AvaloniaProperty.Register<CheckBox, IBrush?>(nameof(IndicatorBorderBrush));
   
   protected IBrush? IndicatorBorderBrush
   {
      get => GetValue(IndicatorBorderBrushProperty);
      set => SetValue(IndicatorBorderBrushProperty, value);
   }
   
   protected static readonly StyledProperty<IBrush?> IndicatorCheckedMarkBrushProperty =
      AvaloniaProperty.Register<CheckBox, IBrush?>(nameof(IndicatorCheckedMarkBrush));
   
   protected IBrush? IndicatorCheckedMarkBrush
   {
      get => GetValue(IndicatorCheckedMarkBrushProperty);
      set => SetValue(IndicatorCheckedMarkBrushProperty, value);
   }
   
   protected static readonly StyledProperty<double> IndicatorCheckedMarkEffectSizeProperty =
      AvaloniaProperty.Register<CheckBox, double>(nameof(IndicatorCheckedMarkEffectSize));
   
   protected double IndicatorCheckedMarkEffectSize
   {
      get => GetValue(IndicatorCheckedMarkEffectSizeProperty);
      set => SetValue(IndicatorCheckedMarkEffectSizeProperty, value);
   }
   
   protected static readonly StyledProperty<IBrush?> IndicatorTristateMarkBrushProperty =
      AvaloniaProperty.Register<CheckBox, IBrush?>(nameof(IndicatorTristateMarkBrush));
   
   protected IBrush? IndicatorTristateMarkBrush
   {
      get => GetValue(IndicatorTristateMarkBrushProperty);
      set => SetValue(IndicatorTristateMarkBrushProperty, value);
   }
   
   protected static readonly StyledProperty<double> IndicatorTristateMarkSizeProperty =
      AvaloniaProperty.Register<CheckBox, double>(nameof(IndicatorTristateMarkSize));
   
   protected double IndicatorTristateMarkSize
   {
      get => GetValue(IndicatorTristateMarkSizeProperty);
      set => SetValue(IndicatorTristateMarkSizeProperty, value);
   }
   
   protected static readonly StyledProperty<IBrush?> IndicatorBackgroundProperty =
      AvaloniaProperty.Register<CheckBox, IBrush?>(nameof(IndicatorBackground));
   
   protected IBrush? IndicatorBackground
   {
      get => GetValue(IndicatorBackgroundProperty);
      set => SetValue(IndicatorBackgroundProperty, value);
   }
   
   protected static readonly StyledProperty<Thickness> IndicatorBorderThicknessProperty =
      AvaloniaProperty.Register<CheckBox, Thickness>(nameof(IndicatorBorderThickness));
   
   protected Thickness IndicatorBorderThickness
   {
      get => GetValue(IndicatorBorderThicknessProperty);
      set => SetValue(IndicatorBorderThicknessProperty, value);
   }
   
   protected static readonly StyledProperty<CornerRadius> IndicatorBorderRadiusProperty =
      AvaloniaProperty.Register<CheckBox, CornerRadius>(nameof(IndicatorBorderRadius));
   
   protected CornerRadius IndicatorBorderRadius
   {
      get => GetValue(IndicatorBorderRadiusProperty);
      set => SetValue(IndicatorBorderRadiusProperty, value);
   }
}