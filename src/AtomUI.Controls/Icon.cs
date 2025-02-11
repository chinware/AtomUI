using AtomUI.Theme.Utils;
using Avalonia;
using Avalonia.Controls;

namespace AtomUI.Controls;

// using BaseIcon = AtomUI.IconPkg.Icon;
//
// public class Icon : BaseIcon,
//                     IAnimationAwareControl
// {
//     #region 公共属性定义
//     public static readonly StyledProperty<bool> IsMotionEnabledProperty
//         = AvaloniaProperty.Register<Icon, bool>(nameof(IsMotionEnabled), true);
//
//     public static readonly StyledProperty<bool> IsWaveAnimationEnabledProperty
//         = AvaloniaProperty.Register<Icon, bool>(nameof(IsWaveAnimationEnabled), true);
//     
//     public bool IsMotionEnabled
//     {
//         get => GetValue(IsMotionEnabledProperty);
//         set => SetValue(IsMotionEnabledProperty, value);
//     }
//
//     public bool IsWaveAnimationEnabled
//     {
//         get => GetValue(IsWaveAnimationEnabledProperty);
//         set => SetValue(IsWaveAnimationEnabledProperty, value);
//     }
//     
//     #endregion
//     
//     Control IAnimationAwareControl.PropertyBindTarget => this;
//
//     public Icon()
//     {
//         this.BindAnimationProperties(IsMotionEnabledProperty, IsWaveAnimationEnabledProperty);
//     }
// }