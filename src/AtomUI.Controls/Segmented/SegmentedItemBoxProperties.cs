using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

internal partial class SegmentedItemBox
{
   // 获取 Token 值属性开始
   private double _controlHeightToken;
   private static readonly DirectProperty<SegmentedItemBox, double> ControlHeightTokenProperty =
      AvaloniaProperty.RegisterDirect<SegmentedItemBox, double>(
         nameof(_controlHeightToken),
         o => o._controlHeightToken,
         (o, v) => o._controlHeightToken = v);
   
   private double _controlHeightLGToken;
   private static readonly DirectProperty<SegmentedItemBox, double> ControlHeightLGTokenProperty =
      AvaloniaProperty.RegisterDirect<SegmentedItemBox, double>(
         nameof(_controlHeightLGToken),
         o => o._controlHeightLGToken,
         (o, v) => o._controlHeightLGToken = v);
   
   private double _controlHeightSMToken;
   private static readonly DirectProperty<SegmentedItemBox, double> ControlHeightSMTokenProperty =
      AvaloniaProperty.RegisterDirect<SegmentedItemBox, double>(
         nameof(_controlHeightSMToken),
         o => o._controlHeightSMToken,
         (o, v) => o._controlHeightSMToken = v);
   
   
   private Thickness _trackPaddingToken;
   private static readonly DirectProperty<SegmentedItemBox, Thickness> TrackPaddingTokenProperty
      = AvaloniaProperty.RegisterDirect<SegmentedItemBox, Thickness>(nameof(_trackPaddingToken),
         (o) => o._trackPaddingToken,
         (o, v) => o._trackPaddingToken = v);
   
   private Thickness _segmentedItemPaddingToken;
   private static readonly DirectProperty<SegmentedItemBox, Thickness> SegmentedItemPaddingTokenProperty
      = AvaloniaProperty.RegisterDirect<SegmentedItemBox, Thickness>(nameof(_segmentedItemPaddingToken),
         (o) => o._segmentedItemPaddingToken,
         (o, v) => o._segmentedItemPaddingToken = v);
   
   private Thickness _segmentedItemPaddingSMToken;
   private static readonly DirectProperty<SegmentedItemBox, Thickness> SegmentedItemPaddingSMTokenProperty
      = AvaloniaProperty.RegisterDirect<SegmentedItemBox, Thickness>(nameof(_segmentedItemPaddingSMToken),
         (o) => o._segmentedItemPaddingSMToken,
         (o, v) => o._segmentedItemPaddingSMToken = v);
   
   // 获取 Token 值属性结束
}