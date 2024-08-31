using AtomUI.Controls;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace AtomUI.Demo.Desktop.ShowCase;

public partial class InfoFlyoutShowCase : UserControl
{
   public static readonly StyledProperty<bool> ShowArrowProperty =
      AvaloniaProperty.Register<TooltipShowCase, bool>(nameof(ShowArrow), true);
   
   public static readonly StyledProperty<bool> IsPointAtCenterProperty =
      AvaloniaProperty.Register<TooltipShowCase, bool>(nameof(IsPointAtCenter), false);

   private Segmented _segmented;
   
   public bool ShowArrow
   {
      get => GetValue(ShowArrowProperty);
      set => SetValue(ShowArrowProperty, value);
   }
   
   public bool IsPointAtCenter
   {
      get => GetValue(IsPointAtCenterProperty);
      set => SetValue(IsPointAtCenterProperty, value);
   }
   
   public InfoFlyoutShowCase()
   {
      DataContext = this;
      InitializeComponent();
      var control = this as Control;
      _segmented = control.FindControl<Segmented>("ArrowSegmented")!;
      _segmented.SelectionChanged += (sender, args) =>
      {
         if (_segmented.SelectedIndex == 0) {
            ShowArrow = true;
            IsPointAtCenter = false;
         } else if (_segmented.SelectedIndex == 1) {
            ShowArrow = false;
            IsPointAtCenter = false;
         } else if (_segmented.SelectedIndex == 2) {
            IsPointAtCenter = true;
            ShowArrow = true;
         }
      };
   }
}