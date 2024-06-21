using Avalonia;

namespace AtomUI.Controls;

public partial class AbstractLineProgress
{
   // 获取 Token 值属性开始
   protected double _lineProgressPadding;
   protected static readonly DirectProperty<AbstractLineProgress, double> LineProgressPaddingTokenProperty =
      AvaloniaProperty.RegisterDirect<AbstractLineProgress, double>(
         nameof(_lineProgressPadding),
         o => o._lineProgressPadding,
         (o, v) => o._lineProgressPadding = v);
   
   protected double _lineExtraInfoMargin;
   protected static readonly DirectProperty<AbstractLineProgress, double> LineExtraInfoMarginTokenProperty =
      AvaloniaProperty.RegisterDirect<AbstractLineProgress, double>(
         nameof(_lineExtraInfoMargin),
         o => o._lineExtraInfoMargin,
         (o, v) => o._lineExtraInfoMargin = v);
   
   protected double _lineInfoIconSize;
   protected static readonly DirectProperty<AbstractLineProgress, double> LineInfoIconSizeTokenProperty =
      AvaloniaProperty.RegisterDirect<AbstractLineProgress, double>(
         nameof(_lineInfoIconSize),
         o => o._lineInfoIconSize,
         (o, v) => o._lineInfoIconSize = v);
   
   protected double _lineInfoIconSizeSM;
   protected static readonly DirectProperty<AbstractLineProgress, double> LineInfoIconSizeSMTokenProperty =
      AvaloniaProperty.RegisterDirect<AbstractLineProgress, double>(
         nameof(_lineInfoIconSizeSM),
         o => o._lineInfoIconSizeSM,
         (o, v) => o._lineInfoIconSizeSM = v);
   
   // 获取 Token 值属性结束
}