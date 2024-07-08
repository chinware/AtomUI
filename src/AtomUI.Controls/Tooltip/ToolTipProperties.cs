using AtomUI.ColorSystem;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;

namespace AtomUI.Controls;

public partial class ToolTip
{
   /// <summary>
   /// Defines the <see cref="Content"/> property.
   /// </summary>
   public static readonly StyledProperty<object?> ContentProperty =
      AvaloniaProperty.Register<ToolTip, object?>(nameof(Content));

   /// <summary>
   /// Defines the ToolTip.Tip attached property.
   /// </summary>
   public static readonly AttachedProperty<object?> TipProperty =
      AvaloniaProperty.RegisterAttached<ToolTip, Control, object?>("Tip");

   /// <summary>
   /// Defines the ToolTip.IsOpen attached property.
   /// </summary>
   public static readonly AttachedProperty<bool> IsOpenProperty =
      AvaloniaProperty.RegisterAttached<ToolTip, Control, bool>("IsOpen");

   /// <summary>
   /// Defines the ToolTip.PresetColor attached property.
   /// </summary>
   public static readonly AttachedProperty<PresetColorType?> PresetColorProperty =
      AvaloniaProperty.RegisterAttached<ToolTip, Control, PresetColorType?>("PresetColor");

   /// <summary>
   /// Defines the ToolTip.PresetColor attached property.
   /// </summary>
   public static readonly AttachedProperty<Color?> ColorProperty =
      AvaloniaProperty.RegisterAttached<ToolTip, Control, Color?>("Color");

   /// <summary>
   /// 是否显示指示箭头
   /// </summary>
   public static readonly AttachedProperty<bool> IsShowArrowProperty =
      AvaloniaProperty.RegisterAttached<ToolTip, Control, bool>("IsShowArrow", true);

   /// <summary>
   /// 箭头是否始终指向中心
   /// </summary>
   public static readonly AttachedProperty<bool> IsPointAtCenterProperty =
      AvaloniaProperty.RegisterAttached<ToolTip, Control, bool>("IsPointAtCenter", false);

   /// <summary>
   /// Defines the ToolTip.Placement property.
   /// </summary>
   public static readonly AttachedProperty<PlacementMode> PlacementProperty =
      AvaloniaProperty.RegisterAttached<ToolTip, Control, PlacementMode>(
         "Placement", defaultValue: PlacementMode.Top);

   /// <summary>
   /// Defines the ToolTip.HorizontalOffset property.
   /// </summary>
   public static readonly AttachedProperty<double> HorizontalOffsetProperty =
      AvaloniaProperty.RegisterAttached<ToolTip, Control, double>("HorizontalOffset");

   /// <summary>
   /// Defines the ToolTip.VerticalOffset property.
   /// </summary>
   public static readonly AttachedProperty<double> VerticalOffsetProperty =
      AvaloniaProperty.RegisterAttached<ToolTip, Control, double>("VerticalOffset");

   /// <summary>
   /// 距离 anchor 的边距，根据垂直和水平进行设置
   /// </summary>
   public static readonly AttachedProperty<double> MarginToAnchorProperty =
      AvaloniaProperty.RegisterAttached<ToolTip, Control, double>("MarginToAnchor", double.NaN);

   /// <summary>
   /// Defines the ToolTip.ShowDelay property.
   /// </summary>
   public static readonly AttachedProperty<int> ShowDelayProperty =
      AvaloniaProperty.RegisterAttached<ToolTip, Control, int>("ShowDelay", 400);

   /// <summary>
   /// Defines the ToolTip.BetweenShowDelay property.
   /// </summary>
   public static readonly AttachedProperty<int> BetweenShowDelayProperty =
      AvaloniaProperty.RegisterAttached<ToolTip, Control, int>("BetweenShowDelay", 100);

