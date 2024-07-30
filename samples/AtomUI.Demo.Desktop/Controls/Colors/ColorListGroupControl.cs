using AtomUI.Demo.Desktop.ViewModels;
using Avalonia;
using Avalonia.Controls.Primitives;

namespace AtomUI.Demo.Desktop.Controls;

public class ColorListGroupControl : TemplatedControl
{
   public static readonly StyledProperty<ColorGroupViewModel> GroupDataProperty = AvaloniaProperty.Register<ColorListGroupControl, ColorGroupViewModel>(
      nameof(GroupData), new ColorGroupViewModel());

   public ColorGroupViewModel GroupData
   {
      get => GetValue(GroupDataProperty);
      set => SetValue(GroupDataProperty, value);
   }
}