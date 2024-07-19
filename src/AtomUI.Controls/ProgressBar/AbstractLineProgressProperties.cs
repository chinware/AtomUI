using Avalonia;

namespace AtomUI.Controls;

public partial class AbstractLineProgress
{
   // 获取 Token 值属性开始
   protected double _lineProgressPaddingToken;
   protected static readonly DirectProperty<AbstractLineProgress, double> LineProgressPaddingTokenProperty =
      AvaloniaProperty.RegisterDirect<AbstractLineProgress, double>(
         nameof(_lineProgressPaddingToken),
         o => o._lineProgressPaddingToken,
         (o, v) => o._lineProgressPaddingToken = v);
   
   protected double _lineExtraInfoMarginToken;
   protected static readonly DirectProperty<AbstractLineProgress, double> LineExtraInfoMarginTokenProperty =
      AvaloniaProperty.RegisterDirect<AbstractLineProgress, double>(
         nameof(_lineExtraInfoMarginToken),
         o => o._lineExtraInfoMarginToken,
         (o, v) => o._lineExtraInfoMarginToken = v);
   
   protected double _lineInfoIconSizeToken;
   protected static readonly DirectProperty<AbstractLineProgress, double> LineInfoIconSizeTokenProperty =
      AvaloniaProperty.RegisterDirect<AbstractLineProgress, double>(
         nameof(_lineInfoIconSizeToken),
         o => o._lineInfoIconSizeToken,
         (o, v) => o._lineInfoIconSizeToken = v);
   
   protected double _lineInfoIconSizeSMToken;
   protected static readonly DirectProperty<AbstractLineProgress, double> LineInfoIconSizeSMTokenProperty =
      AvaloniaProperty.RegisterDirect<AbstractLineProgress, double>(
         nameof(_lineInfoIconSizeSMToken),
         o => o._lineInfoIconSizeSMToken,
         (o, v) => o._lineInfoIconSizeSMToken = v);
   
   // 获取 Token 值属性结束
}