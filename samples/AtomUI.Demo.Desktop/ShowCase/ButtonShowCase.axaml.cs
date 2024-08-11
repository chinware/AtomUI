using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Demo.Desktop.ShowCase;

public partial class ButtonShowCase : UserControl
{
   public static readonly StyledProperty<SizeType> ButtonSizeTypeProperty =
      AvaloniaProperty.Register<ButtonShowCase, SizeType>(nameof(ButtonSizeType), SizeType.Large);
   
   public SizeType ButtonSizeType
   {
      get => GetValue(ButtonSizeTypeProperty);
      set => SetValue(ButtonSizeTypeProperty, value);
   }
   
   public ButtonShowCase()
   {
      InitializeComponent();
      DataContext = this;
      
      ButtonSizeTypeOptionGroup.OptionCheckedChanged += HandleButtonSizeTypeOptionCheckedChanged;
   }
   
   private void HandleButtonSizeTypeOptionCheckedChanged(object? sender, OptionCheckedChangedEventArgs args)
   {
      if (args.Index == 0) {
         ButtonSizeType = SizeType.Large;
      } else if (args.Index == 1) {
         ButtonSizeType = SizeType.Middle;
      } else {
         ButtonSizeType = SizeType.Small;
      }
   }
}