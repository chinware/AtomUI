using AtomUI.Icon;
using Avalonia.Data;
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
      icon.SetValue(PathIcon.AnimationProperty, Animation, BindingPriority.Template);
      icon.SetValue(PathIcon.NormalFilledBrushProperty, NormalFilledColor, BindingPriority.Template);
      icon.SetValue(PathIcon.ActiveFilledBrushProperty, ActiveFilledColor, BindingPriority.Template);
      icon.SetValue(PathIcon.SelectedFilledBrushProperty, SelectedFilledColor, BindingPriority.Template);
      icon.SetValue(PathIcon.DisabledFilledBrushProperty, DisabledFilledColor, BindingPriority.Template);
      icon.SetValue(PathIcon.PrimaryFilledBrushProperty, SelectedFilledColor, BindingPriority.Template);
      icon.SetValue(PathIcon.DisabledFilledBrushProperty, PrimaryFilledColor, BindingPriority.Template);
      icon.SetValue(PathIcon.SecondaryFilledBrushProperty, SecondaryFilledColor, BindingPriority.Template);

      if (!double.IsNaN(Width)) {
         icon.SetValue(PathIcon.WidthProperty, Width, BindingPriority.Template);
      }

      if (!double.IsNaN(Height)) {
         icon.SetValue(PathIcon.HeightProperty, Height, BindingPriority.Template);
      }
      return icon;
   }
}