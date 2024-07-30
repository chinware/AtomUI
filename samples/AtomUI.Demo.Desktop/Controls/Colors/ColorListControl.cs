using AtomUI.Demo.Desktop.ViewModels;
using Avalonia;
using Avalonia.Controls.Primitives;

namespace AtomUI.Demo.Desktop.Controls;

public class ColorListControl : TemplatedControl
{
   public static readonly StyledProperty<ColorListViewModel> ListDataProperty = AvaloniaProperty.Register<ColorListGroupControl, ColorListViewModel>(
      nameof(ListData), new ColorListViewModel());

   public ColorListViewModel ListData
   {
      get => GetValue(ListDataProperty);
      set => SetValue(ListDataProperty, value);
   }
}