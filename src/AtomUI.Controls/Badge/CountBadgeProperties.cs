using Avalonia;

namespace AtomUI.Controls;

public partial class CountBadge
{
   #region Control token 值绑定属性定义
   private TimeSpan _motionDurationSlow;

   private static readonly DirectProperty<CountBadge, TimeSpan> MotionDurationSlowTokenProperty =
      AvaloniaProperty.RegisterDirect<CountBadge, TimeSpan>(
         nameof(_motionDurationSlow),
         o => o._motionDurationSlow,
         (o, v) => o._motionDurationSlow = v);

   #endregion
}