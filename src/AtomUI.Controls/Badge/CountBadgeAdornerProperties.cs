using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

internal partial class CountBadgeAdorner
{
   #region Control token 值绑定属性定义

   private IBrush? _badgeColorToken;

   private static readonly DirectProperty<CountBadgeAdorner, IBrush?> BadgeColorTokenProperty =
      AvaloniaProperty.RegisterDirect<CountBadgeAdorner, IBrush?>(
         nameof(_badgeColorToken),
         o => o._badgeColorToken,
         (o, v) => o._badgeColorToken = v);

   private IBrush? _badgeTextColorToken;

   private static readonly DirectProperty<CountBadgeAdorner, IBrush?> BadgeTextColorTokenProperty =
      AvaloniaProperty.RegisterDirect<CountBadgeAdorner, IBrush?>(
         nameof(_badgeTextColorToken),
         o => o._badgeTextColorToken,
         (o, v) => o._badgeTextColorToken = v);

   private double _indicatorHeightToken;

   private static readonly DirectProperty<CountBadgeAdorner, double> IndicatorHeightTokenProperty =
      AvaloniaProperty.RegisterDirect<CountBadgeAdorner, double>(
         nameof(_indicatorHeightToken),
         o => o._indicatorHeightToken,
         (o, v) => o._indicatorHeightToken = v);

   private double _indicatorHeightSMToken;

   private static readonly DirectProperty<CountBadgeAdorner, double> IndicatorHeightSMTokenProperty =
      AvaloniaProperty.RegisterDirect<CountBadgeAdorner, double>(
         nameof(_indicatorHeightSMToken),
         o => o._indicatorHeightSMToken,
         (o, v) => o._indicatorHeightSMToken = v);
   
   private FontWeight _textFontWeightToken;

   private static readonly DirectProperty<CountBadgeAdorner, FontWeight> TextFontWeightTokenProperty =
      AvaloniaProperty.RegisterDirect<CountBadgeAdorner, FontWeight>(
         nameof(_textFontWeightToken),
         o => o._textFontWeightToken,
         (o, v) => o._textFontWeightToken = v);
   
   private double _textFontSizeToken;

   private static readonly DirectProperty<CountBadgeAdorner, double> TextFontSizeTokenProperty =
      AvaloniaProperty.RegisterDirect<CountBadgeAdorner, double>(
         nameof(_textFontSizeToken),
         o => o._textFontSizeToken,
         (o, v) => o._textFontSizeToken = v);
   
   private double _textFontSizeSMToken;

   private static readonly DirectProperty<CountBadgeAdorner, double> TextFontSizeSMTokenProperty =
      AvaloniaProperty.RegisterDirect<CountBadgeAdorner, double>(
         nameof(_textFontSizeSMToken),
         o => o._textFontSizeSMToken,
         (o, v) => o._textFontSizeSMToken = v);
   
   private IBrush? _badgeShadowColorToken;
   private static readonly DirectProperty<CountBadgeAdorner, IBrush?> BadgeShadowColorTokenProperty =
      AvaloniaProperty.RegisterDirect<CountBadgeAdorner, IBrush?>(
         nameof(_badgeShadowColorToken),
         o => o._badgeShadowColorToken,
         (o, v) => o._badgeShadowColorToken = v);
   
   
   private double _badgeShadowSizeToken;
   private static readonly DirectProperty<CountBadgeAdorner, double> BadgeShadowSizeTokenProperty =
      AvaloniaProperty.RegisterDirect<CountBadgeAdorner, double>(
         nameof(_badgeShadowSizeToken),
         o => o._badgeShadowSizeToken,
         (o, v) => o._badgeShadowSizeToken = v);
   
   private double _paddingXSToken;

   private static readonly DirectProperty<CountBadgeAdorner, double> PaddingXSTokenProperty =
      AvaloniaProperty.RegisterDirect<CountBadgeAdorner, double>(
         nameof(_paddingXSToken),
         o => o._paddingXSToken,
         (o, v) => o._paddingXSToken = v);
   

   #endregion
}