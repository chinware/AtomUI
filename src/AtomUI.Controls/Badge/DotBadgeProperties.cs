using Avalonia;

namespace AtomUI.Controls;

public partial class DotBadge
{
   #region Control token 值绑定属性定义
   private TimeSpan _motionDurationSlow;

   private static readonly DirectProperty<DotBadge, TimeSpan> MotionDurationSlowTokenProperty =
      AvaloniaProperty.RegisterDirect<DotBadge, TimeSpan>(
         nameof(_motionDurationSlow),
         o => o._motionDurationSlow,
         (o, v) => o._motionDurationSlow = v);

   #endregion
}