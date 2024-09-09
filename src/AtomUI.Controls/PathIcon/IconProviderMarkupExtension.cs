using AtomUI.Icon;
using Avalonia.Markup.Xaml;
using Avalonia.Media;

namespace AtomUI.Controls;

public class IconProvider : MarkupExtension
{
   public string Kind { get; set; }
   
   public string? PackageProvider { get; set; }
   
   // Filled 和 Outlined
   public IBrush? NormalFilledColor { get; set; }
   public IBrush? ActiveFilledColor { get; set; }
   public IBrush? SelectedFilledColor { get; set; }
   public IBrush? DisabledFilledColor { get; set; }
   
   // TwoTone
   public IBrush? PrimaryFilledColor { get; set; }
   public IBrush? SecondaryFilledColor { get; set; }
   
   public double Width { get; set; } = double.NaN;
   public double Height { get; set; } = double.NaN;
   public IconAnimation Animation { get; set; } = IconAnimation.None;

   public IconProvider()
   {
      Kind = string.Empty;
   }
   
   public IconProvider(string kind)
   {
      Kind = kind;
   }
   
   public override object ProvideValue(IServiceProvider serviceProvider)
   {
      var icon = new PathIcon()
      {
         Kind = Kind,
         PackageProvider = PackageProvider,
      };
      icon.SetCurrentValue(PathIcon.LoadingAnimationProperty, Animation);
      icon.SetCurrentValue(PathIcon.NormalFilledBrushProperty, NormalFilledColor);
      icon.SetCurrentValue(PathIcon.ActiveFilledBrushProperty, ActiveFilledColor);
      icon.SetCurrentValue(PathIcon.SelectedFilledBrushProperty, SelectedFilledColor);
      icon.SetCurrentValue(PathIcon.DisabledFilledBrushProperty, DisabledFilledColor);
      icon.SetCurrentValue(PathIcon.PrimaryFilledBrushProperty, SelectedFilledColor);
      icon.SetCurrentValue(PathIcon.DisabledFilledBrushProperty, PrimaryFilledColor);
      icon.SetCurrentValue(PathIcon.SecondaryFilledBrushProperty, SecondaryFilledColor);

      if (!double.IsNaN(Width)) {
         icon.SetCurrentValue(PathIcon.WidthProperty, Width);
      }

      if (!double.IsNaN(Height)) {
         icon.SetCurrentValue(PathIcon.HeightProperty, Height);
      }
      return icon;
   }
}