using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

internal partial class RibbonBadgeAdorner
{
   #region Control token 值绑定属性定义
   private Point _badgeRibbonOffsetToken;
   private static readonly DirectProperty<RibbonBadgeAdorner, Point> BadgeRibbonOffsetTokenProperty =
      AvaloniaProperty.RegisterDirect<RibbonBadgeAdorner, Point>(
         nameof(_badgeRibbonOffsetToken),
         o => o._badgeRibbonOffsetToken,
         (o, v) => o._badgeRibbonOffsetToken = v);
   
   private double _marginXSToken;
   private static readonly DirectProperty<RibbonBadgeAdorner, double> MarginXSTokenProperty =
      AvaloniaProperty.RegisterDirect<RibbonBadgeAdorner, double>(
         nameof(_marginXSToken),
         o => o._marginXSToken,
         (o, v) => o._marginXSToken = v);
   
   private IBrush? _colorPrimaryToken;
   private static readonly DirectProperty<RibbonBadgeAdorner, IBrush?> ColorPrimaryTokenProperty =
      AvaloniaProperty.RegisterDirect<RibbonBadgeAdorner, IBrush?>(
         nameof(_colorPrimaryToken),
         o => o._colorPrimaryToken,
         (o, v) => o._colorPrimaryToken = v);
   
   private int _badgeRibbonCornerDarkenAmountToken;
   private static readonly DirectProperty<RibbonBadgeAdorner, int> BadgeRibbonCornerDarkenAmountTokenProperty =
      AvaloniaProperty.RegisterDirect<RibbonBadgeAdorner, int>(
         nameof(_badgeRibbonCornerDarkenAmountToken),
         o => o._badgeRibbonCornerDarkenAmountToken,
         (o, v) => o._badgeRibbonCornerDarkenAmountToken = v);
   
   private Transform? _badgeRibbonCornerTransformToken;
   private static readonly DirectProperty<RibbonBadgeAdorner, Transform?> BadgeRibbonCornerTransformTokenProperty =
      AvaloniaProperty.RegisterDirect<RibbonBadgeAdorner, Transform?>(
         nameof(_badgeRibbonCornerTransformToken),
         o => o._badgeRibbonCornerTransformToken,
         (o, v) => o._badgeRibbonCornerTransformToken = v);
   
   private CornerRadius _borderRadiusSMToken;
   private static readonly DirectProperty<RibbonBadgeAdorner, CornerRadius> BorderRadiusSMTokenProperty =
      AvaloniaProperty.RegisterDirect<RibbonBadgeAdorner, CornerRadius>(
         nameof(_borderRadiusSMToken),
         o => o._borderRadiusSMToken,
         (o, v) => o._borderRadiusSMToken = v);
   
   #endregion
}