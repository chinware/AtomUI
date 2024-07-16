using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

public partial class CheckBox
{
   internal static readonly StyledProperty<double> CheckIndicatorSizeProperty =
      AvaloniaProperty.Register<CheckBox, double>(nameof(CheckIndicatorSize), 0);

   internal double CheckIndicatorSize
   {
      get => GetValue(CheckIndicatorSizeProperty);
      set => SetValue(CheckIndicatorSizeProperty, value);
   }
   
   internal static readonly StyledProperty<double> PaddingInlineProperty =
      AvaloniaProperty.Register<CheckBox, double>(nameof(PaddingInline), 0);
   
   internal double PaddingInline
   {
      get => GetValue(PaddingInlineProperty);
      set => SetValue(PaddingInlineProperty, value);
   }
   
   internal static readonly StyledProperty<IBrush?> IndicatorBorderBrushProperty =
      AvaloniaProperty.Register<CheckBox, IBrush?>(nameof(IndicatorBorderBrush));
   
   internal IBrush? IndicatorBorderBrush
   {
      get => GetValue(IndicatorBorderBrushProperty);
      set => SetValue(IndicatorBorderBrushProperty, value);
   }
   
   internal static readonly StyledProperty<IBrush?> IndicatorCheckedMarkBrushProperty =
      AvaloniaProperty.Register<CheckBox, IBrush?>(nameof(IndicatorCheckedMarkBrush));
   
   internal IBrush? IndicatorCheckedMarkBrush
   {
      get => GetValue(IndicatorCheckedMarkBrushProperty);
      set => SetValue(IndicatorCheckedMarkBrushProperty, value);
   }
   
   internal static readonly StyledProperty<double> IndicatorCheckedMarkEffectSizeProperty =
      AvaloniaProperty.Register<CheckBox, double>(nameof(IndicatorCheckedMarkEffectSize));
   
   internal double IndicatorCheckedMarkEffectSize
   {
      get => GetValue(IndicatorCheckedMarkEffectSizeProperty);
      set => SetValue(IndicatorCheckedMarkEffectSizeProperty, value);
   }
   
   internal static readonly StyledProperty<IBrush?> IndicatorTristateMarkBrushProperty =
      AvaloniaProperty.Register<CheckBox, IBrush?>(nameof(IndicatorTristateMarkBrush));
   
   internal IBrush? IndicatorTristateMarkBrush
   {
      get => GetValue(IndicatorTristateMarkBrushProperty);
      set => SetValue(IndicatorTristateMarkBrushProperty, value);
   }
   
   internal static readonly StyledProperty<double> IndicatorTristateMarkSizeProperty =
      AvaloniaProperty.Register<CheckBox, double>(nameof(IndicatorTristateMarkSize));
   
   internal double IndicatorTristateMarkSize
   {
      get => GetValue(IndicatorTristateMarkSizeProperty);
      set => SetValue(IndicatorTristateMarkSizeProperty, value);
   }
   
   internal static readonly StyledProperty<IBrush?> IndicatorBackgroundProperty =
      AvaloniaProperty.Register<CheckBox, IBrush?>(nameof(IndicatorBackground));
   
   internal IBrush? IndicatorBackground
   {
      get => GetValue(IndicatorBackgroundProperty);
      set => SetValue(IndicatorBackgroundProperty, value);
   }
   
   internal static readonly StyledProperty<Thickness> IndicatorBorderThicknessProperty =
      AvaloniaProperty.Register<CheckBox, Thickness>(nameof(IndicatorBorderThickness));
   
   internal Thickness IndicatorBorderThickness
   {
      get => GetValue(IndicatorBorderThicknessProperty);
      set => SetValue(IndicatorBorderThicknessProperty, value);
   }
   
   internal static readonly StyledProperty<CornerRadius> IndicatorBorderRadiusProperty =
      AvaloniaProperty.Register<CheckBox, CornerRadius>(nameof(IndicatorBorderRadius));
   
   internal CornerRadius IndicatorBorderRadius
   {
      get => GetValue(IndicatorBorderRadiusProperty);
      set => SetValue(IndicatorBorderRadiusProperty, value);
   }
}