using Avalonia;

namespace AtomUI.Controls;

public partial class MarqueeLabel
{
   private static readonly StyledProperty<double> PivotOffsetProperty =
      AvaloniaProperty.Register<MarqueeLabel, double>(nameof(PivotOffset), 0);

   private double PivotOffset
   {
      get => GetValue(PivotOffsetProperty);
      set => SetValue(PivotOffsetProperty, value);
   }
}