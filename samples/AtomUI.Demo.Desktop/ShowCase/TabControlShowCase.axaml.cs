using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Demo.Desktop.ShowCase;

public partial class TabControlShowCase : UserControl
{
   public static readonly StyledProperty<Dock> PositionTabStripPlacementProperty =
      AvaloniaProperty.Register<TabControlShowCase, Dock>(nameof(PositionTabStripPlacement), Dock.Top);
   
   public Dock PositionTabStripPlacement
   {
      get => GetValue(PositionTabStripPlacementProperty);
      set => SetValue(PositionTabStripPlacementProperty, value);
   }
   
   public TabControlShowCase()
   {
      InitializeComponent();
      DataContext = this;
      PositionTabStripOptionGroup.OptionCheckedChanged += HandleOptionCheckedChanged;
   }

   private void HandleOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
   {
      if (args.Index == 0) {
         PositionTabStripPlacement = Dock.Top;
      } else if (args.Index == 1) {
         PositionTabStripPlacement = Dock.Bottom;
      } else if (args.Index == 2) {
         PositionTabStripPlacement = Dock.Left;
      } else {
         PositionTabStripPlacement = Dock.Right;
      }
   }
}