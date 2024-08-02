using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Demo.Desktop.ShowCase;

public partial class TabControlShowCase : UserControl
{
   public static readonly StyledProperty<Dock> PositionTabStripPlacementProperty =
      AvaloniaProperty.Register<TabControlShowCase, Dock>(nameof(PositionTabStripPlacement), Dock.Top);
   
   public static readonly StyledProperty<SizeType> SizeTypeTabStripProperty =
      AvaloniaProperty.Register<TabControlShowCase, SizeType>(nameof(SizeTypeTabStrip), SizeType.Middle);
   
   public Dock PositionTabStripPlacement
   {
      get => GetValue(PositionTabStripPlacementProperty);
      set => SetValue(PositionTabStripPlacementProperty, value);
   }
   
   public SizeType SizeTypeTabStrip
   {
      get => GetValue(SizeTypeTabStripProperty);
      set => SetValue(SizeTypeTabStripProperty, value);
   }
   
   public TabControlShowCase()
   {
      InitializeComponent();
      DataContext = this;
      PositionTabStripOptionGroup.OptionCheckedChanged += HandlePlacementOptionCheckedChanged;
      SizeTypeTabStripOptionGroup.OptionCheckedChanged += HandleSizeTypeOptionCheckedChanged;
   }

   private void HandlePlacementOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
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
   
   private void HandleSizeTypeOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
   {
      if (args.Index == 0) {
         SizeTypeTabStrip = SizeType.Small;
      } else if (args.Index == 1) {
         SizeTypeTabStrip = SizeType.Middle;
      } else {
         SizeTypeTabStrip = SizeType.Large;
      }
   }
}