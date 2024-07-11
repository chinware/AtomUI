using Avalonia;
using Avalonia.Media;

namespace AtomUI.Controls;

internal partial class RibbonBadgeAdorner
{
   #region Control token 值绑定属性定义
   private Point _badgeRibbonOffset;
   private static readonly DirectProperty<RibbonBadgeAdorner, Point> BadgeRibbonOffsetTokenProperty =
      AvaloniaProperty.RegisterDirect<RibbonBadgeAdorner, Point>(
         nameof(_badgeRibbonOffset),
         o => o._badgeRibbonOffset,
         (o, v) => o._badgeRibbonOffset = v);
   
   private double _marginXS;
   private static readonly DirectProperty<RibbonBadgeAdorner, double> MarginXSTokenProperty =
      AvaloniaProperty.RegisterDirect<RibbonBadgeAdorner, double>(
         nameof(_marginXS),
         o => o._marginXS,
         (o, v) => o._marginXS = v);
   
   private IBrush? _colorPrimary;
   private static readonly DirectProperty<RibbonBadgeAdorner, IBrush?> ColorPrimaryTokenProperty =
      AvaloniaProperty.RegisterDirect<RibbonBadgeAdorner, IBrush?>(
         nameof(_colorPrimary),
         o => o._colorPrimary,
         (o, v) => o._colorPrimary = v);
   
   private int _badgeRibbonCornerDarkenAmount;
   private static readonly DirectProperty<RibbonBadgeAdorner, int> BadgeRibbonCornerDarkenAmountTokenProperty =
      AvaloniaProperty.RegisterDirect<RibbonBadgeAdorner, int>(
         nameof(_badgeRibbonCornerDarkenAmount),
         o => o._badgeRibbonCornerDarkenAmount,
         (o, v) => o._badgeRibbonCornerDarkenAmount = v);
   
   private Transform? _badgeRibbonCornerTransform;
   private static readonly DirectProperty<RibbonBadgeAdorner, Transform?> BadgeRibbonCornerTransformTokenProperty =
      AvaloniaProperty.RegisterDirect<RibbonBadgeAdorner, Transform?>(
         nameof(_badgeRibbonCornerTransform),
         o => o._badgeRibbonCornerTransform,
         (o, v) => o._badgeRibbonCornerTransform = v);
   
   private CornerRadius _borderRadiusSM;
   private static readonly DirectProperty<RibbonBadgeAdorner, CornerRadius> BorderRadiusSMTokenProperty =
      AvaloniaProperty.RegisterDirect<RibbonBadgeAdorner, CornerRadius>(
         nameof(_borderRadiusSM),
         o => o._borderRadiusSM,
         (o, v) => o._borderRadiusSM = v);
   
   #endregion
}