using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

internal partial class DotBadgeAdorner
{
   #region Control token 值绑定属性定义
   private IBrush? _colorTextPlaceholderToken;

   private static readonly DirectProperty<DotBadgeAdorner, IBrush?> ColorTextPlaceholderTokenProperty =
      AvaloniaProperty.RegisterDirect<DotBadgeAdorner, IBrush?>(
         nameof(_colorTextPlaceholderToken),
         o => o._colorTextPlaceholderToken,
         (o, v) => o._colorTextPlaceholderToken = v);
   
   private IBrush? _colorErrorToken;

   private static readonly DirectProperty<DotBadgeAdorner, IBrush?> ColorErrorTokenProperty =
      AvaloniaProperty.RegisterDirect<DotBadgeAdorner, IBrush?>(
         nameof(_colorErrorToken),
         o => o._colorErrorToken,
         (o, v) => o._colorErrorToken = v);
   
   private IBrush? _colorWarningToken;
   private static readonly DirectProperty<DotBadgeAdorner, IBrush?> ColorWarningTokenProperty =
      AvaloniaProperty.RegisterDirect<DotBadgeAdorner, IBrush?>(
         nameof(_colorWarningToken),
         o => o._colorWarningToken,
         (o, v) => o._colorWarningToken = v);
   
   private IBrush? _colorSuccessToken;
   private static readonly DirectProperty<DotBadgeAdorner, IBrush?> ColorSuccessTokenProperty =
      AvaloniaProperty.RegisterDirect<DotBadgeAdorner, IBrush?>(
         nameof(_colorSuccessToken),
         o => o._colorSuccessToken,
         (o, v) => o._colorSuccessToken = v);
   
   private IBrush? _colorInfoToken;
   private static readonly DirectProperty<DotBadgeAdorner, IBrush?> ColorInfoTokenProperty =
      AvaloniaProperty.RegisterDirect<DotBadgeAdorner, IBrush?>(
         nameof(_colorInfoToken),
         o => o._colorInfoToken,
         (o, v) => o._colorInfoToken = v);
   
   private double _dotSizeToken;
   private static readonly DirectProperty<DotBadgeAdorner, double> DotSizeTokenProperty =
      AvaloniaProperty.RegisterDirect<DotBadgeAdorner, double>(
         nameof(_dotSizeToken),
         o => o._dotSizeToken,
         (o, v) => o._dotSizeToken = v);
   
   private double _statusSizeToken;
   private static readonly DirectProperty<DotBadgeAdorner, double> StatusSizeTokenProperty =
      AvaloniaProperty.RegisterDirect<DotBadgeAdorner, double>(
         nameof(_statusSizeToken),
         o => o._statusSizeToken,
         (o, v) => o._statusSizeToken = v);
   
   private double _marginXSToken;
   private static readonly DirectProperty<DotBadgeAdorner, double> MarginXSTokenProperty =
      AvaloniaProperty.RegisterDirect<DotBadgeAdorner, double>(
         nameof(_marginXSToken),
         o => o._marginXSToken,
         (o, v) => o._marginXSToken = v);
   
   private IBrush? _badgeColorToken;
   private static readonly DirectProperty<DotBadgeAdorner, IBrush?> BadgeColorTokenProperty =
      AvaloniaProperty.RegisterDirect<DotBadgeAdorner, IBrush?>(
         nameof(_badgeColorToken),
         o => o._badgeColorToken,
         (o, v) => o._badgeColorToken = v);
   
   private IBrush? _badgeShadowColorToken;
   private static readonly DirectProperty<DotBadgeAdorner, IBrush?> BadgeShadowColorTokenProperty =
      AvaloniaProperty.RegisterDirect<DotBadgeAdorner, IBrush?>(
         nameof(_badgeShadowColorToken),
         o => o._badgeShadowColorToken,
         (o, v) => o._badgeShadowColorToken = v);
   
   
   private double _badgeShadowSizeToken;
   private static readonly DirectProperty<DotBadgeAdorner, double> BadgeShadowSizeTokenProperty =
      AvaloniaProperty.RegisterDirect<DotBadgeAdorner, double>(
         nameof(_badgeShadowSizeToken),
         o => o._badgeShadowSizeToken,
         (o, v) => o._badgeShadowSizeToken = v);
   
   #endregion
}