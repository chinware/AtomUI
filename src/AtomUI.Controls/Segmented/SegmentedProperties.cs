using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

public partial class Segmented
{
   internal static readonly StyledProperty<IBrush?> SelectedThumbBgProperty =
      AvaloniaProperty.Register<Segmented, IBrush?>(
         nameof(SelectedThumbBg));
   
   internal static readonly StyledProperty<BoxShadows> SelectedThumbBoxShadowsProperty =
      AvaloniaProperty.Register<Segmented, BoxShadows>(
         nameof(SelectedThumbBoxShadows));
   
   internal IBrush? SelectedThumbBg
   {
      get => GetValue(SelectedThumbBgProperty);
      set => SetValue(SelectedThumbBgProperty, value);
   }
   
   internal BoxShadows SelectedThumbBoxShadows
   {
      get => GetValue(SelectedThumbBoxShadowsProperty);
      set => SetValue(SelectedThumbBoxShadowsProperty, value);
   }
   
   // 内部动画属性
   private static readonly StyledProperty<Size> SelectedThumbSizeProperty =
      AvaloniaProperty.Register<Segmented, Size>(nameof(SelectedThumbSize));

   private Size SelectedThumbSize
   {
      get => GetValue(SelectedThumbSizeProperty);
      set => SetValue(SelectedThumbSizeProperty, value);
   }
   
   private static readonly StyledProperty<Point> SelectedThumbPosProperty =
      AvaloniaProperty.Register<Segmented, Point>(nameof(SelectedThumbPos));

   private Point SelectedThumbPos
   {
      get => GetValue(SelectedThumbPosProperty);
      set => SetValue(SelectedThumbPosProperty, value);
   }
}