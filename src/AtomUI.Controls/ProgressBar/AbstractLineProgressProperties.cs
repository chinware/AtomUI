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
   
   // 获取 Token 值属性结束
}