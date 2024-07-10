using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

internal partial class DotBadgeAdorner
{
   #region Control token 值绑定属性定义
   private IBrush? _colorTextPlaceholder;

   private static readonly DirectProperty<DotBadgeAdorner, IBrush?> ColorTextPlaceholderTokenProperty =
      AvaloniaProperty.RegisterDirect<DotBadgeAdorner, IBrush?>(
         nameof(_colorTextPlaceholder),
         o => o._colorTextPlaceholder,
         (o, v) => o._colorTextPlaceholder = v);
   
   private IBrush? _colorError;

   private static readonly DirectProperty<DotBadgeAdorner, IBrush?> ColorErrorTokenProperty =
      AvaloniaProperty.RegisterDirect<DotBadgeAdorner, IBrush?>(
         nameof(_colorError),
         o => o._colorError,
         (o, v) => o._colorError = v);
   
   private IBrush? _colorWarning;
   private static readonly DirectProperty<DotBadgeAdorner, IBrush?> ColorWarningTokenProperty =
      AvaloniaProperty.RegisterDirect<DotBadgeAdorner, IBrush?>(
         nameof(_colorWarning),
         o => o._colorWarning,
         (o, v) => o._colorWarning = v);
   
   private IBrush? _colorSuccess;
   private static readonly DirectProperty<DotBadgeAdorner, IBrush?> ColorSuccessTokenProperty =
      AvaloniaProperty.RegisterDirect<DotBadgeAdorner, IBrush?>(
         nameof(_colorSuccess),
         o => o._colorSuccess,
         (o, v) => o._colorSuccess = v);
   
   private IBrush? _colorInfo;
   private static readonly DirectProperty<DotBadgeAdorner, IBrush?> ColorInfoTokenProperty =
      AvaloniaProperty.RegisterDirect<DotBadgeAdorner, IBrush?>(
         nameof(_colorInfo),
         o => o._colorInfo,
         (o, v) => o._colorInfo = v);
   
   private double _dotSize;
   private static readonly DirectProperty<DotBadgeAdorner, double> DotSizeTokenProperty =
      AvaloniaProperty.RegisterDirect<DotBadgeAdorner, double>(
         nameof(_dotSize),
         o => o._dotSize,
         (o, v) => o._dotSize = v);
   
   private double _statusSize;
   private static readonly DirectProperty<DotBadgeAdorner, double> StatusSizeTokenProperty =
      AvaloniaProperty.RegisterDirect<DotBadgeAdorner, double>(
         nameof(_statusSize),
         o => o._statusSize,
         (o, v) => o._statusSize = v);
   
   private double _marginXS;
   private static readonly DirectProperty<DotBadgeAdorner, double> MarginXSTokenProperty =
      AvaloniaProperty.RegisterDirect<DotBadgeAdorner, double>(
         nameof(_marginXS),
         o => o._marginXS,
         (o, v) => o._marginXS = v);
   
   private IBrush? _badgeColor;
   private static readonly DirectProperty<DotBadgeAdorner, IBrush?> BadgeColorTokenProperty =
      AvaloniaProperty.RegisterDirect<DotBadgeAdorner, IBrush?>(
         nameof(_badgeColor),
         o => o._badgeColor,
         (o, v) => o._badgeColor = v);
   
   private IBrush? _badgeShadowColor;
   private static readonly DirectProperty<DotBadgeAdorner, IBrush?> BadgeShadowColorTokenProperty =
      AvaloniaProperty.RegisterDirect<DotBadgeAdorner, IBrush?>(
         nameof(_badgeShadowColor),
         o => o._badgeShadowColor,
         (o, v) => o._badgeShadowColor = v);
   
   
   private double _badgeShadowSize;
   private static readonly DirectProperty<DotBadgeAdorner, double> BadgeShadowSizeTokenProperty =
      AvaloniaProperty.RegisterDirect<DotBadgeAdorner, double>(
         nameof(_badgeShadowSize),
         o => o._badgeShadowSize,
         (o, v) => o._badgeShadowSize = v);
   
   #endregion
}