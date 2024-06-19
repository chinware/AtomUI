using Avalonia;

namespace AtomUI.Controls;

public partial class SegmentedItem
{
   // 获取 Token 值属性开始
   
   private double _controlHeight;
   private static readonly DirectProperty<SegmentedItem, double> ControlHeightTokenProperty =
      AvaloniaProperty.RegisterDirect<SegmentedItem, double>(
         nameof(_controlHeight),
         o => o._controlHeight,
         (o, v) => o._controlHeight = v);
   
   private double _paddingXXS;
   private static readonly DirectProperty<SegmentedItem, double> PaddingXXSTokenProperty =
      AvaloniaProperty.RegisterDirect<SegmentedItem, double>(
         nameof(_paddingXXS),
         o => o._paddingXXS,
         (o, v) => o._paddingXXS = v);
   
   // 获取 Token 值属性结束
}