using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Demo.Desktop.ShowCase;

public partial class BadgeShowCase : UserControl
{
   public static readonly StyledProperty<double> DynamicBadgeCountProperty =
      AvaloniaProperty.Register<ProgressBarShowCase, double>(nameof(DynamicBadgeCount), 5);
   
   public static readonly StyledProperty<bool> DynamicDotBadgeVisibleProperty =
      AvaloniaProperty.Register<ProgressBarShowCase, bool>(nameof(DynamicDotBadgeVisible), true);
   
   public double DynamicBadgeCount
   {
      get => GetValue(DynamicBadgeCountProperty);
      set => SetValue(DynamicBadgeCountProperty, value);
   }
   
   public bool DynamicDotBadgeVisible
   {
      get => GetValue(DynamicDotBadgeVisibleProperty);
      set => SetValue(DynamicDotBadgeVisibleProperty, value);
   }
   
   public BadgeShowCase()
   {
      DataContext = this;
      InitializeComponent();
   }
   
   public void AddDynamicBadgeCount()
   {
      DynamicBadgeCount += 1;
   }

   public void SubDynamicBadgeCount()
   {
      DynamicBadgeCount -= 1;
   }
   
   public void RandomDynamicBadgeCount()
   {
      var random = new Random();
      DynamicBadgeCount = random.Next(0, 110);
   }
}