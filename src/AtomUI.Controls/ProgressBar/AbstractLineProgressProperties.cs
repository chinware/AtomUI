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
   
   protected double _fontSize;
   protected static readonly DirectProperty<AbstractLineProgress, double> FontSizeTokenProperty =
      AvaloniaProperty.RegisterDirect<AbstractLineProgress, double>(
         nameof(_fontSize),
         o => o._fontSize,
         (o, v) => o._fontSize = v);
   
   protected double _fontSizeSM;
   protected static readonly DirectProperty<AbstractLineProgress, double> FontSizeSMTokenProperty =
      AvaloniaProperty.RegisterDirect<AbstractLineProgress, double>(
         nameof(_fontSizeSM),
         o => o._fontSizeSM,
         (o, v) => o._fontSizeSM = v);
   // 获取 Token 值属性结束
}