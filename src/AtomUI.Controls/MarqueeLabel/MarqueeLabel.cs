using AtomUI.Data;
using AtomUI.TokenSystem;
using Avalonia;
using Avalonia.Controls;
using Avalonia.LogicalTree;

namespace AtomUI.Controls;

public partial class MarqueeLabel : TextBlock, ITokenIdProvider
{
   string ITokenIdProvider.TokenId => MarqueeLabelToken.ID;
   
   public static readonly DirectProperty<MarqueeLabel, double> CycleSpaceProperty =
      AvaloniaProperty.RegisterDirect<MarqueeLabel, double>(nameof(CycleSpace),
                                                            o => o.CycleSpace,
                                                            (o, v) => o.CycleSpace = v);

   public static readonly DirectProperty<MarqueeLabel, double> MoveSpeedProperty =
      AvaloniaProperty.RegisterDirect<MarqueeLabel, double>(nameof(MoveSpeed),
                                                            o => o.MoveSpeed,
                                                            (o, v) => o.MoveSpeed = v);
   
   public double CycleSpace
   {
      get => _cycleSpace;
      set => SetAndRaise(CycleSpaceProperty, ref _cycleSpace, value);
   }
   
   public double MoveSpeed
   {
      get => _moveSpeed;
      set => SetAndRaise(MoveSpeedProperty, ref _moveSpeed, value);
   }
   
   static MarqueeLabel()
   {
      AffectsRender<MarqueeLabel>(PivotOffsetProperty, CycleSpaceProperty, MoveSpeedProperty);   
   }
   
   public MarqueeLabel()
   {
      _tokenResourceBinder = new TokenResourceBinder(this);
      _customStyle = this;
   }

   protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
   {
      base.OnAttachedToVisualTree(e);
      HandleStartupMarqueeAnimation();
   }

   protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
   {
      base.OnDetachedFromVisualTree(e);
      HandleCleanupMarqueeAnimation();
      _pivotOffsetStartValue = 0;
   }

   protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs e)
   {
      base.OnPropertyChanged(e);
      _customStyle?.HandlePropertyChangedForStyle(e);
   }

   protected override void OnAttachedToLogicalTree(LogicalTreeAttachmentEventArgs e)
   {
      base.OnAttachedToLogicalTree(e);
      if (!_initialized) {
         _customStyle?.SetupUi();
         _initialized = true;
      }
   }
   
   protected override Size MeasureOverride(Size availableSize)
   {
      var size = base.MeasureOverride(availableSize);
      HandleLayoutUpdated(size, availableSize);
      return size;
   }
   
}