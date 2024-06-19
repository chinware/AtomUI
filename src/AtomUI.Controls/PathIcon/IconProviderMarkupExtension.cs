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
   
   // Twotone
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
         Animation = Animation,
         NormalFilledBrush = NormalFilledColor,
         ActiveFilledBrush = ActiveFilledColor,
         SelectedFilledBrush = SelectedFilledColor,
         DisabledFilledBrush = DisabledFilledColor,
         
         PrimaryFilledBrush = PrimaryFilledColor,
         SecondaryFilledBrush = SecondaryFilledColor
      };
      if (!double.IsNaN(Width)) {
         icon.Width = Width;
      }

      if (!double.IsNaN(Height)) {
         icon.Height = Height;
      }
      return icon;
   }
}