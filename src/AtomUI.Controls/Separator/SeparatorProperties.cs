using Avalonia;

namespace AtomUI.Controls;

public partial class Separator
{
   #region Control token 值绑定属性定义
   
   private double _textPaddingInline;

   private static readonly DirectProperty<Separator, double> TextPaddingInlineTokenProperty =
      AvaloniaProperty.RegisterDirect<Separator, double>(
         nameof(_textPaddingInline),
         o => o._textPaddingInline,
         (o, v) => o._textPaddingInline = v);

   private double _orientationMarginPercent;

   private static readonly DirectProperty<Separator, double> OrientationMarginPercentTokenProperty =
      AvaloniaProperty.RegisterDirect<Separator, double>(
         nameof(_orientationMarginPercent),
         o => o._orientationMarginPercent,
         (o, v) => o._orientationMarginPercent = v);
   
   private double _verticalMarginInline;

   private static readonly DirectProperty<Separator, double> VerticalMarginInlineTokenProperty =
      AvaloniaProperty.RegisterDirect<Separator, double>(
         nameof(_verticalMarginInline),
         o => o._verticalMarginInline,
         (o, v) => o._verticalMarginInline = v);
   
   #endregion
}