   /// <summary>
   /// Defines the ToolTip.ShowOnDisabled property.
   /// </summary>
   public static readonly AttachedProperty<bool> ShowOnDisabledProperty =
      AvaloniaProperty.RegisterAttached<ToolTip, Control, bool>("ShowOnDisabled", defaultValue: false, inherits: true);

   /// <summary>
   /// Defines the ToolTip.ServiceEnabled property.
   /// </summary>
   public static readonly AttachedProperty<bool> ServiceEnabledProperty =
      AvaloniaProperty.RegisterAttached<ToolTip, Control, bool>("ServiceEnabled", defaultValue: true, inherits: true);

   /// <summary>
   /// Stores the current <see cref="ToolTip"/> instance in the control.
   /// </summary>
   internal static readonly AttachedProperty<ToolTip?> ToolTipProperty =
      AvaloniaProperty.RegisterAttached<ToolTip, Control, ToolTip?>("ToolTip");

   public object? Content
   {
      get => GetValue(ContentProperty);
      set => SetValue(ContentProperty, value);
   }

   // 私有属性
   private static StyledProperty<ThemeVariant?> RequestedThemeVariantProperty;
   
   private static readonly StyledProperty<bool> IsShowArrowEffectiveProperty =
      ArrowDecoratedBox.IsShowArrowProperty.AddOwner<ToolTip>();
   
   /// <summary>
   /// 是否实际显示箭头
   /// </summary>
   public bool IsShowArrowEffective
   {
      get => GetValue(IsShowArrowEffectiveProperty);
      set => SetValue(IsShowArrowEffectiveProperty, value);
   }

   // 组件的 Token 绑定属性
   private IBrush? _defaultBackground;

   private static readonly DirectProperty<ToolTip, IBrush?> DefaultBgTokenProperty
      = AvaloniaProperty.RegisterDirect<ToolTip, IBrush?>(nameof(_defaultBackground),
                                                          (o) => o._defaultBackground,
                                                          (o, v) => o._defaultBackground = v);

   private double _toolTipArrowSize;

   private static readonly DirectProperty<ToolTip, double> ToolTipArrowSizeTokenProperty
      = AvaloniaProperty.RegisterDirect<ToolTip, double>(nameof(_toolTipArrowSize),
                                                         (o) => o._toolTipArrowSize,
                                                         (o, v) => o._toolTipArrowSize = v);

   private double _marginXXS;

   private static readonly DirectProperty<ToolTip, double> MarginXXSTokenProperty
      = AvaloniaProperty.RegisterDirect<ToolTip, double>(nameof(_marginXXS),
                                                         (o) => o._marginXXS,
                                                         (o, v) => o._marginXXS = v);
   
   private TimeSpan _motionDuration;

   private static readonly DirectProperty<ToolTip, TimeSpan> MotionDurationTokenProperty
      = AvaloniaProperty.RegisterDirect<ToolTip, TimeSpan>(nameof(_motionDuration),
                                                           (o) => o._motionDuration,
                                                           (o, v) => o._motionDuration = v);

   // 暂时不支持自定义
   private BoxShadows _shadows;
   
   private static readonly DirectProperty<ToolTip, BoxShadows> ShadowsTokenProperty
      = AvaloniaProperty.RegisterDirect<ToolTip, BoxShadows>(nameof(_shadows),
                                                             (o) => o._shadows,
                                                             (o, v) => o._shadows = v);
   // 组件的 Token 绑定属性

   internal static readonly DirectProperty<ToolTip, PlacementMode?> FlipPlacementProperty =
      AvaloniaProperty.RegisterDirect<ToolTip, PlacementMode?>(nameof(FlipPlacement),
                                                               o => o.FlipPlacement,
                                                               (o, v) => o.FlipPlacement = v);

   private PlacementMode? _flipPlacement;

   internal PlacementMode? FlipPlacement
   {
      get => _flipPlacement;
      set => SetAndRaise(FlipPlacementProperty, ref _flipPlacement, value);
   }
}