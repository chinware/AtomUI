using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

internal partial class SegmentedItemBox
{
   // 获取 Token 值属性开始
   private double _controlHeight;
   private static readonly DirectProperty<SegmentedItemBox, double> ControlHeightTokenProperty =
      AvaloniaProperty.RegisterDirect<SegmentedItemBox, double>(
         nameof(_controlHeight),
         o => o._controlHeight,
         (o, v) => o._controlHeight = v);
   
   private double _controlHeightLG;
   private static readonly DirectProperty<SegmentedItemBox, double> ControlHeightLGTokenProperty =
      AvaloniaProperty.RegisterDirect<SegmentedItemBox, double>(
         nameof(_controlHeightLG),
         o => o._controlHeightLG,
         (o, v) => o._controlHeightLG = v);
   
   private double _controlHeightSM;
   private static readonly DirectProperty<SegmentedItemBox, double> ControlHeightSMTokenProperty =
      AvaloniaProperty.RegisterDirect<SegmentedItemBox, double>(
         nameof(_controlHeightSM),
         o => o._controlHeightSM,
         (o, v) => o._controlHeightSM = v);
   
   
   private Thickness _trackPadding;
   private static readonly DirectProperty<SegmentedItemBox, Thickness> TrackPaddingTokenProperty
      = AvaloniaProperty.RegisterDirect<SegmentedItemBox, Thickness>(nameof(_trackPadding),
         (o) => o._trackPadding,
         (o, v) => o._trackPadding = v);
   
   private Thickness _segmentedItemPadding;
   private static readonly DirectProperty<SegmentedItemBox, Thickness> SegmentedItemPaddingTokenProperty
      = AvaloniaProperty.RegisterDirect<SegmentedItemBox, Thickness>(nameof(_segmentedItemPadding),
         (o) => o._segmentedItemPadding,
         (o, v) => o._segmentedItemPadding = v);
   
   private Thickness _segmentedItemPaddingSM;
   private static readonly DirectProperty<SegmentedItemBox, Thickness> SegmentedItemPaddingSMTokenProperty
      = AvaloniaProperty.RegisterDirect<SegmentedItemBox, Thickness>(nameof(_segmentedItemPaddingSM),
         (o) => o._segmentedItemPaddingSM,
         (o, v) => o._segmentedItemPaddingSM = v);
   
   // 获取 Token 值属性结束
}